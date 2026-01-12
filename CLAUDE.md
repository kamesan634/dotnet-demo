# Claude Code 專案指引

## 專案概述

這是一個 ASP.NET Core 8.0 + Blazor Server 全端專案，提供完整的零售管理系統功能。

## README.md 格式規範

### 技術棧區塊

技術棧標題和表格一律使用以下格式：

```markdown
## 技能樹 請點以下技能

| 技能 | 版本 | 說明 |
|------|------|------|
```

### License 區塊

License 部分一律使用以下格式：

```markdown
## License

MIT License
我一開始以為是Made In Taiwan 咧！(羞
```

## 程式碼規範

### XML 文件註釋格式

所有類別和公開方法都要加上中文 XML 註釋：

```csharp
/// <summary>
/// 類別說明
/// </summary>
public class MyClass
{
    /// <summary>
    /// 方法說明
    /// </summary>
    /// <param name="param">參數說明</param>
    /// <returns>回傳值說明</returns>
    public string MyMethod(string param)
    {
        // ...
    }
}
```

### 專案結構

```
src/DotnetDemo/
├── Components/
│   ├── Layout/          # 版面配置元件
│   └── Pages/           # 頁面元件
│       ├── Auth/        # 登入/登出
│       ├── Dashboard/   # 儀表板
│       ├── BasicData/   # 基礎資料模組
│       ├── Sales/       # 銷售模組
│       ├── Inventory/   # 庫存模組
│       ├── Purchasing/  # 採購模組
│       └── Reports/     # 報表模組
├── Data/                # DbContext 與種子資料
├── Models/
│   ├── Entities/        # 實體類別
│   └── DTOs/            # 資料傳輸物件
├── Services/
│   ├── Interfaces/      # 服務介面
│   └── Implementations/ # 服務實作
└── wwwroot/             # 靜態檔案
```

## Port 配置

| 服務 | Port |
|------|------|
| ASP.NET Core | 8004 |
| MySQL | 3304 |
| Redis | 6384 |

## 測試帳號

密碼一律使用：`password123`

| 帳號 | 角色 |
|------|------|
| admin | 系統管理員 |
| manager | 店長 |
| cashier | 收銀員 |
| warehouse | 倉管人員 |

## 常用指令

```bash
# 啟動 Docker 服務
docker-compose up -d

# 執行資料庫遷移
dotnet ef migrations add <MigrationName>
dotnet ef database update

# 啟動應用程式
dotnet run

# 執行測試
dotnet test

# 執行測試並產生覆蓋率報告
dotnet test --collect:"XPlat Code Coverage"
```

## 測試架構

### 測試專案結構

```
tests/DotnetDemo.Tests/
├── TestHelpers/
│   └── MockDbContextFactory.cs   # InMemory DbContext 工廠
└── Services/
    ├── ProductServiceTests.cs    # 商品服務測試
    ├── CategoryServiceTests.cs   # 分類服務測試
    ├── CustomerServiceTests.cs   # 客戶服務測試
    ├── SupplierServiceTests.cs   # 供應商服務測試
    ├── InventoryServiceTests.cs  # 庫存服務測試
    ├── OrderServiceTests.cs      # 訂單服務測試
    ├── PurchaseOrderServiceTests.cs  # 採購單服務測試
    └── ReportServiceTests.cs     # 報表服務測試 (100% 覆蓋率目標)
```

### 測試工具

| 套件 | 用途 |
|------|------|
| xUnit | 測試框架 |
| Moq | Mock 框架 |
| FluentAssertions | 斷言庫 |
| Microsoft.EntityFrameworkCore.InMemory | InMemory 資料庫 |
| coverlet.collector | 覆蓋率收集 |

### 覆蓋率目標

| 模組 | 目標覆蓋率 |
|------|-----------|
| Service 層 | 80%+ |
| ReportService | 100% |
