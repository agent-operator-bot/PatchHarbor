FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish PatchHarbor.Web/PatchHarbor.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
VOLUME ["/app/PatchHarbor.Web/data"]
EXPOSE 8080
ENTRYPOINT ["dotnet", "PatchHarbor.Web.dll"]
