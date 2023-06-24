using Xunit;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
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

        category.Should().NotBeNull();
        category.Id.Should().NotBe(default(Guid));
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt > datetimeBefore).Should().BeTrue();
        (category.CreatedAt < datetimeAfter).Should().BeTrue();
        category.IsActive.Should().BeTrue();
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

        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);

        category.Should().NotBeNull();
        category.Id.Should().NotBe(default(Guid));
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.CreatedAt.Should().NotBeSameDateAs(default);
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void InstantiateErrorWhenNameIsEmptyOrNull(string? name)
    {
        Action action = () => new DomainEntity.Category(name!, "category description");
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action = () => new DomainEntity.Category("Category name", null!);
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not be null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void InstantiateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        Action action = () => new DomainEntity.Category(invalidName, "category description");
        action.Should().Throw<EntityValidationException>().WithMessage("Name should have at least 3 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenNameHasMoreThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(invalidName, "category description");
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not have more than 255 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category("category name", invalidDescription);
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not have more than 10_000 characters");
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

        category.IsActive.Should().BeTrue();
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

        category.IsActive.Should().BeFalse();
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

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
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

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void UpdateErrorWhenNameIsEmptyOrNull(string? name)
    {
        var category = new DomainEntity.Category("category name", "category description");
        Action action = () => category.Update(name!);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null");
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
        Action action = () => category.Update(invalidName);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should have at least 3 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateErrorWhenNameHasMoreThan255Characters()
    {
        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var category = new DomainEntity.Category("category name", "category description");
        Action action = () => category.Update(invalidName);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not have more than 255 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        var category = new DomainEntity.Category("category name", "category description");
        Action action = () => category.Update("new category name", invalidDescription);
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not have more than 10_000 characters");
    }
}


