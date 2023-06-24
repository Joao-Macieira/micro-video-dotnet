using Xunit;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestsFixture))]
public class CategoryTests
{
    private readonly CategoryTestsFixture _categoryTestsFixture;

    public CategoryTests(CategoryTestsFixture categoryTestsFixture) => _categoryTestsFixture = categoryTestsFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Instantiate()
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);

        var datetimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Id.Should().NotBe(default(Guid));
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);

        category.Should().NotBeNull();
        category.Id.Should().NotBe(default(Guid));
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
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
        var validCategory = _categoryTestsFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(name!, validCategory.Description);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(validCategory.Name, null!);
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not be null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregate")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 5)]
    public void InstantiateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should have at least 3 characters");
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTests = 6)
    {
        var fixture = new CategoryTestsFixture();

        for(int i = 0; i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;

            yield return new object[] { fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)] };
        }
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenNameHasMoreThan255Characters()
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not have more than 255 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void InstantiateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not have more than 10_000 characters");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Activate()
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Deactivate()
    {
        var validCategory = _categoryTestsFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregate")]
    public void Update()
    {
        var category = _categoryTestsFixture.GetValidCategory();

        var categoryWithNewValues = _categoryTestsFixture.GetValidCategory();

        category.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

        category.Name.Should().Be(categoryWithNewValues.Name);
        category.Description.Should().Be(categoryWithNewValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateOnlyName()
    {
        var category = _categoryTestsFixture.GetValidCategory();
        var currentDescription = category.Description;

        var newName = _categoryTestsFixture.GetValidCategoryName();

        category.Update(newName);

        category.Name.Should().Be(newName);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void UpdateErrorWhenNameIsEmptyOrNull(string? name)
    {
        var category = _categoryTestsFixture.GetValidCategory();
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
        var category = _categoryTestsFixture.GetValidCategory();
        Action action = () => category.Update(invalidName);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should have at least 3 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateErrorWhenNameHasMoreThan255Characters()
    {
        var invalidName = _categoryTestsFixture.Faker.Lorem.Letter(256);
        var category = _categoryTestsFixture.GetValidCategory();
        Action action = () => category.Update(invalidName);
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not have more than 255 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregate")]
    public void UpdateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        var invalidDescription = _categoryTestsFixture.Faker.Commerce.ProductDescription();

        while (invalidDescription.Length <= 10_000)
            invalidDescription = $"{invalidDescription} {_categoryTestsFixture.Faker.Commerce.ProductDescription()}";

        var category = _categoryTestsFixture.GetValidCategory();
        Action action = () => category.Update("new category name", invalidDescription);
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not have more than 10_000 characters");
    }
}


