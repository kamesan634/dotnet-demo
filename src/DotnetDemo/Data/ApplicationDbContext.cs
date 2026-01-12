using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DotnetDemo.Models.Entities;

namespace DotnetDemo.Data;

/// <summary>
/// 應用程式資料庫上下文
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    /// <summary>
    /// 建構子
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>門市</summary>
    public DbSet<Store> Stores => Set<Store>();

    /// <summary>商品分類</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>計量單位</summary>
    public DbSet<Unit> Units => Set<Unit>();

    /// <summary>商品</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>供應商</summary>
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    /// <summary>供應商價格</summary>
    public DbSet<SupplierPrice> SupplierPrices => Set<SupplierPrice>();

    /// <summary>會員等級</summary>
    public DbSet<CustomerLevel> CustomerLevels => Set<CustomerLevel>();

    /// <summary>客戶</summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>付款方式</summary>
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    /// <summary>倉庫</summary>
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    /// <summary>庫存</summary>
    public DbSet<Inventory> Inventories => Set<Inventory>();

    /// <summary>庫存異動</summary>
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    /// <summary>訂單</summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>訂單明細</summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    /// <summary>訂單付款</summary>
    public DbSet<OrderPayment> OrderPayments => Set<OrderPayment>();

    /// <summary>採購單</summary>
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    /// <summary>採購單明細</summary>
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    /// <summary>進貨單</summary>
    public DbSet<PurchaseReceipt> PurchaseReceipts => Set<PurchaseReceipt>();

    /// <summary>進貨單明細</summary>
    public DbSet<PurchaseReceiptItem> PurchaseReceiptItems => Set<PurchaseReceiptItem>();

    /// <summary>調撥單</summary>
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

    /// <summary>調撥單明細</summary>
    public DbSet<StockTransferItem> StockTransferItems => Set<StockTransferItem>();

    /// <summary>庫存調整單</summary>
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();

    /// <summary>庫存調整明細</summary>
    public DbSet<StockAdjustmentItem> StockAdjustmentItems => Set<StockAdjustmentItem>();

    /// <summary>操作紀錄</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// 配置模型
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Store
        builder.Entity<Store>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
        });

        // Category - 自我關聯
        builder.Entity<Category>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Unit
        builder.Entity<Unit>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
        });

        // Product
        builder.Entity<Product>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.HasIndex(x => x.Barcode);
            e.Property(x => x.CostPrice).HasPrecision(18, 2);
            e.Property(x => x.SellingPrice).HasPrecision(18, 2);
        });

        // Supplier
        builder.Entity<Supplier>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
        });

        // SupplierPrice
        builder.Entity<SupplierPrice>(e =>
        {
            e.HasIndex(x => new { x.SupplierId, x.ProductId });
            e.Property(x => x.Price).HasPrecision(18, 2);
        });

        // CustomerLevel
        builder.Entity<CustomerLevel>(e =>
        {
            e.Property(x => x.RequiredAmount).HasPrecision(18, 2);
            e.Property(x => x.DiscountPercent).HasPrecision(5, 2);
            e.Property(x => x.PointMultiplier).HasPrecision(5, 2);
        });

        // Customer
        builder.Entity<Customer>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.TotalSpent).HasPrecision(18, 2);
        });

        // PaymentMethod
        builder.Entity<PaymentMethod>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
        });

        // Warehouse
        builder.Entity<Warehouse>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
        });

        // Inventory
        builder.Entity<Inventory>(e =>
        {
            e.HasIndex(x => new { x.ProductId, x.WarehouseId }).IsUnique();
        });

        // Order
        builder.Entity<Order>(e =>
        {
            e.HasIndex(x => x.OrderNumber).IsUnique();
            e.Property(x => x.SubTotal).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.TaxAmount).HasPrecision(18, 2);
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
        });

        // OrderItem
        builder.Entity<OrderItem>(e =>
        {
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.SubTotal).HasPrecision(18, 2);
            e.Property(x => x.CostPrice).HasPrecision(18, 2);
        });

        // OrderPayment
        builder.Entity<OrderPayment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });

        // PurchaseOrder
        builder.Entity<PurchaseOrder>(e =>
        {
            e.HasIndex(x => x.OrderNumber).IsUnique();
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
        });

        // PurchaseOrderItem
        builder.Entity<PurchaseOrderItem>(e =>
        {
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.Property(x => x.SubTotal).HasPrecision(18, 2);
        });

        // PurchaseReceipt
        builder.Entity<PurchaseReceipt>(e =>
        {
            e.HasIndex(x => x.ReceiptNumber).IsUnique();
        });

        // StockTransfer
        builder.Entity<StockTransfer>(e =>
        {
            e.HasIndex(x => x.TransferNumber).IsUnique();
            e.HasOne(x => x.FromWarehouse)
                .WithMany()
                .HasForeignKey(x => x.FromWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ToWarehouse)
                .WithMany()
                .HasForeignKey(x => x.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StockAdjustment
        builder.Entity<StockAdjustment>(e =>
        {
            e.HasIndex(x => x.AdjustmentNumber).IsUnique();
        });

        // AuditLog
        builder.Entity<AuditLog>(e =>
        {
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => new { x.EntityType, x.EntityId });
        });
    }
}
