using Microsoft.EntityFrameworkCore;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        StoreDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
        if (await dbContext.Categories.AnyAsync(cancellationToken))
        {
            return;
        }

        var cosmetics = new Category
        {
            Name = "Mỹ phẩm",
            Image = "photo-1596462502278-27bfdc403348",
            DisplayOrder = 1
        };
        var clothing = new Category
        {
            Name = "Quần áo nữ",
            Image = "photo-1483985988355-763728e1935b",
            DisplayOrder = 2
        };

        dbContext.Categories.AddRange(cosmetics, clothing);
        dbContext.Products.AddRange(
            Product("Serum Dưỡng Ẩm Hyaluronic MMTT", "serum-duong-am-hyaluronic-mmtt", cosmetics, 299000, 390000, 120, "photo-1608248543803-ba4f8c70ae0b", "Bán chạy", "Serum cấp ẩm chuyên sâu giúp làn da mềm mại, căng mướt và tươi sáng. Kết cấu mỏng nhẹ, thấm nhanh, phù hợp cho chu trình chăm sóc da hằng ngày.", "30ml · Mọi loại da · Xuất xứ: Hàn Quốc", 24, true),
            Product("Kem Chống Nắng Daily UV SPF50+", "kem-chong-nang-daily-uv-spf50", cosmetics, 259000, 320000, 98, "photo-1556229010-6c3f2c9ca5f8", "Bán chạy", "Kem chống nắng phổ rộng SPF50+ bảo vệ da trước tia UVA và UVB. Chất kem nhẹ, dễ tán, phù hợp sử dụng mỗi ngày.", "50ml · SPF50+ PA++++ · Mọi loại da", 18, true),
            Product("Sữa Rửa Mặt Dịu Nhẹ Rau Má", "sua-rua-mat-diu-nhe-rau-ma", cosmetics, 189000, 230000, 76, "photo-1556228720-195a672e8a03", "Mới về", "Làm sạch bụi bẩn và dầu thừa một cách dịu nhẹ, không gây cảm giác khô căng. Chiết xuất rau má giúp làn da luôn dễ chịu sau khi rửa.", "120ml · Da nhạy cảm · Dùng sáng và tối", 32, true),
            Product("Son Tint Lì Mịn Môi", "son-tint-li-min-moi", cosmetics, 179000, 220000, 145, "photo-1586495777744-4413f21062fa", "Bán chạy", "Son tint lên màu chuẩn với lớp finish mịn nhẹ, dễ tán và thoải mái suốt ngày dài. Bảng màu trẻ trung phù hợp nhiều phong cách.", "4g · 6 màu thời thượng · Lâu trôi", 41, true),
            Product("Kem Dưỡng Ẩm Phục Hồi Da", "kem-duong-am-phuc-hoi-da", cosmetics, 329000, 399000, 64, "photo-1571781926291-c477ebfd024b", "Giảm giá", "Kem dưỡng ẩm giúp củng cố hàng rào bảo vệ da và duy trì độ ẩm mềm mịn. Kết cấu êm dịu, thích hợp dùng trong quy trình dưỡng da buổi tối.", "50g · Phục hồi và dưỡng ẩm · Mọi loại da", 15, false),
            Product("Áo Thun Nữ Cotton Basic", "ao-thun-nu-cotton-basic", clothing, 149000, 199000, 89, "photo-1483985988355-763728e1935b", "Bán chạy", "Áo thun nữ phom basic dễ phối đồ, phù hợp đi học, đi làm hay dạo phố. Chất vải cotton mềm mại, thoáng mát và thoải mái.", "Cotton mềm · Màu trắng, đen, be · Phom regular", 20, false),
            Product("Đầm Midi Hoa Nhí Dịu Dàng", "dam-midi-hoa-nhi-diu-dang", clothing, 369000, 459000, 76, "photo-1595777457583-95e059d581b8", "Giảm giá", "Đầm midi họa tiết hoa nhí với thiết kế nữ tính, nhẹ nhàng. Dáng váy bay, tôn vẻ thanh lịch trong những buổi hẹn và dịp dạo phố.", "Vải voan mềm · Họa tiết hoa · Dáng midi", 12, false),
            Product("Áo Sơ Mi Nữ Thanh Lịch", "ao-so-mi-nu-thanh-lich", clothing, 289000, 350000, 58, "photo-1539109136881-3be0616acf4b", "Mới về", "Áo sơ mi nữ tối giản với đường cắt tinh tế, dễ kết hợp cùng quần âu hoặc chân váy. Lựa chọn thanh lịch cho tủ đồ công sở.", "Vải mềm ít nhăn · Trắng, xanh nhạt · Cổ đức", 17, false),
            Product("Quần Ống Rộng Cạp Cao", "quan-ong-rong-cap-cao", clothing, 329000, 399000, 43, "photo-1483985988355-763728e1935b", "Mới về", "Quần ống rộng cạp cao giúp tôn dáng và tạo cảm giác thoải mái khi di chuyển. Dễ phối cùng áo thun, sơ mi hoặc áo kiểu.", "Vải tuyết mưa · Đen, kem · Cạp cao", 14, false),
            Product("Áo Khoác Cardigan Len Mỏng", "ao-khoac-cardigan-len-mong", clothing, 399000, 499000, 51, "photo-1529139574466-a303027c1d8b", "Bán chạy", "Cardigan len mỏng nhẹ nhàng khoác ngoài áo thun hoặc váy. Thiết kế tiện dụng, giữ ấm vừa phải và dễ phối đồ quanh năm.", "Len dệt mềm · Kem, nâu · Dáng suông", 9, false));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Product Product(
        string name,
        string slug,
        Category category,
        decimal price,
        decimal oldPrice,
        int rating,
        string image,
        string badge,
        string description,
        string detail,
        int stock,
        bool isFeatured) =>
        new()
        {
            Name = name,
            Slug = slug,
            Category = category,
            Price = price,
            OldPrice = oldPrice,
            Rating = rating,
            Image = image,
            Badge = badge,
            Description = description,
            Detail = detail,
            Stock = stock,
            IsFeatured = isFeatured
        };
}
