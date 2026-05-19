namespace McpAssetKit.Server.Storage;

// Mirrors the OpenSearch 'assets' index document shape. Distinct from the
// OLTP Asset record because the search index is denormalized (includes
// client_name) and is the source of truth for search-time results.
public sealed record AssetSearchHit(
    string AssetId,
    string ClientId,
    string ClientName,
    string AssetType,
    string Vendor,
    string Model,
    string Serial,
    string Status,
    string? WarrantyExpiresAt);
