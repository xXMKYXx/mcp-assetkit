# Tears down docker compose stack (including volumes) and brings it back up.
# Useful when the schema or seed data has changed.

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$compose   = Join-Path $scriptDir "..\docker\docker-compose.yml"

Write-Host "Stopping containers and removing volumes..."
docker compose -f $compose down -v

Write-Host "Starting fresh..."
docker compose -f $compose up -d

Write-Host "Waiting for OpenSearch to become healthy..."
$tries = 0
while ($tries -lt 30) {
    try {
        $health = Invoke-RestMethod -Method Get -Uri "http://localhost:9200/_cluster/health" -TimeoutSec 2
        if ($health.status -in @("green","yellow")) {
            Write-Host "OpenSearch ready (status=$($health.status))."
            break
        }
    } catch { }
    Start-Sleep -Seconds 2
    $tries++
}

Write-Host "Reseeding OpenSearch..."
& (Join-Path $scriptDir "seed-opensearch.ps1")

Write-Host "Reset complete."
