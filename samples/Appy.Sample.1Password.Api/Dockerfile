FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /
COPY ["samples/Appy.Sample.1Password.Api/Appy.Sample.1Password.Api.csproj", "samples/Appy.Sample.1Password.Api/"]
COPY ["src/Appy.Configuration.1Password/Appy.Configuration.1Password.csproj", "src/Appy.Configuration.1Password/"]
COPY ["src/Appy.Configuration/Appy.Configuration.csproj", "src/Appy.Configuration/"]
COPY ["src/Appy.Infrastructure.1Password/Appy.Infrastructure.1Password.csproj", "src/Appy.Infrastructure.1Password/"]
RUN dotnet restore "samples/Appy.Sample.1Password.Api/Appy.Sample.1Password.Api.csproj"
COPY . .
WORKDIR "/samples/Appy.Sample.1Password.Api"
RUN dotnet build "Appy.Sample.1Password.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Appy.Sample.1Password.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Appy.Sample.1Password.Api.dll"]
