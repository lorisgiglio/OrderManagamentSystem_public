cd .\Api\OrderManagement.Api.Addresses\
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ..\..
cd .\Api\OrderManagement.Api.Categories\
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ..\..
cd .\Api\OrderManagement.Api.Orders\
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ..\..
cd .\Api\OrderManagement.Api.Product\
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ..\..
cd .\Api\OrderManagement.Api.Users\
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ..\..
