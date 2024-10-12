# Quick packing to see the result. Change the version if needed.

Write-Host "Packing the project..." -ForegroundColor Cyan
dotnet pack

Write-Host "Uninstalling the old template 'BlazorStatic.Templates'..." -ForegroundColor Cyan
dotnet new uninstall BlazorStatic.Templates

Write-Host "Installing the new template" -ForegroundColor Cyan
dotnet new install --force "bin/Release/BlazorStatic.Templates.1.0.2.nupkg"

Write-Host "Removing the old 'TestProject' directory..." -ForegroundColor Cyan
Remove-Item -Recurse -Force -Path ".\TestProject"

Write-Host "Creating a new project from the 'BlazorStaticMinimalBlog' template..." -ForegroundColor Cyan
dotnet new BlazorStaticMinimalBlog -o "TestProject" --force -e
