function CreateEnovaProject {
    param(
        [Parameter(Mandatory = $false)]
        [Alias("n")]
        [string]$Name,
        
        [Parameter(Mandatory = $false)]
        [Alias("n")]

    )
    dotnet new sln
}