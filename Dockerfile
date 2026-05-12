# Frontend build
FROM node:20-alpine AS frontend
WORKDIR /app
COPY frontend/package.json ./
RUN npm install
COPY frontend/ ./
RUN npm run build

# Backend build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /app
COPY src/ErpEscolar.Api/*.csproj src/ErpEscolar.Api/
COPY src/ErpEscolar.Core/*.csproj src/ErpEscolar.Core/
COPY src/ErpEscolar.Infra/*.csproj src/ErpEscolar.Infra/
RUN dotnet restore src/ErpEscolar.Api/ErpEscolar.Api.csproj
COPY . .
RUN dotnet publish src/ErpEscolar.Api/ErpEscolar.Api.csproj -c Release -o /out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

COPY --from=backend /out .
COPY --from=frontend /app/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ErpEscolar.Api.dll"]
