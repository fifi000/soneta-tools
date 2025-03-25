param(
    [Parameter(Position = 0, Mandatory = $true)]
    [ValidateSet("multi", "explorer")]
    [string]$Command,

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$Help
)

# show specified command help

if ($Help) {
    switch ($Command) {
        "multi" {
            & .\download_multi.ps1 -Help
        }
        "explorer" {
            & .\download_explorer.ps1 -Help
        }
    }
    return
}

# pass args to specified command

switch ($Command) {
    "multi" {
        & .\download_multi.ps1 @args
    }
    "explorer" {
        & .\download_explorer.ps1 @args
    }
    default {
        Write-Host "Unknown command: $Command" -ForegroundColor Red
        Write-Host "Use -Help for usage information" -ForegroundColor Yellow
    }
}