using OpenSearch.Client;
using OpenSearch.Net;

namespace McpAssetKit.Server.Storage;

public sealed class OpenSearchAssetIndex
{
    private readonly IOpenSearchClient _client;
    private const string IndexName = "assets";

    public OpenSearchAssetIndex(string uri)
    {
        var pool = new SingleNodeConnectionPool(new Uri(uri));
        var settings = new ConnectionSettings(pool)
            .DefaultIndex(IndexName)
            .DefaultFieldNameInferrer(SnakeCase);
        _client = new OpenSearchClient(settings);
    }

    public async Task<IReadOnlyList<AssetSearchHit>> SearchAsync(
        string query,
        int limit,
        CancellationToken ct = default)
    {
        var resp = await _client.SearchAsync<AssetSearchHit>(s => s
            .Size(limit)
            .Query(q => q.MultiMatch(m => m
                .Fields(f => f
                    .Field("client_name^2")
                    .Field("vendor")
                    .Field("model")
                    .Field("search_blob"))
                .Query(query)
                .Fuzziness(Fuzziness.Auto))),
            ct);

        return resp.IsValid ? resp.Documents.ToList() : Array.Empty<AssetSearchHit>();
    }

    public async Task<IReadOnlyDictionary<string, long>> CountByWarrantyStatusAsync(
        CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var soon = today.AddDays(90);

        var resp = await _client.SearchAsync<AssetSearchHit>(s => s
            .Size(0)
            .Aggregations(a => a
                .DateRange("warranty_buckets", dr => dr
                    .Field("warranty_expires_at")
                    .Ranges(
                        r => r.Key("Expired").To(today),
                        r => r.Key("ExpiringSoon").From(today).To(soon),
                        r => r.Key("Active").From(soon)))),
            ct);

        if (!resp.IsValid)
        {
            return new Dictionary<string, long>
            {
                ["Expired"] = 0,
                ["ExpiringSoon"] = 0,
                ["Active"] = 0,
            };
        }

        var buckets = resp.Aggregations.DateRange("warranty_buckets");
        return buckets.Buckets.ToDictionary(b => b.Key, b => b.DocCount);
    }

    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        var resp = await _client.PingAsync(ct: ct);
        return resp.IsValid;
    }

    // PascalCase → snake_case. AssetType → asset_type, ClientName → client_name.
    private static string SnakeCase(string name)
    {
        var sb = new System.Text.StringBuilder(name.Length + 4);
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (i > 0 && char.IsUpper(c) && (char.IsLower(name[i - 1]) || (i + 1 < name.Length && char.IsLower(name[i + 1]))))
            {
                sb.Append('_');
            }
            sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }
}
