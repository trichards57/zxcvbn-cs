mkdir tools
mkdir reports\coverage\history

nuget install OpenCover -ExcludeVersion -OutputDirectory  tools
nuget install xunit.runner.console -ExcludeVersion -OutputDirectory tools
nuget install ReportGenerator -ExcludeVersion -OutputDirectory tools

dotnet build

tools\OpenCover\tools\OpenCover.Console.exe -target:"tools\xunit.runner.console\tools\xunit.console.x86.exe" -targetargs:"zxcvbn-core-test\bin\Debug\net462\win7-x86\zxcvbn-core-test.exe -noShadow -xml test-results.xml" -register:user -output:"reports\coverage\coverage.xml" -skipautoprops -filter:"+[zxcvbn-core*]* -[zxcvbn-core-test]*"  -excludebyattribute:*.ExcludeFromCodeCoverage* -mergebyhash -returntargetcode

tools\ReportGenerator\tools\ReportGenerator.exe -reports:reports\coverage\coverage.xml -targetdir:reports\coverage -historydir:reports\coverage\history

