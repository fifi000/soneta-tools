param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("multi", "explorer", "help", "")]
    [string]$command = "help",
    
    [Parameter(Mandatory = $false)]
    [Alias("v")]
    [string]$version,

    [Parameter(Mandatory = $false)]
    [Alias("o")]
    [string]$output,

    [Parameter(Mandatory = $false)]
    [Alias("n")]
    [string]$name,

    [Parameter(Mandatory = $false)]
    [Alias("h")]
    [switch]$help
)

. "$PSScriptRoot\utils.ps1"

# 
# help menu
# 

if ($help.IsPresent -or $command -eq "help" -or $command -eq "") {
    return 'download help'
}

# 
# functions
# 

# 
# command switch
# 

switch ($command) {
    "multi" {
        return Get-Multi
    }
    "explorer" {
        return Get-Explorer
    }
    default {
        throw "Unknown command: '$command'"
    }
}