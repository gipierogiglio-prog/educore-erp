FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Restore
COPY src/ErpEscolar.Core/ErpEscolar.Core.csproj src/ErpEscolar.Core/
COPY src/ErpEscolar.Infra/ErpEscolar.Infra.csproj src/ErpEscolar.Infra/
COPY src/ErpEscolar.Api/ErpEscolar.Api.csproj src/ErpEscolar.Api/
RUN dotnet restore src/ErpEscolar.Api/ErpEscolar.Api.csproj

# Build
COPY . .
RUN dotnet publish src/ErpEscolar.Api/ErpEscolar.Api.csproj -c Release -o /out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ErpEscolar.Api.dll"]
