"""Drive the mcp-assetkit server over stdio using bare JSON-RPC.

Not part of the project surface — just a smoke test that proves the server
boots, registers tools, and returns sensible data. Real MCP clients
(Claude Code etc.) use the SDK clients instead of speaking the protocol by hand.
"""
import json
import subprocess
import sys
import threading
import queue
import os

SERVER_DLL = r"D:\dev\mcp-assetkit\src\McpAssetKit.Server\bin\Debug\net8.0\McpAssetKit.Server.dll"

env = os.environ.copy()
env["ASSETKIT_MARIADB"] = "Server=localhost;Port=3306;Database=assetkit;User=assetkit;Password=assetkit;"
env["ASSETKIT_OPENSEARCH"] = "http://localhost:9200"

proc = subprocess.Popen(
    ["dotnet", SERVER_DLL],
    stdin=subprocess.PIPE,
    stdout=subprocess.PIPE,
    stderr=subprocess.PIPE,
    env=env,
    bufsize=0,
)

out_q = queue.Queue()

def reader():
    while True:
        line = proc.stdout.readline()
        if not line:
            break
        out_q.put(line.decode("utf-8").rstrip())

def err_reader():
    while True:
        line = proc.stderr.readline()
        if not line:
            break
        sys.stderr.write("[server] " + line.decode("utf-8", errors="replace"))
        sys.stderr.flush()

threading.Thread(target=reader, daemon=True).start()
threading.Thread(target=err_reader, daemon=True).start()


def send(req):
    line = json.dumps(req) + "\n"
    proc.stdin.write(line.encode("utf-8"))
    proc.stdin.flush()


def recv(timeout=10):
    return out_q.get(timeout=timeout)


def call(method, params=None, rid=1):
    req = {"jsonrpc": "2.0", "id": rid, "method": method}
    if params is not None:
        req["params"] = params
    send(req)
    return json.loads(recv())


try:
    # 1. Initialize
    init = call("initialize", {
        "protocolVersion": "2024-11-05",
        "capabilities": {},
        "clientInfo": {"name": "smoke-mcp", "version": "0.0.1"},
    }, rid=1)
    print("# initialize OK; server:", init.get("result", {}).get("serverInfo"))

    # Notifications/initialized — no response expected
    send({"jsonrpc": "2.0", "method": "notifications/initialized"})

    # 2. List tools
    listed = call("tools/list", {}, rid=2)
    tools = listed.get("result", {}).get("tools", [])
    names = [t["name"] for t in tools]
    print("# tools registered:", names)
    laby = next((t for t in tools if t["name"] == "list_assets_by_client"), None)
    if laby:
        print("# list_assets_by_client input schema:", json.dumps(laby.get("inputSchema"), indent=2))

    # 3. Call list_assets_by_client for Acme (c1) — full list, will filter for warranty here
    res = call("tools/call", {
        "name": "list_assets_by_client",
        "arguments": {"clientId": "c1", "limit": 500},
    }, rid=3)
    print("# raw list_assets_by_client response:", json.dumps(res, indent=2))
    contents = res.get("result", {}).get("content", [])
    payload = contents[0]["text"] if contents and contents[0].get("text") else "[]"
    assets = json.loads(payload)
    from datetime import date, timedelta
    today = date.today()
    soon = today + timedelta(days=90)

    def in_window(a):
        w = a.get("warranty_expires_at")
        if not w:
            return False
        d = date.fromisoformat(w)
        return today <= d <= soon

    expiring = [a for a in assets if in_window(a)]
    print(f"# Acme (c1) total assets: {len(assets)}; warranty expiring within 90d: {len(expiring)}")
    for a in expiring:
        print(f"  - {a['id']}: {a['vendor']} {a['model']} ({a['asset_type']}) — warranty {a['warranty_expires_at']}")

    # 4. Call warranty_status_counts (global)
    res = call("tools/call", {
        "name": "warranty_status_counts",
        "arguments": {},
    }, rid=4)
    contents = res.get("result", {}).get("content", [])
    payload = contents[0]["text"] if contents else "{}"
    print("# warranty_status_counts (global):", payload)

finally:
    proc.stdin.close()
    proc.terminate()
    try:
        proc.wait(timeout=5)
    except subprocess.TimeoutExpired:
        proc.kill()
