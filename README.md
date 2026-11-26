# Web Kinh Doanh API

Backend cho cửa hàng Mộc Mây, tổ chức theo Clean Architecture trên .NET 8:

- `WebKinhDoanh.Domain`: entities và trạng thái đơn hàng.
- `WebKinhDoanh.Application`: DTO, validation, service và repository contracts.
- `WebKinhDoanh.Infrastructure`: EF Core 9, PostgreSQL, migrations, seed data và Dapper.
- `WebKinhDoanh.Api`: REST API, Swagger, CORS và xử lý lỗi thống nhất.

## Chạy local

Yêu cầu .NET 8 SDK và một PostgreSQL database (ví dụ Neon). Từ thư mục `web_kinh_doanh_api`:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=YOUR_NEON_HOST;Database=YOUR_DATABASE;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require"
dotnet run --project src\WebKinhDoanh.Api\WebKinhDoanh.Api.csproj
```

Mặc định API chạy tại `http://localhost:5181` và `https://localhost:7279`. Khi khởi động, ứng dụng áp dụng EF migrations và thêm danh mục/sản phẩm mẫu nếu catalog còn trống. Swagger chỉ bật trong môi trường Development.

Trong Neon, lấy host, database, user và password tại mục **Connect** của project; đặt connection string bằng biến môi trường như trên thay vì lưu secret vào source code. Không đưa connection string lên frontend.

Các SQL Server migrations cũ được lưu riêng trong `Sql\Migrations\SqlServer` và không được dùng cho PostgreSQL. Migration `InitialPostgres` đã được tạo sẵn. Sau khi đặt connection string Neon, áp dụng schema lên database:

```powershell
dotnet ef database update --project src\WebKinhDoanh.Infrastructure\WebKinhDoanh.Infrastructure.csproj --startup-project src\WebKinhDoanh.Api\WebKinhDoanh.Api.csproj
```

Cho phép frontend chạy ở `localhost:5173` hoặc `localhost:4173`. Muốn thêm origin, cập nhật `AllowedOrigins` trong appsettings.

## API

Tất cả phản hồi dùng envelope `{ success, message, data, errors }`.

| Method | Endpoint | Mô tả |
| --- | --- | --- |
| `GET` | `/api/health` | Kiểm tra API |
| `GET` | `/api/categories` | Danh mục và số sản phẩm |
| `GET` | `/api/products?search=&category=&featured=&page=1&pageSize=20` | Tìm kiếm, lọc và phân trang catalog |
| `GET` | `/api/products/{id}` | Chi tiết sản phẩm |
| `POST` | `/api/orders` | Tạo đơn; backend xác minh giá, kích cỡ và tồn kho trong transaction |
| `POST` | `/api/contact` | Gửi yêu cầu liên hệ |

Ví dụ đặt hàng:

```json
{
  "customerName": "Nguyen Van A",
  "phone": "0399999999",
  "email": "a@example.com",
  "address": "1 Nguyen Hue, Quan 1",
  "note": "",
  "items": [
    { "productId": 1, "quantity": 1 },
    { "productId": 6, "quantity": 1, "size": "M" }
  ]
}
```

Đơn hàng được nhận thành công trả HTTP `201` cùng `orderNumber` và `subtotal`. Dữ liệu đầu vào không hợp lệ trả `400`; sản phẩm thiếu tồn kho trả `409`.

## Migrations

EF Core migrations nằm trong `src\WebKinhDoanh.Infrastructure\Sql\Migrations`. Tạo migration mới từ thư mục backend:

```powershell
dotnet ef migrations add MigrationName --project src\WebKinhDoanh.Infrastructure\WebKinhDoanh.Infrastructure.csproj --startup-project src\WebKinhDoanh.Api\WebKinhDoanh.Api.csproj --output-dir Sql\Migrations
```
