﻿param(
    [Parameter(Mandatory)]
    [ValidateSet('Debug','Release')]
    [System.String]$Target,
    
    [Parameter(Mandatory)]
    [System.String]$TargetPath,
    
    [Parameter(Mandatory)]
    [System.String]$TargetAssembly,

    [Parameter(Mandatory)]
    [System.String]$GamePath,
    
    [Parameter(Mandatory)]
    [System.String]$ProjectPath,
    
    [System.String]$DeployPath
)

# Make sure Get-Location is the script path
Push-Location -Path (Split-Path -Parent $MyInvocation.MyCommand.Path)

# Test some preliminaries
("$TargetPath",
 "$GamePath",
 "$ProjectPath"
) | % {
    if (!(Test-Path "$_")) {Write-Error -ErrorAction Stop -Message "$_ folder is missing"}
}

# Go
Write-Host "Publishing for $Target from $TargetPath"

# Plugin name without ".dll"
$name = "$TargetAssembly" -Replace('.dll')

# Debug copies the dll to Valheim
if ($Target.Equals("Debug")) {
    if ($DeployPath.Equals("")){
      $DeployPath = "$GamePath\BepInEx\plugins"
    }
    
    $plug = New-Item -Type Directory -Path "$DeployPath\$name" -Force
    Write-Host "Copy $TargetAssembly to $plug"
    Copy-Item -Path "$TargetPath\$name.dll" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.pdb" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.xml" -Destination "$plug" -Force -ErrorAction SilentlyContinue
    #Copy-Item -Path "$TargetPath\$name.dll.mdb" -Destination "$plug" -Force
}

# Release builds packages for ThunderStore and NexusMods
if($Target.Equals("Release")) {
    $package = "$ProjectPath\_package"
    [xml]$versionxml = Get-Content -Path "$ProjectPath\BuildProps\version.props"
    $version = $versionxml.Project.PropertyGroup.Version
    
    Write-Host "Packaging for ThunderStore"
    New-Item -Type Directory -Path "$package\Thunderstore" -Force
    $thunder = New-Item -Type Directory -Path "$package\Thunderstore\package"
    $thunder.CreateSubdirectory('plugins')
    Copy-Item -Path "$TargetPath\$name.dll" -Destination "$thunder\plugins\"
    Copy-Item -Path "$TargetPath\$name.pdb" -Destination "$thunder\plugins\"
    Copy-Item -Path "$TargetPath\$name.xml" -Destination "$thunder\plugins\"
    Copy-Item -Path "$TargetPath\$name.dll.mdb" -Destination "$thunder\plugins\"
    Copy-Item -Path "$ProjectPath\..\README.md" -Destination "$thunder\README.md"
    Copy-Item -Path "$ProjectPath\manifest.json" -Destination "$thunder\manifest.json"
    Remove-Item -Path "$package\Thunderstore\$name-$version.zip" -Force
    Copy-Item -Path "$(Get-Location)\resources\JVL_Logo_256x256.png" -Destination "$thunder\icon.png"
    Invoke-Expression "& `"$(Get-Location)\libraries\7za.exe`" a `"$package\Thunderstore\$name-$version.zip`" `"$thunder\*`""
    $thunder.Delete($true)

    Write-Host "Packaging for NexusMods"
    New-Item -Type Directory -Path "$package\Nexusmods" -Force
    $nexus = New-Item -Type Directory -Path "$package\Nexusmods\package"
    Copy-Item -Path "$TargetPath\$name.dll" -Destination "$nexus\"
    Copy-Item -Path "$TargetPath\$name.pdb" -Destination "$nexus\"
    Copy-Item -Path "$TargetPath\$name.xml" -Destination "$nexus\"
    Copy-Item -Path "$TargetPath\$name.dll.mdb" -Destination "$nexus\"
    Copy-Item -Path "$ProjectPath\..\README.md" -Destination "$nexus\README"
    Remove-Item -Path "$package\Nexusmods\$name-$version.zip" -Force
    Invoke-Expression "& `"$(Get-Location)\libraries\7za.exe`" a `"$package\Nexusmods\$name-$version.zip`" `"$nexus\*`""
    $nexus.Delete($true)
}


# Pop Location
Pop-Location