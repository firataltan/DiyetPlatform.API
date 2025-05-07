# Use the official .NET 8.0 SDK image as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["DiyetPlatform.API/DiyetPlatform.API.csproj", "DiyetPlatform.API/"]
RUN dotnet restore "DiyetPlatform.API/DiyetPlatform.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "DiyetPlatform.API/DiyetPlatform.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DiyetPlatform.API/DiyetPlatform.API.csproj" -c Release -o /app/publish

# Use the official .NET 8.0 runtime image as the base image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 80
EXPOSE 443

# Set the entry point for the container
ENTRYPOINT ["dotnet", "DiyetPlatform.API.dll"] 