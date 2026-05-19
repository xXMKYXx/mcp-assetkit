# Seeds the OpenSearch 'assets' index with mapping + initial documents.
# Idempotent on a clean cluster; will fail "already exists" if rerun without reset.

$ErrorActionPreference = "Stop"
$base = "http://localhost:9200"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$mapping   = Join-Path $scriptDir "..\opensearch\index-mapping.json"
$seed      = Join-Path $scriptDir "..\opensearch\seed.ndjson"

Write-Host "Creating 'assets' index..."
Invoke-RestMethod -Method Put -Uri "$base/assets" `
    -ContentType "application/json" -InFile $mapping | Out-Null

Write-Host "Bulk-indexing seed documents..."
Invoke-RestMethod -Method Post -Uri "$base/assets/_bulk" `
    -ContentType "application/x-ndjson" -InFile $seed | Out-Null

Write-Host "Refreshing index..."
Invoke-RestMethod -Method Post -Uri "$base/assets/_refresh" | Out-Null

$count = (Invoke-RestMethod -Method Get -Uri "$base/assets/_count").count
Write-Host "Seeded $count documents."
