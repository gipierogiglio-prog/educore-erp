FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /app

# Create directory structure and copy project files
COPY src/ErpEscolar.Api/ErpEscolar.Api.csproj /app/src/ErpEscolar.Api/
COPY src/ErpEscolar.Core/ErpEscolar.Core.csproj /app/src/ErpEscolar.Core/
COPY src/ErpEscolar.Infra/ErpEscolar.Infra.csproj /app/src/ErpEscolar.Infra/
RUN dotnet restore /app/src/ErpEscolar.Api/ErpEscolar.Api.csproj

# Copy everything else and build
COPY . /app/
RUN dotnet publish /app/src/ErpEscolar.Api/ErpEscolar.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=backend /out .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ErpEscolar.Api.dll"]
