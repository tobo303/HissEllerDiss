FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /backend

COPY ./HissEllerDissApi .

RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0.8-alpine3.20
# Change our working directory to where source files are located
WORKDIR /backend
COPY --from=build-env /backend/out .
ENV HTTP_PORTS=5210
ENV RABBITMQ_HOST=rabbitmq
CMD [ "dotnet", "HissEllerDissApi.dll" ]