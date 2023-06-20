using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.Domain.Seedwork;

namespace Codeflix.Catalog.Domain.Entity;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true) : base()
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description ?? Description;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new EntityValidationException($"{nameof(Name)} should not be empty or null");
        }

        if (Name.Length < 3)
        {
            throw new EntityValidationException($"{nameof(Name)} should have at least 3 characters");
        }

        if (Name.Length > 255)
        {
            throw new EntityValidationException($"{nameof(Name)} should not have more than 255 characters");
        }

        if (Description == null)
        {
            throw new EntityValidationException($"{nameof(Description)} should not be null");
        }

        if (Description.Length > 10000)
        {
            throw new EntityValidationException($"{nameof(Description)} should not have more than 10_000 characters");
        }
    }
}
