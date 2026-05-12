FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /app
COPY src/ErpEscolar.Api/*.csproj src/ErpEscolar.Api/
COPY src/ErpEscolar.Core/*.csproj src/ErpEscolar.Core/
COPY src/ErpEscolar.Infra/*.csproj src/ErpEscolar.Infra/
RUN dotnet restore src/ErpEscolar.Api/ErpEscolar.Api.csproj
COPY . .
RUN dotnet publish src/ErpEscolar.Api/ErpEscolar.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=backend /out .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ErpEscolar.Api.dll"]
