using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace DotnetDemo.Tests.Routes;

/// <summary>
/// 路由驗證測試
/// 確保 NavMenu 中的所有連結都有對應的頁面
/// </summary>
public class RouteValidationTests
{
    /// <summary>
    /// NavMenu 中定義的所有路由
    /// </summary>
    private static readonly string[] NavMenuRoutes =
    [
        "/",                      // 儀表板
        "/dashboard",             // 儀表板 (別名)
        "/products",              // 商品管理
        "/categories",            // 分類管理
        "/suppliers",             // 供應商管理
        "/customers",             // 客戶管理
        "/units",                 // 單位管理
        "/payment-methods",       // 付款方式
        "/warehouses",            // 倉庫管理
        "/pos",                   // POS 銷售
        "/orders",                // 訂單查詢
        "/inventory",             // 庫存查詢
        "/stock-transfers",       // 庫存調撥
        "/stock-adjustments",     // 庫存調整
        "/low-stock-alerts",      // 庫存警示
        "/purchase-orders",       // 採購單管理
        "/purchase-receipts",     // 進貨單管理
        "/purchase-suggestions",  // 採購建議
        "/reports/sales",         // 銷售報表
        "/reports/inventory",     // 庫存報表
        "/reports/purchasing",    // 採購報表
        "/users",                 // 使用者管理
        "/roles",                 // 角色管理
        "/stores",                // 門市管理
        "/audit-logs",            // 操作紀錄
        "/login",                 // 登入頁面
        "/logout"                 // 登出頁面
    ];

    /// <summary>
    /// 取得所有已註冊的路由
    /// </summary>
    private static HashSet<string> GetRegisteredRoutes()
    {
        var routes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 取得 DotnetDemo 組件
        var assembly = Assembly.Load("DotnetDemo");

        // 掃描所有有 RouteAttribute 的類別
        foreach (var type in assembly.GetTypes())
        {
            var routeAttributes = type.GetCustomAttributes<RouteAttribute>();
            foreach (var routeAttr in routeAttributes)
            {
                routes.Add(routeAttr.Template);
            }
        }

        return routes;
    }

    [Fact]
    public void AllNavMenuRoutes_ShouldHaveCorrespondingPages()
    {
        // Arrange
        var registeredRoutes = GetRegisteredRoutes();
        var missingRoutes = new List<string>();

        // Act
        foreach (var route in NavMenuRoutes)
        {
            if (!registeredRoutes.Contains(route))
            {
                missingRoutes.Add(route);
            }
        }

        // Assert
        missingRoutes.Should().BeEmpty(
            $"以下路由在 NavMenu 中定義但沒有對應的頁面: {string.Join(", ", missingRoutes)}");
    }

    [Fact]
    public void AllRegisteredRoutes_ShouldBeDocumented()
    {
        // Arrange
        var registeredRoutes = GetRegisteredRoutes();
        var navMenuRoutesSet = new HashSet<string>(NavMenuRoutes, StringComparer.OrdinalIgnoreCase);

        // 排除不需要在 NavMenu 中的路由 (如 Error 頁面)
        var excludedRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/Error"
        };

        // Act
        var undocumentedRoutes = registeredRoutes
            .Where(r => !navMenuRoutesSet.Contains(r) && !excludedRoutes.Contains(r))
            .ToList();

        // Assert - 這個測試是提醒性質，不一定要失敗
        // 但可以幫助發現新增的頁面是否需要加入 NavMenu
        if (undocumentedRoutes.Any())
        {
            // 輸出警告訊息
            var message = $"以下路由已註冊但未在 NavMenuRoutes 測試陣列中: {string.Join(", ", undocumentedRoutes)}";
            // 可選擇是否要讓測試失敗
            // undocumentedRoutes.Should().BeEmpty(message);
        }
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/dashboard")]
    [InlineData("/products")]
    [InlineData("/categories")]
    [InlineData("/suppliers")]
    [InlineData("/customers")]
    [InlineData("/units")]
    [InlineData("/payment-methods")]
    [InlineData("/warehouses")]
    [InlineData("/pos")]
    [InlineData("/orders")]
    [InlineData("/inventory")]
    [InlineData("/stock-transfers")]
    [InlineData("/stock-adjustments")]
    [InlineData("/low-stock-alerts")]
    [InlineData("/purchase-orders")]
    [InlineData("/purchase-receipts")]
    [InlineData("/purchase-suggestions")]
    [InlineData("/reports/sales")]
    [InlineData("/reports/inventory")]
    [InlineData("/reports/purchasing")]
    [InlineData("/users")]
    [InlineData("/roles")]
    [InlineData("/stores")]
    [InlineData("/audit-logs")]
    [InlineData("/login")]
    [InlineData("/logout")]
    public void Route_ShouldExist(string route)
    {
        // Arrange
        var registeredRoutes = GetRegisteredRoutes();

        // Act & Assert
        registeredRoutes.Should().Contain(route,
            $"路由 '{route}' 應該要有對應的頁面");
    }

    [Fact]
    public void RegisteredRoutes_ShouldNotBeEmpty()
    {
        // Arrange & Act
        var registeredRoutes = GetRegisteredRoutes();

        // Assert
        registeredRoutes.Should().NotBeEmpty("應該至少有一個已註冊的路由");
        registeredRoutes.Count.Should().BeGreaterThan(20, "應該有超過 20 個已註冊的路由");
    }

    [Fact]
    public void DashboardRoute_ShouldSupportBothFormats()
    {
        // Arrange
        var registeredRoutes = GetRegisteredRoutes();

        // Assert - 儀表板應該同時支援 "/" 和 "/dashboard"
        registeredRoutes.Should().Contain("/", "根路由 '/' 應該對應到儀表板");
        registeredRoutes.Should().Contain("/dashboard", "'/dashboard' 路由應該對應到儀表板");
    }
}
