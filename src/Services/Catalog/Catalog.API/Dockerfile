#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#=====================================================================================================================================


# Build Section
#===================
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
# below we copy from local directory to the docker directory
COPY ["Services/Catalog/Catalog.API/Catalog.API.csproj", "Services/Catalog/Catalog.API/"]
RUN dotnet restore "Services/Catalog/Catalog.API/Catalog.API.csproj"
COPY . .
WORKDIR "/src/Services/Catalog/Catalog.API"
RUN dotnet build "Catalog.API.csproj" -c Release -o /app/build

# Publish Section with name: publish
#===============
FROM build AS publish
RUN dotnet publish "Catalog.API.csproj" -c Release -o /app/publish

 # Run Section
 #+============
FROM base AS final
WORKDIR /app
# copy from publish image to run area
COPY --from=publish /app/publish .
# use dot net to run this .dll
# at start, the Program.cs file is used as the entry point and there, the host is set-up
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
