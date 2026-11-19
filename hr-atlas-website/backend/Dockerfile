# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["HrAtlas.SaaS.csproj", "./"]
RUN dotnet restore "./HrAtlas.SaaS.csproj"
COPY . .
RUN dotnet publish "HrAtlas.SaaS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS="http://+:5000"
ENTRYPOINT ["dotnet", "HrAtlas.SaaS.dll"]

