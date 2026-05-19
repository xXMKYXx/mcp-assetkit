using McpAssetKit.Server.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

// MCP servers speak on stdio, so any console logging would corrupt the protocol.
// Route logs to stderr only.
builder.Logging.AddConsole(o =>
{
    o.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Connection strings sourced from env vars so the same binary works against
// any environment without rebuilding. Defaults point at the docker-compose stack.
var mariaDbConnectionString =
    Environment.GetEnvironmentVariable("ASSETKIT_MARIADB")
    ?? "Server=localhost;Port=3306;Database=assetkit;User=assetkit;Password=assetkit;";

var openSearchUri =
    Environment.GetEnvironmentVariable("ASSETKIT_OPENSEARCH")
    ?? "http://localhost:9200";

builder.Services.AddSingleton(new MariaDbAssetStore(mariaDbConnectionString));
builder.Services.AddSingleton(new OpenSearchAssetIndex(openSearchUri));

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
