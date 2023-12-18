using Shared.Interfaces;

namespace Shared.Entities;

public class Ingrediant : IEntry
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Doses { get; set; }
}
