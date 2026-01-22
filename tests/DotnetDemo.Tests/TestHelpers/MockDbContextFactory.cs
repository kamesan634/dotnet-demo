using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;

namespace DotnetDemo.Tests.TestHelpers;

/// <summary>
/// Mock DbContext 工廠
/// </summary>
public static class MockDbContextFactory
{
    /// <summary>
    /// 建立空的 InMemory DbContext
    /// </summary>
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options);
    }

    /// <summary>
    /// 建立含有種子資料的 InMemory DbContext
    /// </summary>
    public static ApplicationDbContext CreateWithSeedData()
    {
        var context = Create();
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        // 單位
        var units = new[]
        {
            new Unit { Id = 1, Code = "PCS", Name = "個", IsActive = true },
            new Unit { Id = 2, Code = "BOX", Name = "盒", IsActive = true }
        };
        context.Units.AddRange(units);

        // 分類
        var categories = new[]
        {
            new Category { Id = 1, Code = "C01", Name = "飲料", Level = 1, SortOrder = 1, IsActive = true },
            new Category { Id = 2, Code = "C02", Name = "零食", Level = 1, SortOrder = 2, IsActive = true },
            new Category { Id = 3, Code = "C0101", Name = "茶類", ParentId = 1, Level = 2, SortOrder = 1, IsActive = true }
        };
        context.Categories.AddRange(categories);

        // 會員等級
        var customerLevels = new[]
        {
            new CustomerLevel { Id = 1, Name = "一般會員", RequiredAmount = 0, DiscountPercent = 0, PointMultiplier = 1, SortOrder = 1, IsActive = true },
            new CustomerLevel { Id = 2, Name = "銀卡會員", RequiredAmount = 5000, DiscountPercent = 3, PointMultiplier = 1.2m, SortOrder = 2, IsActive = true }
        };
        context.CustomerLevels.AddRange(customerLevels);

        // 供應商
        var suppliers = new[]
        {
            new Supplier { Id = 1, Code = "SUP001", Name = "測試供應商1", ContactName = "張三", Phone = "02-1234567", IsActive = true },
            new Supplier { Id = 2, Code = "SUP002", Name = "測試供應商2", ContactName = "李四", Phone = "02-7654321", IsActive = true }
        };
        context.Suppliers.AddRange(suppliers);

        // 商品
        var products = new[]
        {
            new Product { Id = 1, Code = "P001", Barcode = "1234567890", Name = "測試商品1", CategoryId = 1, UnitId = 1, CostPrice = 10, SellingPrice = 20, SafetyStock = 10, IsActive = true },
            new Product { Id = 2, Code = "P002", Barcode = "0987654321", Name = "測試商品2", CategoryId = 1, UnitId = 1, CostPrice = 15, SellingPrice = 30, SafetyStock = 5, IsActive = true },
            new Product { Id = 3, Code = "P003", Name = "停用商品", CategoryId = 2, UnitId = 2, CostPrice = 20, SellingPrice = 40, SafetyStock = 10, IsActive = false }
        };
        context.Products.AddRange(products);

        // 客戶
        var customers = new[]
        {
            new Customer { Id = 1, Code = "M001", Name = "測試客戶1", Phone = "0912345678", CustomerLevelId = 1, Points = 100, TotalSpent = 1000, IsActive = true },
            new Customer { Id = 2, Code = "M002", Name = "測試客戶2", Phone = "0923456789", CustomerLevelId = 2, Points = 500, TotalSpent = 5000, IsActive = true }
        };
        context.Customers.AddRange(customers);

        // 門市
        var stores = new[]
        {
            new Store { Id = 1, Code = "S001", Name = "測試門市", IsActive = true }
        };
        context.Stores.AddRange(stores);

        // 倉庫
        var warehouses = new[]
        {
            new Warehouse { Id = 1, Code = "W001", Name = "測試倉庫", StoreId = 1, IsPrimary = true, IsActive = true },
            new Warehouse { Id = 2, Code = "W002", Name = "測試倉庫2", StoreId = 1, IsPrimary = false, IsActive = true }
        };
        context.Warehouses.AddRange(warehouses);

        // 庫存
        var inventories = new[]
        {
            new Inventory { Id = 1, ProductId = 1, WarehouseId = 1, Quantity = 100, ReservedQuantity = 0 },
            new Inventory { Id = 2, ProductId = 2, WarehouseId = 1, Quantity = 3, ReservedQuantity = 0 } // 低於安全庫存
        };
        context.Inventories.AddRange(inventories);

        // 付款方式
        var paymentMethods = new[]
        {
            new PaymentMethod { Id = 1, Code = "CASH", Name = "現金", RequiresChange = true, SortOrder = 1, IsActive = true }
        };
        context.PaymentMethods.AddRange(paymentMethods);

        // 訂單
        var orders = new[]
        {
            new Order
            {
                Id = 1,
                OrderNumber = "ORD202401010001",
                StoreId = 1,
                CustomerId = 1,
                Status = OrderStatus.Completed,
                SubTotal = 100,
                DiscountAmount = 0,
                TaxAmount = 0,
                TotalAmount = 100,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow.Date
            },
            new Order
            {
                Id = 2,
                OrderNumber = "ORD202401010002",
                StoreId = 1,
                Status = OrderStatus.Pending,
                SubTotal = 200,
                DiscountAmount = 10,
                TaxAmount = 0,
                TotalAmount = 190,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow.Date
            }
        };
        context.Orders.AddRange(orders);

        // 訂單明細
        var orderItems = new[]
        {
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, ProductName = "測試商品1", UnitPrice = 20, Quantity = 5, DiscountAmount = 0, SubTotal = 100, CostPrice = 10 }
        };
        context.OrderItems.AddRange(orderItems);

        // 採購單
        var purchaseOrders = new[]
        {
            new PurchaseOrder
            {
                Id = 1,
                OrderNumber = "PO202401010001",
                SupplierId = 1,
                WarehouseId = 1,
                Status = PurchaseOrderStatus.Draft,
                ExpectedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                TotalAmount = 1000,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            }
        };
        context.PurchaseOrders.AddRange(purchaseOrders);

        context.SaveChanges();
    }
}
