# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:3fcf6f1e809c0553f9feb222369f58749af314af6f063f389cbd2f913b4ad556 AS build
WORKDIR /src

# Copy only solution + csproj files first (to maximize restore caching)
COPY VermundoBackend.sln ./
COPY src/Vermundo.Api/Vermundo.Api.csproj src/Vermundo.Api/
COPY src/Vermundo.Application/Vermundo.Application.csproj src/Vermundo.Application/
COPY src/Vermundo.Domain/Vermundo.Domain.csproj src/Vermundo.Domain/
COPY src/Vermundo.Infrastructure/Vermundo.Infrastructure.csproj src/Vermundo.Infrastructure/

# Restore now that project graph is known
RUN dotnet restore src/Vermundo.Api/Vermundo.Api.csproj

# Now copy the rest of the source
COPY . .

# Publish the API (avoid hardcoding TFM by using -o)
WORKDIR /src/src/Vermundo.Api
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:b4bea3a52a0a77317fa93c5bbdb076623f81e3e2f201078d89914da71318b5d8 AS final
WORKDIR /app

# Optional: set the port binding used by Kestrel inside the container
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Vermundo.Api.dll"]
