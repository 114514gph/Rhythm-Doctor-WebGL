# Rhythm Doctor WebGL - Local Web Server
# Usage: Right-click -> Run with PowerShell, or: powershell -ExecutionPolicy Bypass -File start-server.ps1

$port = 8080
$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("http://localhost:$port/")
$listener.Start()

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Rhythm Doctor WebGL Server" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  URL: http://localhost:$port/" -ForegroundColor Green
Write-Host ""
Write-Host "  Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

# Open browser
Start-Process "http://localhost:$port/"

function Get-ContentType($filePath) {
    $fileName = [System.IO.Path]::GetFileName($filePath)
    $extension = [System.IO.Path]::GetExtension($filePath)
    
    # Special handling for .gz files - check the underlying extension
    if ($extension -eq ".gz") {
        $baseName = $fileName -replace '\.gz$', ''
        $baseExt = [System.IO.Path]::GetExtension($baseName)
        switch ($baseExt) {
            ".wasm" { return @("application/wasm", $true) }
            ".js" { return @("application/javascript", $true) }
            ".data" { return @("application/octet-stream", $true) }
            ".json" { return @("application/json", $true) }
            ".html" { return @("text/html", $true) }
            ".css" { return @("text/css", $true) }
            default { return @("application/gzip", $true) }
        }
    }
    
    switch ($extension) {
        ".html" { return @("text/html", $false) }
        ".js" { return @("application/javascript", $false) }
        ".css" { return @("text/css", $false) }
        ".json" { return @("application/json", $false) }
        ".png" { return @("image/png", $false) }
        ".jpg" { return @("image/jpeg", $false) }
        ".gif" { return @("image/gif", $false) }
        ".svg" { return @("image/svg+xml", $false) }
        ".ico" { return @("image/x-icon", $false) }
        ".wasm" { return @("application/wasm", $false) }
        ".data" { return @("application/octet-stream", $false) }
        default { return @("application/octet-stream", $false) }
    }
}

try {
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $request = $context.Request
        $response = $context.Response
        
        $urlPath = $request.Url.LocalPath
        if ($urlPath -eq "/") { $urlPath = "/index.html" }
        
        $filePath = Join-Path $PSScriptRoot $urlPath.TrimStart("/")
        
        if (Test-Path $filePath -PathType Leaf) {
            $content = [System.IO.File]::ReadAllBytes($filePath)
            $ctResult = Get-ContentType $filePath
            $response.ContentType = $ctResult[0]
            
            if ($ctResult[1]) {
                $response.AddHeader("Content-Encoding", "gzip")
            }
            
            $response.ContentLength64 = $content.Length
            $response.OutputStream.Write($content, 0, $content.Length)
        } else {
            $response.StatusCode = 404
            $buffer = [System.Text.Encoding]::UTF8.GetBytes("404 Not Found")
            $response.ContentLength64 = $buffer.Length
            $response.OutputStream.Write($buffer, 0, $buffer.Length)
        }
        
        $response.OutputStream.Close()
    }
} finally {
    $listener.Stop()
    Write-Host "`nServer stopped." -ForegroundColor Yellow
}
