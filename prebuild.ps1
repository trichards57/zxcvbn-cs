Write-Host "Patching Project version..."

(Get-Content .\zxcvbn-core\zxcvbn-core.csproj).replace('<Version>1.0.0</Version>', "<Version>$($Env:APPVEYOR_BUILD_VERSION)</Version>") | Set-Content .\zxcvbn-core\zxcvbn-core.csproj
(Get-Content .\zxcvbn-core\zxcvbn-core.csproj).replace('<AssemblyVersion>1.0.0</AssemblyVersion>', "<AssemblyVersion>$($Env:APPVEYOR_BUILD_VERSION)</AssemblyVersion>") | Set-Content .\zxcvbn-core\zxcvbn-core.csproj
(Get-Content .\zxcvbn-core\zxcvbn-core.csproj).replace('<FileVersion>1.0.0</FileVersion>', "<FileVersion>$($Env:APPVEYOR_BUILD_VERSION)</FileVersion>") | Set-Content .\zxcvbn-core\zxcvbn-core.csproj
(Get-Content .\zxcvbn-core\zxcvbn-core.csproj).replace('$commitMsg', $Env:APPVEYOR_REPO_COMMIT_MESSAGE) | Set-Content .\zxcvbn-core\zxcvbn-core.csproj

Write-Host "Updated Project to use version $($Env:APPVEYOR_BUILD_VERSION)"

