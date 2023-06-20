using Xunit;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Exceptions;
using System.Xml.Linq;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Category;

public class CategoryTests
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description",
        };

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description);

        var datetimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.NotEqual(default, category.Id);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default, category.CreatedAt);
        Assert.True(category.CreatedAt > datetimeBefore);
        Assert.True(category.CreatedAt < datetimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description",
        };

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);

        var datetimeAfter = DateTime.Now;

        Assert.NotNull(category);
        Assert.NotEqual(default, category.Id);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default, category.CreatedAt);
        Assert.Equal(isActive, category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void InstantiateErrorWhenNameIsEmptyOrNull(string? name)
    {
        var exception = Assert.Throws<EntityValidationException>(() => new DomainEntity.Category(name!, "category description"));
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var exception = Assert.Throws<EntityValidationException>(() => new DomainEntity.Category("Category name", null!));
        Assert.Equal("Description should not be null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void InstantiateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        var exception = Assert.Throws<EntityValidationException>(() => new DomainEntity.Category(invalidName, "category description"));
        Assert.Equal("Name should have at least 3 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenNameHasMoreThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var exception = Assert.Throws<EntityValidationException>(() => new DomainEntity.Category(invalidName, "category description"));
        Assert.Equal("Name should not have more than 255 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        var exception = Assert.Throws<EntityValidationException>(() => new DomainEntity.Category("category name", invalidDescription));
        Assert.Equal("Description should not have more than 10_000 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Activate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description",
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        category.Activate();

        Assert.True(category.IsActive);
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Deactivate()
    {
        var validData = new
        {
            Name = "category name",
            Description = "category description",
        };

        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        category.Deactivate();

        Assert.False(category.IsActive);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregate")]
    public void Update()
    {
        var category = new DomainEntity.Category("category name", "category description");

        var newValues = new
        {
            Name = "New name",
            Description = "New Description",
        };

        category.Update(newValues.Name, newValues.Description);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(newValues.Description, category.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateOnlyName()
    {
        var category = new DomainEntity.Category("category name", "category description");
        var currentDescription = category.Description;

        var newValues = new
        {
            Name = "New name",
        };

        category.Update(newValues.Name);

        Assert.Equal(newValues.Name, category.Name);
        Assert.Equal(currentDescription, category.Description);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void UpdateErrorWhenNameIsEmptyOrNull(string? name)
    {
        var category = new DomainEntity.Category("category name", "category description");
        var exception = Assert.Throws<EntityValidationException>(() => category.Update(name!));
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void UpdateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        var category = new DomainEntity.Category("category name", "category description");
        var exception = Assert.Throws<EntityValidationException>(() => category.Update(invalidName));
        Assert.Equal("Name should have at least 3 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateErrorWhenNameHasMoreThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var category = new DomainEntity.Category("category name", "category description");
        var exception = Assert.Throws<EntityValidationException>(() => category.Update(invalidName));
        Assert.Equal("Name should not have more than 255 characters", exception.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        var category = new DomainEntity.Category("category name", "category description");
        var exception = Assert.Throws<EntityValidationException>(() => category.Update("new category name", invalidDescription));
        Assert.Equal("Description should not have more than 10_000 characters", exception.Message);
    }
}


