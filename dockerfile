# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0.201-jammy AS build
WORKDIR /app

# Copy the remaining source code and build the application
COPY . ./
RUN dotnet build -c Release "./Api/Api.sln"

# Publish the application to a publish folder
RUN dotnet publish -c Release -o /app/publish "./Api/Api.sln"

# Use the official ASP.NET Core Runtime image as the runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0.201-jammy AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port 80 for HTTP traffic
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "Api.dll"]