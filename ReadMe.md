# Azure AD Data Analyzer
Provides tools for analyzing resources in Azure Active Directory

# Requirements
* .NET Core 3.1 or greater
* Visual Studio 2019 or greater

# Projects
## Libraries
|Project|Type|Purpose|
|---|---|---|
|azuread-data-analyzer|.NET Standard Class Library|Core library for the application|

## Applications
|Project|Type|Purpose|
|---|---|---|
|azuread-data-analyzer.console|Console Application|Console application to run the commands|

# Development Environment Setup
To be able to run the console application, an appSettings.json file needs to be created at the root of the azuread-data-analyzer.console project with the following settings:
```json
{
  "CosmosDocumentKey": "<Key to the Cosmos DB used to store data>",
  "CosmosUrl": "<Url to the Cosmos DB used to store data>",

  "SqlConnectionString": "<Connection string to the SQL Server database used to store data>",

  "AzureAdTenantId": "<Tenant Id for the Azure AD instance (tenant) to analyze data on>",
  "AzureAdClientId": "<Client Id used to authenticate against the Azure AD instance for reading data>",
  "AzureAdClientSecret": "<Secret for the Client referenced above>"
}
```