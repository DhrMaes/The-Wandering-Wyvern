FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY WanderingWyvern.slnx ./
COPY WanderingWyvern.Core/WanderingWyvern.Core.csproj WanderingWyvern.Core/
COPY WanderingWyvern.Web.Client/WanderingWyvern.Web.Client.csproj WanderingWyvern.Web.Client/
COPY WanderingWyvern.Web/WanderingWyvern.Web.csproj WanderingWyvern.Web/
RUN dotnet restore WanderingWyvern.Web/WanderingWyvern.Web.csproj

COPY . .
RUN dotnet publish WanderingWyvern.Web/WanderingWyvern.Web.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8090
EXPOSE 8090

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WanderingWyvern.Web.dll"]
