using System.ComponentModel;
using System.Text.Json;
using McpAssetKit.Server.Storage;
using ModelContextProtocol.Server;

namespace McpAssetKit.Server.Tools;

[McpServerToolType]
public static class AssetTools
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    [McpServerTool, Description(
        "Full-text search across MSP-style assets (vendor, model, client name). " +
        "Backed by OpenSearch. Use this for free-text queries like 'Dell laptop' or 'Acme firewall'.")]
    public static async Task<string> SearchAssets(
        OpenSearchAssetIndex index,
        [Description("Free-text query, e.g. 'Dell laptop' or 'Acme firewall'")] string query,
        [Description("Max results to return (1–50)")] int limit = 10,
        CancellationToken ct = default)
    {
        var results = await index.SearchAsync(query, Math.Clamp(limit, 1, 50), ct);
        return JsonSerializer.Serialize(results, JsonOpts);
    }

    [McpServerTool, Description(
        "Get a single asset by ID from the authoritative MariaDB store. Returns null if not found.")]
    public static async Task<string> GetAsset(
        MariaDbAssetStore store,
        [Description("Asset ID, e.g. 'a-0001'")] string assetId,
        CancellationToken ct = default)
    {
        var asset = await store.GetAsync(assetId, ct);
        return JsonSerializer.Serialize(asset, JsonOpts);
    }

    [McpServerTool, Description(
        "List assets for a client, optionally filtered by status. " +
        "Status values: Active, Decommissioned, InRepair.")]
    public static async Task<string> ListAssetsByClient(
        MariaDbAssetStore store,
        [Description("Client ID, e.g. 'c1'")] string clientId,
        [Description("Optional status filter: Active, Decommissioned, or InRepair. Omit for all statuses.")] string? status = null,
        [Description("Max results to return (1–500)")] int limit = 100,
        CancellationToken ct = default)
    {
        var assets = await store.ListByClientAsync(clientId, status, Math.Clamp(limit, 1, 500), ct);
        return JsonSerializer.Serialize(assets, JsonOpts);
    }

    [McpServerTool, Description(
        "List all MSP clients. Returns client ID, name, and industry — useful as a starting " +
        "point for navigating the inventory.")]
    public static async Task<string> ListClients(
        MariaDbAssetStore store,
        CancellationToken ct = default)
    {
        var clients = await store.ListClientsAsync(ct);
        return JsonSerializer.Serialize(clients, JsonOpts);
    }

    [McpServerTool, Description(
        "Aggregate counts of assets by warranty bucket: Expired, ExpiringSoon (next 90 days), Active. " +
        "Backed by an OpenSearch date_range aggregation.")]
    public static async Task<string> WarrantyStatusCounts(
        OpenSearchAssetIndex index,
        CancellationToken ct = default)
    {
        var counts = await index.CountByWarrantyStatusAsync(ct);
        return JsonSerializer.Serialize(counts, JsonOpts);
    }
}
