param(
    [Parameter(Mandatory = $true)]
    [Alias("v")]
    [string]$Version,
    
    [Parameter(Mandatory = $false)]
    [Alias("o")]
    [string]$Output = (Join-Path -Path [Environment]::GetFolderPath("UserProfile") -ChildPath "Downloads"),

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$Help
)

if ($Help) {
    Write-Host "explorer help"
    return
}

# Download logic
$url = "https://download.enova.pl/instalatory/net/enova365_$($Version)_instalator.exe"
Write-Host "Downloading explorer installer from: $url"
# Add download logic here