FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Flekt.Wpf.Shared/Flekt.Wpf.Shared.csproj Flekt.Wpf.Shared/
COPY src/Flekt.Wpf.Api/Flekt.Wpf.Api.csproj Flekt.Wpf.Api/
RUN dotnet restore Flekt.Wpf.Api/Flekt.Wpf.Api.csproj
COPY src/Flekt.Wpf.Shared/ Flekt.Wpf.Shared/
COPY src/Flekt.Wpf.Api/ Flekt.Wpf.Api/
RUN dotnet publish Flekt.Wpf.Api/Flekt.Wpf.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Flekt.Wpf.Api.dll"]
