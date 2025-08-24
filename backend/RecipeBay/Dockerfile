# -------------------
# BUILD STAGE
# -------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore dependencies
COPY RecipeBay.csproj .
RUN dotnet restore

# copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app

# -------------------
# RUNTIME STAGE
# -------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app ./

# run the application
ENTRYPOINT ["dotnet", "RecipeBay.dll"]

# expose port for API
EXPOSE 5000
