
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build



WORKDIR /src

#generate the cert, define the path to store it and password to use
#RUN dotnet dev-certs https -ep /Storage/https/aspnetapp.pfx -p yabo2024

COPY ["dotnetbase.csproj", "./"]
RUN dotnet restore "dotnetbase.csproj"
COPY . .
RUN dotnet build "dotnetbase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnetbase.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy the certificate
#COPY Storage/https/aspnetapp.pfx /app/Storage/https/
#COPY Storage/yabotest-b1e2dd0f6cd8.json /app/Storage/yabotest-b1e2dd0f6cd8.json


# ENV ASPNETCORE_URLS="http://0.0.0.0:5000"
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV DB_CONNECTION=mysql
ENV DB_HOST=db
ENV DB_PORT=3306
ENV DB_DATABASE=yabocleandb
ENV DB_USERNAME=root
ENV DB_PASSWORD=qwerty123
ENV IMAGE_STORAGE_PATH="Storage/Images/"
ENV LOG_CHANNEL=file
ENV LOG_LEVEL=information
ENV MAIL_MAILER=file
ENV MAIL_PORT=25
ENV MAIL_FROM_ADDRESS="hello@yabo.com"
ENV MAIL_FROM_NAME="Yabo Admin"
ENV ENVUSER_ROLE_ID="RegisteredUser"
ENV RESET_PASSWORD_LINK="http://5.161.118.86/reset-password"
ENV EMAIL_CONFIRMATION_LINK="http://5.161.118.86/confirm-email"
ENV DEFAULT_PAGE_SIZE=10
ENV DEFAULT_PAGE_NUMBER=1
ENV NEW_ORDERSTATUS_ID: 1
ENV CANCEL_ORDERSTATUS_ID: 3
ENV PAID_ORDERSTATUS_ID: 2
ENV PICKED_ORDERSTATUS_ID: 4
ENV SERVICEINPROGRESS_ORDERSTATUS_ID: 5
ENV SERVICECOMPLETE_ORDERSTATUS_ID: 6
ENV CUSTOMERCONFIRMED_ORDERSTATUS_ID: 7
ENV REG_STATUS_ID=1

# This volume is virtual mount for application
# VOLUME ["/app/Storage/Logging"]
# VOLUME ["/app/Storage/Mail"]
# VOLUME ["/app/Storage/Images"]
# VOLUME ["/app/Storage/"]

ENV ASPNETCORE_URLS=http://+:5000
#ENV ASPNETCORE_URLS="https://+:5001;http://+:5000"
#ENV ASPNETCORE_HTTPS_PORT=5001
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password="yabo2024"
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/Storage/https/aspnetapp.pfx"
EXPOSE 5000
# EXPOSE 5001
# Set environment variables.
# 

ENTRYPOINT ["dotnet", "dotnetbase.dll"]