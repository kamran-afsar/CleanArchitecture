# Create Resource Group
az group create --name rg-clean-architecture --location westeurope

# Create App Service Plans
az appservice plan create --name asp-clean-architecture-dev --resource-group rg-clean-architecture --sku B1
az appservice plan create --name asp-clean-architecture-test --resource-group rg-clean-architecture --sku B1
az appservice plan create --name asp-clean-architecture-acc --resource-group rg-clean-architecture --sku B1
az appservice plan create --name asp-clean-architecture-prod --resource-group rg-clean-architecture --sku P1V2

# Create Web Apps
az webapp create --name clean-architecture-dev --resource-group rg-clean-architecture --plan asp-clean-architecture-dev
az webapp create --name clean-architecture-test --resource-group rg-clean-architecture --plan asp-clean-architecture-test
az webapp create --name clean-architecture-acc --resource-group rg-clean-architecture --plan asp-clean-architecture-acc
az webapp create --name clean-architecture-prod --resource-group rg-clean-architecture --plan asp-clean-architecture-prod 