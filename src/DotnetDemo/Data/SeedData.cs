using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotnetDemo.Models.Entities;

namespace DotnetDemo.Data;

/// <summary>
/// 種子資料初始化
/// </summary>
public static class SeedData
{
    /// <summary>
    /// 初始化種子資料
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // 建立角色
        await SeedRolesAsync(roleManager);

        // 建立預設使用者
        await SeedUsersAsync(userManager);

        // 建立門市
        await SeedStoresAsync(context);

        // 建立單位
        await SeedUnitsAsync(context);

        // 建立分類
        await SeedCategoriesAsync(context);

        // 建立會員等級
        await SeedCustomerLevelsAsync(context);

        // 建立付款方式
        await SeedPaymentMethodsAsync(context);

        // 建立倉庫
        await SeedWarehousesAsync(context);

        // 建立供應商
        await SeedSuppliersAsync(context);

        // 建立商品
        await SeedProductsAsync(context);

        // 建立客戶
        await SeedCustomersAsync(context);

        // 建立庫存
        await SeedInventoryAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[]
        {
            new ApplicationRole { Name = "Admin", Description = "系統管理員", IsSystemRole = true },
            new ApplicationRole { Name = "Manager", Description = "店長", IsSystemRole = true },
            new ApplicationRole { Name = "Cashier", Description = "收銀員", IsSystemRole = true },
            new ApplicationRole { Name = "Warehouse", Description = "倉管人員", IsSystemRole = true }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new[]
        {
            new { User = new ApplicationUser { UserName = "admin", Email = "admin@demo.com", DisplayName = "系統管理員", IsActive = true }, Password = "password123", Role = "Admin" },
            new { User = new ApplicationUser { UserName = "manager", Email = "manager@demo.com", DisplayName = "店長小王", IsActive = true }, Password = "password123", Role = "Manager" },
            new { User = new ApplicationUser { UserName = "cashier", Email = "cashier@demo.com", DisplayName = "收銀員小美", IsActive = true }, Password = "password123", Role = "Cashier" },
            new { User = new ApplicationUser { UserName = "warehouse", Email = "warehouse@demo.com", DisplayName = "倉管小李", IsActive = true }, Password = "password123", Role = "Warehouse" }
        };

        foreach (var item in users)
        {
            if (await userManager.FindByNameAsync(item.User.UserName!) == null)
            {
                var result = await userManager.CreateAsync(item.User, item.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(item.User, item.Role);
                }
            }
        }
    }

    private static async Task SeedStoresAsync(ApplicationDbContext context)
    {
        if (await context.Stores.AnyAsync()) return;

        var stores = new[]
        {
            new Store { Code = "S001", Name = "台北總店", Address = "台北市中正區忠孝東路一段1號", Phone = "02-12345678", IsPrimary = true },
            new Store { Code = "S002", Name = "台中分店", Address = "台中市西屯區台灣大道三段1號", Phone = "04-12345678" },
            new Store { Code = "S003", Name = "高雄分店", Address = "高雄市前鎮區中山二路1號", Phone = "07-12345678" }
        };

        context.Stores.AddRange(stores);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUnitsAsync(ApplicationDbContext context)
    {
        if (await context.Units.AnyAsync()) return;

        var units = new[]
        {
            new Unit { Code = "PCS", Name = "個" },
            new Unit { Code = "BOX", Name = "盒" },
            new Unit { Code = "PKG", Name = "包" },
            new Unit { Code = "BTL", Name = "瓶" },
            new Unit { Code = "CAN", Name = "罐" },
            new Unit { Code = "BAG", Name = "袋" },
            new Unit { Code = "SET", Name = "組" },
            new Unit { Code = "KG", Name = "公斤" }
        };

        context.Units.AddRange(units);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var categories = new[]
        {
            new Category { Code = "C01", Name = "飲料", Level = 1, SortOrder = 1 },
            new Category { Code = "C02", Name = "零食", Level = 1, SortOrder = 2 },
            new Category { Code = "C03", Name = "日用品", Level = 1, SortOrder = 3 },
            new Category { Code = "C04", Name = "生鮮食品", Level = 1, SortOrder = 4 },
            new Category { Code = "C05", Name = "冷凍食品", Level = 1, SortOrder = 5 }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // 子分類
        var drinkCategory = await context.Categories.FirstAsync(c => c.Code == "C01");
        var snackCategory = await context.Categories.FirstAsync(c => c.Code == "C02");

        var subCategories = new[]
        {
            new Category { Code = "C0101", Name = "茶類", ParentId = drinkCategory.Id, Level = 2, SortOrder = 1 },
            new Category { Code = "C0102", Name = "咖啡", ParentId = drinkCategory.Id, Level = 2, SortOrder = 2 },
            new Category { Code = "C0103", Name = "果汁", ParentId = drinkCategory.Id, Level = 2, SortOrder = 3 },
            new Category { Code = "C0201", Name = "餅乾", ParentId = snackCategory.Id, Level = 2, SortOrder = 1 },
            new Category { Code = "C0202", Name = "糖果", ParentId = snackCategory.Id, Level = 2, SortOrder = 2 },
            new Category { Code = "C0203", Name = "巧克力", ParentId = snackCategory.Id, Level = 2, SortOrder = 3 }
        };

        context.Categories.AddRange(subCategories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomerLevelsAsync(ApplicationDbContext context)
    {
        if (await context.CustomerLevels.AnyAsync()) return;

        var levels = new[]
        {
            new CustomerLevel { Name = "一般會員", RequiredAmount = 0, DiscountPercent = 0, PointMultiplier = 1, SortOrder = 1 },
            new CustomerLevel { Name = "銀卡會員", RequiredAmount = 5000, DiscountPercent = 3, PointMultiplier = 1.2m, SortOrder = 2 },
            new CustomerLevel { Name = "金卡會員", RequiredAmount = 20000, DiscountPercent = 5, PointMultiplier = 1.5m, SortOrder = 3 },
            new CustomerLevel { Name = "白金會員", RequiredAmount = 50000, DiscountPercent = 8, PointMultiplier = 2, SortOrder = 4 },
            new CustomerLevel { Name = "鑽石會員", RequiredAmount = 100000, DiscountPercent = 10, PointMultiplier = 3, SortOrder = 5 }
        };

        context.CustomerLevels.AddRange(levels);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPaymentMethodsAsync(ApplicationDbContext context)
    {
        if (await context.PaymentMethods.AnyAsync()) return;

        var methods = new[]
        {
            new PaymentMethod { Code = "CASH", Name = "現金", RequiresChange = true, SortOrder = 1 },
            new PaymentMethod { Code = "CARD", Name = "信用卡", RequiresChange = false, SortOrder = 2 },
            new PaymentMethod { Code = "LINEPAY", Name = "Line Pay", RequiresChange = false, SortOrder = 3 },
            new PaymentMethod { Code = "JKOPAY", Name = "街口支付", RequiresChange = false, SortOrder = 4 },
            new PaymentMethod { Code = "APPLEPAY", Name = "Apple Pay", RequiresChange = false, SortOrder = 5 }
        };

        context.PaymentMethods.AddRange(methods);
        await context.SaveChangesAsync();
    }

    private static async Task SeedWarehousesAsync(ApplicationDbContext context)
    {
        if (await context.Warehouses.AnyAsync()) return;

        var store = await context.Stores.FirstOrDefaultAsync();

        var warehouses = new[]
        {
            new Warehouse { Code = "W001", Name = "台北總倉", StoreId = store?.Id, Address = "台北市內湖區瑞光路100號", ContactName = "王小明", Phone = "02-87654321", IsPrimary = true },
            new Warehouse { Code = "W002", Name = "台中倉庫", Address = "台中市大里區工業路50號", ContactName = "李小華", Phone = "04-87654321" },
            new Warehouse { Code = "W003", Name = "高雄倉庫", Address = "高雄市小港區工業街30號", ContactName = "張小強", Phone = "07-87654321" }
        };

        context.Warehouses.AddRange(warehouses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSuppliersAsync(ApplicationDbContext context)
    {
        if (await context.Suppliers.AnyAsync()) return;

        var suppliers = new[]
        {
            new Supplier { Code = "SUP001", Name = "統一企業", ContactName = "陳經理", Phone = "02-11111111", Email = "uni@demo.com", Address = "台南市永康區工業路1號", TaxId = "12345678", PaymentTermDays = 30 },
            new Supplier { Code = "SUP002", Name = "味全食品", ContactName = "林經理", Phone = "02-22222222", Email = "weichuan@demo.com", Address = "台北市內湖區瑞光路2號", TaxId = "23456789", PaymentTermDays = 30 },
            new Supplier { Code = "SUP003", Name = "義美食品", ContactName = "王經理", Phone = "02-33333333", Email = "imei@demo.com", Address = "桃園市蘆竹區工業路3號", TaxId = "34567890", PaymentTermDays = 45 },
            new Supplier { Code = "SUP004", Name = "光泉牧場", ContactName = "張經理", Phone = "02-44444444", Email = "kuangchuan@demo.com", Address = "台北市中山區工業路4號", TaxId = "45678901", PaymentTermDays = 30 },
            new Supplier { Code = "SUP005", Name = "黑松公司", ContactName = "劉經理", Phone = "02-55555555", Email = "heysong@demo.com", Address = "台北市松山區工業路5號", TaxId = "56789012", PaymentTermDays = 30 }
        };

        context.Suppliers.AddRange(suppliers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync()) return;

        var teaCategory = await context.Categories.FirstAsync(c => c.Code == "C0101");
        var coffeeCategory = await context.Categories.FirstAsync(c => c.Code == "C0102");
        var cookieCategory = await context.Categories.FirstAsync(c => c.Code == "C0201");
        var pcsUnit = await context.Units.FirstAsync(u => u.Code == "PCS");
        var btlUnit = await context.Units.FirstAsync(u => u.Code == "BTL");
        var boxUnit = await context.Units.FirstAsync(u => u.Code == "BOX");
        var supplier1 = await context.Suppliers.FirstAsync(s => s.Code == "SUP001");
        var supplier5 = await context.Suppliers.FirstAsync(s => s.Code == "SUP005");

        var products = new[]
        {
            new Product { Code = "P001", Barcode = "4710088430120", Name = "純喫茶-檸檬紅茶 650ml", CategoryId = teaCategory.Id, UnitId = btlUnit.Id, CostPrice = 18, SellingPrice = 25, SafetyStock = 50, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P002", Barcode = "4710088430137", Name = "純喫茶-無糖綠茶 650ml", CategoryId = teaCategory.Id, UnitId = btlUnit.Id, CostPrice = 18, SellingPrice = 25, SafetyStock = 50, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P003", Barcode = "4714431040015", Name = "黑松沙士 600ml", CategoryId = teaCategory.Id, UnitId = btlUnit.Id, CostPrice = 15, SellingPrice = 22, SafetyStock = 30, DefaultSupplierId = supplier5.Id },
            new Product { Code = "P004", Barcode = "4710088431004", Name = "左岸咖啡-昂列白 240ml", CategoryId = coffeeCategory.Id, UnitId = pcsUnit.Id, CostPrice = 25, SellingPrice = 35, SafetyStock = 30, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P005", Barcode = "4710088432001", Name = "左岸咖啡-拿鐵 240ml", CategoryId = coffeeCategory.Id, UnitId = pcsUnit.Id, CostPrice = 25, SellingPrice = 35, SafetyStock = 30, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P006", Barcode = "4711271000115", Name = "義美小泡芙-巧克力", CategoryId = cookieCategory.Id, UnitId = boxUnit.Id, CostPrice = 30, SellingPrice = 45, SafetyStock = 20, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P007", Barcode = "4711271000122", Name = "義美小泡芙-牛奶", CategoryId = cookieCategory.Id, UnitId = boxUnit.Id, CostPrice = 30, SellingPrice = 45, SafetyStock = 20, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P008", Barcode = "4710088001015", Name = "統一科學麵 5入", CategoryId = cookieCategory.Id, UnitId = pcsUnit.Id, CostPrice = 35, SellingPrice = 50, SafetyStock = 25, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P009", Barcode = "4710088002015", Name = "統一肉燥麵 5入", CategoryId = cookieCategory.Id, UnitId = pcsUnit.Id, CostPrice = 45, SellingPrice = 65, SafetyStock = 25, DefaultSupplierId = supplier1.Id },
            new Product { Code = "P010", Barcode = "4710088003015", Name = "統一布丁", CategoryId = cookieCategory.Id, UnitId = pcsUnit.Id, CostPrice = 12, SellingPrice = 18, SafetyStock = 40, DefaultSupplierId = supplier1.Id }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomersAsync(ApplicationDbContext context)
    {
        if (await context.Customers.AnyAsync()) return;

        var normalLevel = await context.CustomerLevels.FirstAsync(l => l.SortOrder == 1);
        var silverLevel = await context.CustomerLevels.FirstAsync(l => l.SortOrder == 2);
        var goldLevel = await context.CustomerLevels.FirstAsync(l => l.SortOrder == 3);

        var customers = new[]
        {
            new Customer { Code = "M001", Name = "王大明", Phone = "0912345678", Email = "wang@demo.com", CustomerLevelId = goldLevel.Id, Points = 1500, TotalSpent = 25000 },
            new Customer { Code = "M002", Name = "李小華", Phone = "0923456789", Email = "lee@demo.com", CustomerLevelId = silverLevel.Id, Points = 800, TotalSpent = 8000 },
            new Customer { Code = "M003", Name = "張美玲", Phone = "0934567890", Email = "chang@demo.com", CustomerLevelId = normalLevel.Id, Points = 200, TotalSpent = 2000 },
            new Customer { Code = "M004", Name = "陳志明", Phone = "0945678901", Email = "chen@demo.com", CustomerLevelId = normalLevel.Id, Points = 100, TotalSpent = 1000 },
            new Customer { Code = "M005", Name = "林淑芬", Phone = "0956789012", Email = "lin@demo.com", CustomerLevelId = silverLevel.Id, Points = 600, TotalSpent = 6000 }
        };

        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedInventoryAsync(ApplicationDbContext context)
    {
        if (await context.Inventories.AnyAsync()) return;

        var products = await context.Products.ToListAsync();
        var warehouse = await context.Warehouses.FirstAsync(w => w.IsPrimary);

        var inventories = products.Select((p, i) => new Inventory
        {
            ProductId = p.Id,
            WarehouseId = warehouse.Id,
            Quantity = 100 + (i * 10),
            ReservedQuantity = 0
        }).ToList();

        context.Inventories.AddRange(inventories);
        await context.SaveChangesAsync();
    }
}
