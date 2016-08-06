Write-Host "Patching project.json version..."
(Get-Content $PSScriptRoot\zxcvbn-core\project.ci.json).replace('$version$', $Env:APPVEYOR_BUILD_VERSION) | Set-Content $PSScriptRoot\zxcvbn-core\project.json
Write-Host "Updated project.json to use version $($Env:APPVEYOR_BUILD_VERSION)"

dotnet restore
