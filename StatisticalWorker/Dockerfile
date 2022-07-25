FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

COPY . .
WORKDIR StatisticalWorker
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=builder /app .
EXPOSE 30202

ENTRYPOINT ["dotnet", "StatisticalWorker.dll"]
