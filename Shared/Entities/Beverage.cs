using Shared.Interfaces;

namespace Shared.Entities;

public class Beverage : IEntry
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Ingrediant> Ingrediants { get; set; }
    public bool Available { get; set; }
}
