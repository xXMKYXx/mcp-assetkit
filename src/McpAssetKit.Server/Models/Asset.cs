namespace McpAssetKit.Server.Models;

public sealed record Asset(
    string Id,
    string ClientId,
    string AssetType,
    string Vendor,
    string Model,
    string Serial,
    string Status,
    DateOnly? WarrantyExpiresAt);
