# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy all files
COPY . .

# Restore + Build + Publish
RUN dotnet restore ./Clinic-Management-System/Clinic-Management-System.csproj
RUN dotnet publish ./Clinic-Management-System/Clinic-Management-System.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "Clinic-Management-System.dll"]
