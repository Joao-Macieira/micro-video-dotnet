namespace Codeflix.Catalog.Domain.Seedwork;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity() => Id = Guid.NewGuid();
}

