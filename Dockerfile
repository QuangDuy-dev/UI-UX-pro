# syntax=docker/dockerfile:1

###############################################################################
# STAGE 1 — Build
###############################################################################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj và restore dependencies (layer cache tối ưu)
COPY "UI-UX pro.csproj" ./
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

# Mặc định Production (bỏ development hardening, dùng HSTS/Error handler)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5165

# Copy kết quả publish
COPY --from=build /app/publish .

# Cấu hình nhạy cảm (AI key, admin password...) nên truyền qua biến môi trường
# khi chạy container, KHÔNG build cứng vào image. Ví dụ:
#   -e ConnectionStrings__MongoDb='...' \
#   -e AdminSettings__MasterPassword='...' \
#   -e AspNetCore__...  (nếu muốn override)
# admin-config.json (cấu hình AI/automation do user chỉnh ở Settings) nên là
# volume gắn ngoài (thư mục config) thay vì build sẵn, xem README phần chạy.

USER $APP_UID
ENTRYPOINT ["dotnet", "UI-UX pro.dll"]
