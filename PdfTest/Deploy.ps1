dotnet build .\PdfTest.csproj --configuration Release
$zip = '.\bin\pdf-test.zip'
Compress-Archive -Path .\bin\Release\netcoreapp2.1\* -DestinationPath $zip -Force
az functionapp deployment source config-zip --resource-group 'dev-au01-local-apollo-sss-rg' --name 'fn-dev-sss-pdf-debug' --src $zip