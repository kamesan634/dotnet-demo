using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using DotnetDemo.Components;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;
using DotnetDemo.Services.Implementations;

// 設定 Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("啟動應用程式");

    var builder = WebApplication.CreateBuilder(args);

    // 配置 Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // 配置 Entity Framework Core
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var serverVersion = new MySqlServerVersion(new Version(8, 4, 0));
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, serverVersion));

    // 配置 ASP.NET Identity
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // 配置 Cookie 驗證
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

    // 配置 Redis 快取
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "DotnetDemo_";
        });
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }

    // 註冊應用程式服務
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<ISupplierService, SupplierService>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    builder.Services.AddScoped<IReportService, ReportService>();

    // 配置 MudBlazor
    builder.Services.AddMudServices(config =>
    {
        config.PopoverOptions.ThrowOnDuplicateProvider = false;
    });

    // 配置 Blazor 服務
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // 配置健康檢查
    builder.Services.AddHealthChecks()
        .AddMySql(connectionString!, name: "mysql")
        .AddRedis(redisConnectionString ?? "localhost:6379", name: "redis");

    // 配置授權
    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();

    var app = builder.Build();

    // 配置 Serilog 請求日誌
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    // 配置中介軟體
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    // 健康檢查端點
    app.MapHealthChecks("/health");

    // 登入端點 (處理 Cookie 認證)
    app.MapGet("/api/auth/login", async (
        string userName,
        bool rememberMe,
        string? returnUrl,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager) =>
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user != null)
        {
            await signInManager.SignInAsync(user, rememberMe);
        }
        var redirectUrl = string.IsNullOrEmpty(returnUrl) ? "/dashboard" : Uri.UnescapeDataString(returnUrl);
        if (redirectUrl == "/" || redirectUrl == "%2F" || redirectUrl == "/index.html")
        {
            redirectUrl = "/dashboard";
        }
        return Results.LocalRedirect(redirectUrl);
    });

    // 登出端點
    app.MapGet("/api/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Redirect("/login");
    });

    // Blazor 端點
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    // 自動建立資料庫 (開發環境)
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 使用 EnsureCreated 建立資料表 (如果不存在)
        await context.Database.EnsureCreatedAsync();

        // 種子資料
        await SeedData.InitializeAsync(scope.ServiceProvider);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "應用程式啟動失敗");
}
finally
{
    Log.CloseAndFlush();
}
