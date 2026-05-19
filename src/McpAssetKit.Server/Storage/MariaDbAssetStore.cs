using Dapper;
using McpAssetKit.Server.Models;
using MySqlConnector;

namespace McpAssetKit.Server.Storage;

public sealed class MariaDbAssetStore
{
    private readonly string _connectionString;

    static MariaDbAssetStore()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }

    public MariaDbAssetStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    private const string AssetColumns =
        "id AS Id, " +
        "client_id AS ClientId, " +
        "asset_type AS AssetType, " +
        "vendor AS Vendor, " +
        "model AS Model, " +
        "serial AS Serial, " +
        "status AS Status, " +
        "warranty_expires_at AS WarrantyExpiresAt ";

    public async Task<Asset?> GetAsync(string id, CancellationToken ct = default)
    {
        await using var conn = new MySqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Asset>(
            new CommandDefinition(
                $"SELECT {AssetColumns} FROM assets WHERE id = @id LIMIT 1",
                new { id },
                cancellationToken: ct));
    }

    public async Task<IReadOnlyList<Asset>> ListByClientAsync(
        string clientId,
        string? status,
        int limit,
        CancellationToken ct = default)
    {
        await using var conn = new MySqlConnection(_connectionString);
        var sql =
            $"SELECT {AssetColumns} FROM assets WHERE client_id = @clientId " +
            (status is null ? "" : "AND status = @status ") +
            "ORDER BY created_at DESC LIMIT @limit";
        var rows = await conn.QueryAsync<Asset>(
            new CommandDefinition(sql, new { clientId, status, limit }, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<Client>> ListClientsAsync(CancellationToken ct = default)
    {
        await using var conn = new MySqlConnection(_connectionString);
        var rows = await conn.QueryAsync<Client>(
            new CommandDefinition(
                "SELECT id AS Id, name AS Name, industry AS Industry FROM clients ORDER BY name",
                cancellationToken: ct));
        return rows.AsList();
    }
}
