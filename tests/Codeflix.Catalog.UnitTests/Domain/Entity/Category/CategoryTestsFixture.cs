using Xunit;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Category;

public class CategoryTestsFixture
{
	public DomainEntity.Category GetValidCategory() => new("Category Name", "Category Description");
}

[CollectionDefinition(nameof(CategoryTestsFixture))]
public class CategoryTestsFixtureCollection : ICollectionFixture<CategoryTestsFixture> { }

