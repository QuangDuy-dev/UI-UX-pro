# syntax=docker/dockerfile:1

###############################################################################
# STAGE 1 — Build
###############################################################################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj và restore dependencies (dùng mảng JSON để hỗ trợ tên file có khoảng trắng)
COPY ["UI-UX pro.csproj", "./"]
RUN dotnet restore "UI-UX pro.csproj"

# Copy toàn bộ source còn lại và build theo Release
COPY . .
RUN dotnet publish "UI-UX pro.csproj" -c Release -o /app/publish --no-restore

###############################################################################
# STAGE 2 — Runtime (image gọn, chỉ có ASP.NET Core runtime)
###############################################################################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5165

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5165

# Copy kết quả publish
COPY --from=build /app/publish .

USER $APP_UID
ENTRYPOINT ["dotnet", "UI-UX pro.dll"]