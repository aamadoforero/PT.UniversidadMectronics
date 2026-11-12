FROM mcr.microsoft.com/dotnet/sdk:10.0 AS compilacion
ARG PROYECTO
WORKDIR /src
COPY . .
RUN dotnet publish "$PROYECTO" -c Release -o /app --nologo

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=compilacion /app .
