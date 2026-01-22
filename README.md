#  龜三的ERP Demo (Retail Management System)

![CI](https://github.com/kamesan634/dotnet-demo/actions/workflows/ci.yml/badge.svg)

一個使用 ASP.NET Core 8.0 + Blazor Server 建構的現代化零售管理系統，提供完整的 POS 銷售、庫存管理、採購管理等功能。

## 技能樹 請點以下技能

| 技能 | 版本 | 說明 |
|------|------|------|
| ASP.NET Core | 8.0 | 後端框架 |
| Blazor Server | 8.0 | 前端互動式 UI 框架 |
| Entity Framework Core | 8.0 | ORM 資料存取 |
| MySQL | 8.4 | 關聯式資料庫 |
| Redis | 7 | 分散式快取 |
| MudBlazor | 6.11 | UI 元件庫 |
| ASP.NET Identity | 8.0 | 身份驗證與授權 |
| Serilog | 8.0 | 結構化日誌 |
| Docker | - | 容器化部署 |

## 系統功能

### 系統管理
- 使用者管理
- 角色權限管理
- 門市管理
- 操作紀錄

### 基礎資料
- 商品管理
- 分類管理
- 供應商管理
- 客戶/會員管理
- 單位管理
- 付款方式管理
- 倉庫管理

### 銷售管理
- POS 銷售
- 訂單查詢
- 會員消費紀錄

### 庫存管理
- 庫存查詢
- 庫存調撥
- 庫存調整
- 低庫存警示

### 採購管理
- 採購單管理
- 進貨單管理
- 採購建議

### 報表分析
- 銷售報表
- 庫存報表
- 採購報表
- 儀表板 KPI

## 快速開始

### 環境需求
- .NET 8.0 SDK
- Docker & Docker Compose (選用)

### 使用 Docker 啟動

```bash
# 啟動所有服務
docker-compose up -d

# 查看服務狀態
docker-compose ps

# 停止服務
docker-compose down
```

### 本地開發

```bash
# 1. 啟動 MySQL 和 Redis
docker-compose up -d mysql redis

# 2. 還原套件
cd src/DotnetDemo
dotnet restore

# 3. 執行資料庫遷移
dotnet ef database update

# 4. 啟動應用程式
dotnet run
```

## Port 配置

| 服務 | Port |
|------|------|
| ASP.NET Core | 8004 |
| MySQL | 3304 |
| Redis | 6384 |

## 資料庫配置

| 項目 | 值 |
|------|------|
| 資料庫名稱 | dotnetdemo_db |
| 使用者 | root |
| 密碼 | dev123 |

## 測試帳號

| 帳號 | 密碼 | 角色 |
|------|------|------|
| admin | password123 | 系統管理員 |
| manager | password123 | 店長 |
| cashier | password123 | 收銀員 |
| warehouse | password123 | 倉管人員 |

## 專案結構

```
dotnet-demo/
├── src/
│   └── DotnetDemo/
│       ├── Components/
│       │   ├── Layout/          # 版面配置
│       │   └── Pages/           # 頁面元件
│       │       ├── Auth/        # 登入/登出
│       │       ├── Dashboard/   # 儀表板
│       │       ├── BasicData/   # 基礎資料
│       │       ├── Sales/       # 銷售
│       │       ├── Inventory/   # 庫存
│       │       ├── Purchasing/  # 採購
│       │       └── Reports/     # 報表
│       ├── Data/                # DbContext
│       ├── Models/
│       │   ├── Entities/        # 實體類別
│       │   └── DTOs/            # 資料傳輸物件
│       ├── Services/            # 商業邏輯服務
│       └── wwwroot/             # 靜態檔案
├── tests/                       # 單元測試
├── docker-compose.yml
└── README.md
```

## 開發指南

### 建立新頁面

1. 在 `Components/Pages` 下建立 `.razor` 檔案
2. 加入 `@page` 路由指令
3. 加入 `@attribute [Authorize]` 驗證屬性
4. 在 `NavMenu.razor` 加入導覽連結

### 建立新實體

1. 在 `Models/Entities` 下建立實體類別
2. 在 `ApplicationDbContext` 加入 DbSet
3. 執行 `dotnet ef migrations add <MigrationName>`
4. 執行 `dotnet ef database update`

### 日誌配置

日誌使用 Serilog，配置在 `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" }}
    ]
  }
}
```

## 測試

### 執行測試

```bash
# 執行所有測試
dotnet test

# 執行測試並產生覆蓋率報告
dotnet test --collect:"XPlat Code Coverage"

# 執行特定測試類別
dotnet test --filter "FullyQualifiedName~ReportServiceTests"
```

### 測試覆蓋率目標

| 模組 | 目標覆蓋率 | 說明 |
|------|-----------|------|
| Service 層 | 80%+ | 商業邏輯核心測試 |
| ReportService | 100% | 報表功能完整測試 |

### 測試工具

| 套件 | 說明 |
|------|------|
| xUnit | 測試框架 |
| Moq | Mock 框架 |
| FluentAssertions | 流暢斷言語法 |
| EF Core InMemory | 記憶體資料庫測試 |

## 健康檢查

應用程式提供健康檢查端點：

```
GET /health
```

回傳 MySQL 和 Redis 的連線狀態。

## License

MIT License
我一開始以為是Made In Taiwan 咧！(羞
