# Použije oficiální .NET runtime obraz jako základ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Použije oficiální .NET SDK obraz pro sestavení aplikace
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderEventProcessor/OrderEventProcessor.csproj", "OrderEventProcessor/"]
RUN dotnet restore "OrderEventProcessor/OrderEventProcessor.csproj"
COPY . .
WORKDIR "/src/OrderEventProcessor"
RUN dotnet build "OrderEventProcessor.csproj" -c Release -o /app/build

# Publikuje aplikaci
FROM build AS publish
RUN dotnet publish "OrderEventProcessor.csproj" -c Release -o /app/publish

# Použije runtime obraz pro finální fázi
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderEventProcessor.dll"]
