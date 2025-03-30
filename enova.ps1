param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("multi", "explorer", "clean", "help")]
    [string]$Command = "help",

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$Help
)

if ($Help.IsPresent -or $Command -eq "help" -or $Command -eq "") {
    Write-Host "enova help"
    return
}


#region utils


function getDownloadFolder {
    return Join-Path -Path ([Environment]::GetFolderPath("UserProfile")) -ChildPath "Downloads"
}

function validateFilename {
    param(
        [string]$filename
    )

    if ([string]::IsNullOrWhiteSpace($filename)) {
        throw "Filename cannot be empty or whitespace."
    }

    $invalidChars = [System.IO.Path]::GetInvalidFileNameChars()
    if ($filename.IndexOfAny($invalidChars) -ne -1) {
        throw "Filename contains invalid characters."
    }

    $reservedNames = @("CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9")
    if ($reservedNames -contains $filename.ToUpper()) {
        throw "Filename is a reserved name."
    }
}
#endregion

#region multi
function downloadMulti([string]$version, [string]$output = "", [string]$name = "") {
    if ($output -eq "") {
        $output = getDownloadFolder
    }
    
    if (-not (Test-Path -Path $output)) {
        throw "Given folder path does not exist: '$output'"
    }

    if ($version -eq "") {
        throw "Must provide a value for version"
    }
    
    $url = "https://download.enova.pl/instalatory/net/Soneta.Standard.$($version).zip"
    
    $filename = if ($name -eq "") { $url.Split("/")[-1] } else { $name }
    Validate-Filename -filename $filename

    $path = Join-Path -Path $output -ChildPath $filename
    
    Invoke-WebRequest -Uri $url -OutFile $path

    return $path
}

function Multi {
    param(
        [Parameter(Mandatory = $false)]
        [ValidateSet("download", "help", "")] 

        [Parameter(Mandatory = $false)]
        [Alias("h")]
        [switch]$Help
    )

    if ($Help.IsPresent -or $Command -eq "help" -or $Command -eq "") {
        Write-Host "multi help"
        return
    }

    switch ($Command) {
        "download" {
            DownloadMulti @args
        }
        default {
            throw "Unknown command: $Command"
        }
    }
}
#endregion

#region explorer
function DownloadExplorer {
    if ($output -eq "") {
        $output = getDownloadFolder
    }
    
    if (-not (Test-Path -Path $output)) {
        throw "Given folder path does not exist: '$output'"
    }

    if ($version -eq "") {
        throw "Must provide a value for version"
    }

    $url = "https://download.enova.pl/instalatory/net/enova365_$($Version)_instalator.exe"
    
    $filename = if ($name -eq "") { $url.Split("/")[-1] } else { $name }
    validateFilename -filename $filename
    $path = Join-Path -Path $output -ChildPath $filename
    
    Invoke-WebRequest -Uri $url -OutFile $path

    return $path
}
#endregion

#region clean
#endregion

# main

switch ($Command) {
    "download" {        
        Invoke-Expression ".\download.ps1 $subCommand $remainingArgs"
    }
    default {
        Write-Host "Unknown command: $Command"
    }
}
