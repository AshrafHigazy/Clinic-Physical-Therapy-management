# -----------------------------
#       BUILD STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy all repo files
COPY . .

# Restore dependencies
RUN dotnet restore "Clinic-Management-System/Clinic-Management-System.csproj"

# Build and publish
RUN dotnet publish "Clinic-Management-System/Clinic-Management-System.csproj" -c Release -o /app/publish

# -----------------------------
#       RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Clinic-Management-System.dll"]
