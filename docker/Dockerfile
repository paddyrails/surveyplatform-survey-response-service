FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore and publish the API project
RUN dotnet restore src/SurveyPlatform.SurveyResponseService.Api/SurveyPlatform.SurveyResponseService.Api.csproj
RUN dotnet publish src/SurveyPlatform.SurveyResponseService.Api/SurveyPlatform.SurveyResponseService.Api.csproj -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SurveyPlatform.SurveyResponseService.Api.dll"]
