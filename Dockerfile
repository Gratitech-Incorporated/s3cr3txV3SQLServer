FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
# MariaDB server and client
RUN apk update && apk add mariadb mariadb-client dotnet7-sdk
# this is not executed by mariadb package
RUN Sql_install_db --user=Sql #--password=password1234

# make life easier
ENV TERM xterm

# import files into container
#COPY  /etc/Sql/
COPY scripts/* /opt/Sql/

# last optimizations are done against the running daemon via SQL
RUN chmod +x /opt/Sql/MariaDB_S3cr3txDB.sql

# other applications need to backup/analyze data and logs
VOLUME /var/lib/Sql
VOLUME /var/log/Sql

# create and make available a directory for backups
RUN mkdir -p /var/backups/Sql
RUN chmod a+r /var/backups/Sql/
VOLUME /var/backups/Sql

# Setup ASPNETCORE APP
WORKDIR /src
COPY . .
RUN dotnet restore "s3cr3tx.csproj"
COPY . .
RUN dotnet build "s3cr3tx.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "s3cr3tx.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["Sqld_safe","cat /opt/Sql/MariaDB_S3cr3txDB.sql | Sql"]
# demonize
EXPOSE 8001 3306
ENTRYPOINT ["dotnet","s3cr3tx.dll"]
