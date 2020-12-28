# #Build
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["BecamexIDC.Authentication/BecamexIDC.Authentication.csproj", "BecamexIDC.Authentication/"]
RUN dotnet restore "BecamexIDC.Authentication/BecamexIDC.Authentication.csproj" 
COPY . .
WORKDIR "/src/BecamexIDC.Authentication"
RUN dotnet build "BecamexIDC.Authentication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BecamexIDC.Authentication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BecamexIDC.Authentication.dll"]