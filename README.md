# mcp-assetkit

A small C# Model Context Protocol (MCP) server exposing an MSP-style asset inventory across **MariaDB** (OLTP) and **OpenSearch** (search / analytics). MCP clients — Claude Code, etc. — can call tools to list, search, and inspect assets across multiple "MSP clients."

The project mirrors the data-backbone shape common in MSP-platform tooling: a relational source of truth for operational data, plus a denormalized search index for low-latency text queries and aggregations.

## Stack

- **.NET 8** — C# 12, attribute-based MCP tool definitions
- **MCP C# SDK** (`ModelContextProtocol`) — stdio server, tool surface
- **MariaDB 11** — OLTP store (clients, assets, warranties)
- **OpenSearch 2** — full-text search + warranty aggregations
- **Dapper + MySqlConnector** — minimal SQL data access
- **OpenSearch.Client** — official .NET client
- **Docker Compose** — local-dev backing services

## What the server exposes

| Tool | Purpose |
|---|---|
| `search_assets` | Free-text search via OpenSearch — multi-field, fuzzy |
| `get_asset` | Single-asset lookup by ID from MariaDB |
| `list_assets_by_client` | All assets for a client, optionally filtered by status |
| `list_clients` | Enumerate available MSP clients |
| `warranty_status_counts` | Aggregation: expired / expiring soon / active asset counts |

## Quick start

Requires Docker Desktop, .NET 8 SDK.

```powershell
git clone git@github.com:xXMKYXx/mcp-assetkit.git
cd mcp-assetkit

# Bring up MariaDB + OpenSearch
docker compose -f docker/docker-compose.yml up -d

# Wait ~30s for services to be healthy, then seed OpenSearch
./scripts/seed-opensearch.ps1

# Build and run the MCP server
dotnet run --project src/McpAssetKit.Server
```

The MCP server speaks over **stdio**. Point an MCP client at it via your client's server configuration.

## Project layout

```
mcp-assetkit/
├── docker/
│   └── docker-compose.yml           # MariaDB + OpenSearch
├── db/
│   └── init.sql                     # Schema + ~50 seed assets across 5 clients
├── opensearch/
│   ├── index-mapping.json
│   └── seed.ndjson
├── scripts/
│   ├── seed-opensearch.ps1
│   └── reset.ps1
├── src/
│   └── McpAssetKit.Server/          # C# MCP server
└── tests/
    └── McpAssetKit.Server.Tests/    # Smoke tests
```

## Demo / dev only

This project is a scoped portfolio piece, not a production system:

- OpenSearch security plugin is disabled for simplicity
- MariaDB root password is in plaintext in compose
- No auth, no tenancy beyond a `client_id` column
- No retention or backup story

A production deployment would re-enable the security plugin, source credentials from a secret store, layer service-to-service auth, and implement per-tenant isolation.

## License

MIT — see [LICENSE](LICENSE).
