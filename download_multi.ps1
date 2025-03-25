param(
    [Parameter(Mandatory = $true)]
    [Alias("v")]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [Alias("o")]
    [string]$DownloadOutput = '',
    
    [Parameter(Mandatory = $false)]
    [Alias("o")]
    [string]$UnzipOutput = '',

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$Help
)

if ($Help) {
    Write-Host "multi help"
    return
}
if ([string]::IsNullOrEmpty($Version)) {
    throw "Invalid version name"
}

if ([string]::IsNullOrEmpty($DownloadOutput)) {
    $DownloadOutput = (Join-Path -Path [Environment]::GetFolderPath("UserProfile") -ChildPath "Downloads")
}

$unzip = -not [string]::IsNullOrEmpty($UnzipOutput)

if (-not (Test-Path -Path $DownloadOutput)) {
    New-Item -ItemType Directory -Path $DownloadOutput -Force | Out-Null
    Write-Host "Created download output directory: $DownloadOutput"
}

# Download logic
$url = "https://download.enova.pl/instalatory/net/Soneta.Standard.$($Version).zip"
Write-Host "Downloading multi version from: $url"

function Download {
    param (
        [string]$Version,
        [string]$Output
    )
    
    # Validate output parameter
    if ([string]::IsNullOrEmpty($Output)) {
        throw "Output directory is required for 'multi' command"
    }
    
    $url = "https://download.enova.pl/instalatory/net/Soneta.Standard.$($Version).zip"
    Write-Host "Downloading multi version from: $url" -ForegroundColor Cyan
    
    # Create a temporary directory for download
    $tempDir = [Path]::Combine([Path]::GetTempPath(), [Guid]::NewGuid().ToString())
    New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
    Write-Host "Created temporary directory: $tempDir" -ForegroundColor Gray
    
    # Download the ZIP file
    $zipPath = [Path]::Combine($tempDir, "Soneta.Standard.$($Version).zip")
    try {
        Write-Host "Downloading ZIP file..." -ForegroundColor Yellow
        $ProgressPreference = 'SilentlyContinue'  # Hide progress bar for faster downloads
        Invoke-WebRequest -Uri $url -OutFile $zipPath
        Write-Host "Download complete. File size: $([Math]::Round((Get-Item $zipPath).Length / 1MB, 2)) MB" -ForegroundColor Green
    }
    catch {
        throw "Failed to download ZIP file: $_"
    }

    # Ask user if they want to unzip it
    if ((Read-Host "Do you want to unzip the downloaded file? (yes/no)") -notmatch "^(yes|y)$") {
        return
    }

    # Unlock the ZIP file if it's blocked
    try {
        Write-Host "Checking if file is blocked..." -ForegroundColor Gray
        $zone = Get-Item $zipPath -Stream "Zone.Identifier" -ErrorAction SilentlyContinue
        if ($zone) {
            Write-Host "Unblocking ZIP file..." -ForegroundColor Yellow
            Unblock-File -Path $zipPath
            Write-Host "File unblocked successfully" -ForegroundColor Green
        }
        else {
            Write-Host "File is not blocked" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "Warning: Could not check or unblock file: $_" -ForegroundColor Yellow
    }
    
    # Create output directory if it doesn't exist
    if (-not (Test-Path -Path $Output)) {
        New-Item -ItemType Directory -Path $Output -Force | Out-Null
        Write-Host "Created output directory: $Output" -ForegroundColor Green
    }
    
    # Extract the ZIP file
    try {
        Write-Host "Extracting ZIP file to output directory..." -ForegroundColor Yellow
        
        # Use .NET to extract the ZIP
        [ZipFile]::ExtractToDirectory($zipPath, $Output)
        
        Write-Host "Extraction complete: $Output" -ForegroundColor Green
    }
    catch {
        throw "Failed to extract ZIP file: $_"
    }
    finally {
        # Clean up temporary files
        try {
            Write-Host "Cleaning up temporary files..." -ForegroundColor Gray
            Remove-Item -Path $tempDir -Recurse -Force
            Write-Host "Cleanup complete" -ForegroundColor Gray
        }
        catch {
            Write-Host "Warning: Failed to clean up temporary files: $_" -ForegroundColor Yellow
        }
    }
}

function Download-Explorer {
    param (
        [string]$Version
    )
    
    $url = "https://download.enova.pl/instalatory/net/enova365_$($Version)_instalator.exe"
    Write-Host "Downloading explorer installer from: $url" -ForegroundColor Cyan
    
    # Download the installer
    try {
        $outputFile = "enova365_$($Version)_installer.exe"
        Write-Host "Downloading installer to current directory..." -ForegroundColor Yellow
        Invoke-WebRequest -Uri $url -OutFile $outputFile
        Write-Host "Download complete: $outputFile" -ForegroundColor Green
    }
    catch {
        throw "Failed to download installer: $_"
    }
}

# Parameter handling
param(
    [Parameter(Position = 0, Mandatory = $false)]
    [ValidateSet("multi", "explorer")]
    [string]$Command,
    
    [Parameter(Mandatory = $false)]
    [Alias("v")]
    [string]$Version,
    
    [Parameter(Mandatory = $false)]
    [Alias("o")]
    [string]$Output = (Join-Path -Path [Environment]::GetFolderPath("UserProfile") -ChildPath "Downloads"),

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$Help
)

# Main execution
try {
    # Handle help parameter
    if ($Help) {
        Show-Help -Command $Command
        return
    }
    
    # No command specified
    if ([string]::IsNullOrEmpty($Command)) {
        Write-Host "Please specify a command: 'multi' or 'explorer'" -ForegroundColor Yellow
        Write-Host "Use -h for help" -ForegroundColor Yellow
        return
    }
    
    # Execute command
    switch ($Command) {
        "multi" {
            Download-Multi -Version $Version -Output $Output
        }
        "explorer" {
            Download-Explorer -Version $Version
        }
        default {
            Write-Host "Unknown command: $Command" -ForegroundColor Red
            Write-Host "Use -h for help" -ForegroundColor Yellow
        }
    }
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}