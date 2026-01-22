FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln .
COPY src/*/*.csproj ./
RUN for f in *.csproj; do mkdir -p src/${f%.csproj} && mv $f src/${f%.csproj}/; done
RUN dotnet restore
COPY src/ src/
RUN dotnet publish src/SurveyPlatform.SurveyResponseService.Api -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SurveyPlatform.SurveyResponseService.Api.dll"]
