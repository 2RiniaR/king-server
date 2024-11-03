FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 8.0.10
RUN dotnet ef migrations bundle -o out/efbundle --self-contained -r linux-x64
RUN chmod 755 out/efbundle

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "king-server.dll"]