# upload results to Coveralls

 $result = Get-ChildItem -Path . -Filter coverage.cobertura.xml -Recurse |%{$_.FullName}
 .\tools\coveralls.io\tools\coveralls.net.exe --autodetect $result