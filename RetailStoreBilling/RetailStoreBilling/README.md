# Abhishek Pandit

# 

# RetailStoreBilling

A starter retail billing application built with **Blazor Web App (.NET 8, Interactive Server)**, **Entity Framework Core 8**, and **SQL Server**.

## Features

* Dashboard with product, low-stock, and sales summaries.
* Product CRUD: create, list/search, edit, and delete products.
* Checkout: add products to a cart, change quantities, calculate subtotal/tax/total, choose payment method, and save a bill.
* SQL Server persistence for products, sales, and sale line items.
* Stock is reduced when a sale is recorded.
* Sales history and void operation; voiding a bill restores stock.
* Basic validation for prices, stock, quantity, and tax rate.

## Prerequisites

* .NET 8 SDK
* SQL Server LocalDB (included with many Visual Studio installations), SQL Server Express, or SQL Server
* Visual Studio 2022 or another .NET-capable editor

## Configure SQL Server

The default connection string in `appsettings.json` is for Windows LocalDB:

```json
"RetailDb": "Server=(localdb)\\\\MSSQLLocalDB;Database=RetailStoreBillingDb;Trusted\_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Change it to match your SQL Server instance if needed. For example, SQL Server Express might use `Server=.\\\\SQLEXPRESS;Database=RetailStoreBillingDb;Trusted\_Connection=True;TrustServerCertificate=True`.

## Create the database

From the solution folder:

```bash
dotnet tool install --global dotnet-ef --version 8.\*
dotnet restore
dotnet ef migrations add InitialCreate --project RetailStoreBilling/RetailStoreBilling.csproj
dotnet ef database update --project RetailStoreBilling/RetailStoreBilling.csproj
dotnet run --project RetailStoreBilling/RetailStoreBilling.csproj
```

Open the HTTPS URL printed in the console (or `https://localhost:7178` if the launch profile is used).

> If `dotnet-ef` is already installed, update it to version 8 if needed. Migrations create the database schema; the application does not automatically create a database at startup.

## Main pages

* `/` — Dashboard
* `/products` — Product CRUD and inventory
* `/billing` — New bill / checkout
* `/sales` — Sales history and void

## Project structure

```text
RetailStoreBilling/
  Components/
    Layout/MainLayout.razor
    Pages/Home.razor
    Pages/Products.razor
    Pages/Billing.razor
    Pages/Sales.razor
    App.razor
    Routes.razor
  Data/RetailDbContext.cs
  Models/Product.cs
  Models/Sale.cs
  Models/SaleItem.cs
  Services/ProductService.cs
  Services/BillingService.cs
  Program.cs
  appsettings.json
```

## Important production notes

This is a learning/starter project, not a production-ready point-of-sale system. Before production use, add authentication and authorization, audit logs instead of hard-deleting/voiding transactions, concurrency handling for inventory, receipt printing, refunds/returns, configurable tax rules, payment-provider integration, backups, and automated tests. The sample does not process card payments; it only records the selected payment method. Protect the connection string with user secrets/environment variables in deployed environments.

