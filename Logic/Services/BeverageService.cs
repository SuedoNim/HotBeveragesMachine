using Shared.Entities;
using Shared.Providers;
using System.Collections.ObjectModel;

namespace Logic.Services
{
    public class BeverageService
    {
        private readonly ResourceProvider<Beverage> beverageDAL;

        public BeverageService(
            ResourceProvider<Beverage> beverageDAL)
        {
            this.beverageDAL = beverageDAL;
        }

        public List<Beverage> Get(Ingrediant[] ingrediants)
        {
            var list = beverageDAL.Get();
            list.ForEach(entry =>
            {
                entry.Available = IsBeverageAvailable(entry, ingrediants);
            });
            return list;
        }
        private static bool IsBeverageAvailable(Beverage beverage, Ingrediant[] ingrediants)
        {
            var availableIngrediants = ingrediants.ToDictionary(x => x.Id, x => x);
            return beverage.Ingrediants.All(ingrediant =>
                availableIngrediants.ContainsKey(ingrediant.Id)
                && availableIngrediants[ingrediant.Id].Doses >= ingrediant.Doses
            );
        }
        public Beverage Create(string name)
        {
            var entry = new Beverage { Id = Guid.NewGuid(), Name = name, Ingrediants = new(), Available = true };
            beverageDAL.Create(entry);
            return entry;
        }
        public void ReduceIngrediant(Beverage beverage, Ingrediant ingrediant, int amount)
        {
            ingrediant.Doses -= ingrediant.Doses <= amount ? 0 : amount;
            var refIngrediant = beverage.Ingrediants.FirstOrDefault(x => x.Id == ingrediant.Id);
            if (refIngrediant == null)
            {
                beverage.Ingrediants.Add(ingrediant);
            }
            else if (ingrediant.Doses == 0)
            {
                beverage.Ingrediants.Remove(refIngrediant);
            }
            else
            {
                refIngrediant.Doses = ingrediant.Doses;
            }
            beverageDAL.Update(beverage);
        }
        public void IncreaseIngrediant(Beverage beverage, Ingrediant ingrediant, int amount)
        {
            ingrediant.Doses += ingrediant.Doses + amount > 10 ? 0 : amount;
            var refIngrediant = beverage.Ingrediants.FirstOrDefault(x => x.Id == ingrediant.Id);
            if (refIngrediant == null)
            {
                beverage.Ingrediants.Add(ingrediant);
            }
            else if (ingrediant.Doses == 0)
            {
                beverage.Ingrediants.Remove(refIngrediant);
            }
            else
            {
                refIngrediant.Doses = ingrediant.Doses;
            }
            beverageDAL.Update(beverage);
        }
        public void Remove(Beverage entity)
        {
            beverageDAL.Remove(entity);
        }
    }
}
