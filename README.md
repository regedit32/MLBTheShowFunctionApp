# MLB The Show Sample Azure Function App

This project is a demonstration on how to use a Azure Functions, written in C#, to consume an API and write the results to a database.  In this
solution, we're consuming the [MLB The Show 22 API](https://mlb22.theshow.com/apis/docs) and using Azure Cosmos DB to store the data.

## Functions

- **ProcessLiveSeriesValue:** Using a timer schedule, this function will retrieve the value of the Live Series Collection by team, division, league, and total collection.  The data pulled from the MLB The Show API is point-in-time data, so the goal of this function is to retrieve the collection value over time in order to do trend analysis with the data later.
- **GetLiveSeriesValue:** HTTP trigger function that will return the current value of the Live Series Collection by team, division, league, and total collection.
- **InitMetadata:** HTTP trigger function that will initialize a database and two containers. The LeagueMetadata container stores the league information (team names and their related division name/league name).  The SeriesMetadata container stores the series names/IDs metadata from The Show API.  No current use case for this other than demo purposes.

## Application Settings

The following app settings need to be configured in order for the function to work.  For information on configuring these settings for a local development environment, see [Code and test Azure Functions locally](https://docs.microsoft.com/azure/azure-functions/functions-develop-local#local-settings-file).  For information on configuring these settings in the Azure portal, see [Work with application settings](https://docs.microsoft.com/azure/azure-functions/functions-how-to-use-azure-function-app-settings?tabs=portal#settings).

- **DatabaseName:** Name of the database used to write data.
- **EndPointUri:** The URI fo the Cosmos DB endpoint.
- **PrimaryKey:** Key used to grant the Function access to the Cosmos DB account.

## Resources

- **[Quickstart: Create a C# function in Azure using Visual Studio Code](https://docs.microsoft.com/azure/azure-functions/create-first-function-vs-code-csharp)**

- **[Quickstart: Create an Azure Cosmos account, database, container, and items from the Azure portal](https://docs.microsoft.com/azure/cosmos-db/sql/create-cosmosdb-resources-portal)**

- **[MLB 22 API Documentation](https://mlb22.theshow.com/apis/docs/)**

## Work in progress

- Add more tests
- Add more logging
- Implement Cosmos DB repository
