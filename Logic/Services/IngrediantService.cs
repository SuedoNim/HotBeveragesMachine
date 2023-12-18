using Shared.Entities;
using Shared.Providers;

namespace Logic.Services
{
    public class IngrediantService
    {
        private readonly ResourceProvider<Ingrediant> ingrediantDAL;

        public IngrediantService(
            ResourceProvider<Ingrediant> ingrediantDAL)
        {
            this.ingrediantDAL = ingrediantDAL;
        }
        public Ingrediant Create(string name, string doses)
        {
            var entry = new Ingrediant
            {
                Id = Guid.NewGuid(),
                Name = name,
                Doses = int.TryParse(doses, out int num) ? num : 0
            };
            ingrediantDAL.Create(entry);
            return entry;
        }
        public List<Ingrediant> Get()
        {
            return ingrediantDAL.Get();
        }
        public void Increment(Ingrediant entry)
        {
            entry.Doses = entry.Doses >= 10 ? 10 : entry.Doses+1;
            ingrediantDAL.Update(entry);
        }
        public void Remove(Ingrediant entry)
        {
            ingrediantDAL.Remove(entry);
        }
        public void Update(Ingrediant entry)
        {
            ingrediantDAL.Update(entry);
        }
    }
}
