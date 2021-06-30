FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
COPY . .
RUN dotnet tool restore
RUN dotnet restore
RUN dotnet publish score-api.sln -c release -o ./app --no-cache

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine as publish
COPY --from=build /app .
ENTRYPOINT ["dotnet", "score-api.dll"]