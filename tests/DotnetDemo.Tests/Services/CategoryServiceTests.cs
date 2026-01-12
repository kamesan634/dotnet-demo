using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Implementations;
using DotnetDemo.Tests.TestHelpers;
using Xunit;

namespace DotnetDemo.Tests.Services;

/// <summary>
/// 分類服務測試
/// </summary>
public class CategoryServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CategoryService _service;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;

    public CategoryServiceTests()
    {
        _context = MockDbContextFactory.CreateWithSeedData();
        _loggerMock = new Mock<ILogger<CategoryService>>();
        _service = new CategoryService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveCategories()
    {
        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(c => c.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCategory()
    {
        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _service.GetByIdAsync(99999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_RootCategory_SetsLevelTo1()
    {
        // Arrange
        var category = new Category
        {
            Code = "NEW01",
            Name = "新分類",
            ParentId = null,
            SortOrder = 99,
            IsActive = true
        };

        // Act
        var result = await _service.CreateAsync(category);

        // Assert
        result.Should().NotBeNull();
        result.Level.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_ChildCategory_SetsLevelTo2()
    {
        // Arrange
        var category = new Category
        {
            Code = "NEW02",
            Name = "子分類",
            ParentId = 1,
            SortOrder = 99,
            IsActive = true
        };

        // Act
        var result = await _service.CreateAsync(category);

        // Assert
        result.Should().NotBeNull();
        result.Level.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingCategory_ReturnsTrue()
    {
        // Arrange
        var category = await _service.GetByIdAsync(1);
        category!.Name = "更新後的分類";

        // Act
        var result = await _service.UpdateAsync(category);

        // Assert
        result.Should().BeTrue();

        var updated = await _service.GetByIdAsync(1);
        updated!.Name.Should().Be("更新後的分類");
    }

    [Fact]
    public async Task UpdateAsync_NonExistingCategory_ReturnsFalse()
    {
        // Arrange
        var category = new Category { Id = 99999, Code = "XXX", Name = "不存在" };

        // Act
        var result = await _service.UpdateAsync(category);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanDeleteAsync_CategoryWithChildren_ReturnsFalse()
    {
        // Category 1 has child category 3

        // Act
        var result = await _service.CanDeleteAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanDeleteAsync_CategoryWithProducts_ReturnsFalse()
    {
        // Category 1 has products

        // Act
        var result = await _service.CanDeleteAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_CategoryWithDependencies_ReturnsFalse()
    {
        // Category 1 has children and products

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_CategoryWithoutDependencies_ReturnsTrue()
    {
        // Arrange - Create a category without dependencies
        var newCategory = await _service.CreateAsync(new Category
        {
            Code = "DEL01",
            Name = "可刪除分類",
            SortOrder = 99,
            IsActive = true
        });

        // Act
        var result = await _service.DeleteAsync(newCategory.Id);

        // Assert
        result.Should().BeTrue();
    }
}
