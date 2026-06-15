FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/Members.WebApi/Members.WebApi.csproj
RUN dotnet build src/Members.WebApi/Members.WebApi.csproj -c Release --no-restore
RUN dotnet publish src/Members.WebApi/Members.WebApi.csproj -c Release --no-build -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Members.WebApi.dll"]
