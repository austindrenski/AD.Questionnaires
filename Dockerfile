# build the app
FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish src/QuestionnairesAPI -c Release -o out

# build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT dotnet QuestionnairesAPI.dll