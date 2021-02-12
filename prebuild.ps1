Write-Host "Patching Project with commit message..."

(Get-Content .\zxcvbn-core\zxcvbn-core.csproj).replace('$commitMsg', $Env:APPVEYOR_REPO_COMMIT_MESSAGE) | Set-Content .\zxcvbn-core\zxcvbn-core.csproj

