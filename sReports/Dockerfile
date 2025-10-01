# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.sln ./
# Copy all csproj files
COPY */*.csproj ./
# Place all csproj files in propriate folders
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
# Directly copy CTCAE.csproj since it is not affected by script from above
COPY CTCAE/CTCAE/CTCAE.csproj ./CTCAE/CTCAE/CTCAE.csproj 
RUN dotnet restore sReports.sln
COPY . .
WORKDIR "/src/."
RUN dotnet build "./sReportsV2/sReportsV2.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./sReportsV2/sReportsV2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sReportsV2.dll"]