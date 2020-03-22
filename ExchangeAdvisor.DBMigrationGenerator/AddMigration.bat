@ECHO OFF

cd.. && dotnet ef migrations add %1 --project ExchangeAdvisor.db --startup-project ExchangeAdvisor.DBMigrationGenerator