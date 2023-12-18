using Logic.Services;
using Shared.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace UI
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private readonly IngrediantService ingrediantService;
        private readonly BeverageService beverageService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Beverage> Beverages { get; }
        public ObservableCollection<Ingrediant> Ingrediants { get; }
        public ObservableCollection<Ingrediant> Recipe { get; }
        public string NewItemName { get; set; } = string.Empty;
        public string NewItemDoses { get; set; } = string.Empty;
        public Beverage SelectedBeverage { get; set; } = new();
        public int SelectedIngrediant { get; set; } = new();
        public int SelectedRecipeItem { get; set; } = new();
        public string StatusMessage { get; private set; } = string.Empty;

        public MainWindowVM(
            IngrediantService ingrediantService,
            BeverageService beverageService
            )
        {
            this.ingrediantService = ingrediantService;
            this.beverageService = beverageService;
            Beverages = new();
            Ingrediants = new();
            Recipe = new();
            LoadData();
            SetCommands();
            Notify("Data Loaded");
        }

        private void SetCommands()
        {
            IncrementIngrediant = new CustomCommand(IncrementIngrediantMethod);
            RemoveIngrediant = new CustomCommand(RemoveIngrediantMethod);
            CreateIngrediant = new CustomCommand(CreateIngrediantMethod);
            CreateBeverage = new CustomCommand(CreateBeverageMethod);
            DecrementRecipe = new CustomCommand(DecrementRecipeMethod);
            IncrementRecipe = new CustomCommand(IncrementRecipeMethod);
            RemoveBeverage = new CustomCommand(RemoveBeverageMethod);
            BrewBeverage = new CustomCommand(BrewBeverageMethod);
        }

        private void Notify(string message)
        {
            StatusMessage = message;
            OnPropertyChanged(nameof(StatusMessage));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadData()
        {
            Ingrediants.Clear();
            ingrediantService.Get().ForEach(entry=> 
            { 
                Ingrediants.Add(entry);
                var copy = new Ingrediant { Id = entry.Id, Name = entry.Name };
                Recipe.Add(copy);
            });
            OnPropertyChanged(nameof(Ingrediants));

            Beverages.Clear();
            beverageService.Get(Ingrediants.ToArray()).ForEach(Beverages.Add);
            OnPropertyChanged(nameof(Beverages));
        }

        public ICommand IncrementIngrediant { get; set; }
        public void IncrementIngrediantMethod()
        {
            var entry = SelectedIngrediant;
            ingrediantService.Increment(Ingrediants[entry]);

            OnPropertyChanged(nameof(Ingrediants));
            Notify($"Added dose of ingrediant {Ingrediants[entry].Name}");
        }
        public ICommand RemoveIngrediant { get; set; }
        public void RemoveIngrediantMethod()
        {
            var entry = SelectedIngrediant;
            Ingrediants.Remove(Ingrediants[entry]);
            ingrediantService.Remove(Ingrediants[entry]);

            OnPropertyChanged(nameof(Ingrediants));
            Notify($"Removed ingrediant {Ingrediants[entry].Name}");
        }
        public ICommand CreateIngrediant { get; set; }
        public void CreateIngrediantMethod()
        {
            if(string.IsNullOrWhiteSpace(NewItemName))
            {
                Notify($"Error: Cannot add ingrediant of undefined name");
                return;
            }
            var entry = ingrediantService.Create(name: NewItemName, doses: NewItemDoses);
            Ingrediants.Add(entry);

            OnPropertyChanged(nameof(Ingrediants));
            Notify($"Added new ingrediant {entry.Name}");
        }
        public void LoadBeverageIngrediants()
        {
            if (SelectedBeverage == null)
            {
                return;
            }
            var newRecipe = SelectedBeverage.Ingrediants.ToDictionary(x=>x.Id,x=>x);
            foreach (var ingrediant in Recipe)
            {
                ingrediant.Doses = newRecipe.ContainsKey(ingrediant.Id) ? newRecipe[ingrediant.Id].Doses : 0;
            }
            OnPropertyChanged(nameof(Recipe));
            Notify($"Displaying recipe for beverage {SelectedBeverage.Name}");
        }
        public ICommand CreateBeverage { get; set; }
        public void CreateBeverageMethod()
        {
            if (string.IsNullOrWhiteSpace(NewItemName))
            {
                Notify($"Error: Cannot add beverage of undefined name");
                return;
            }
            var entry = beverageService.Create(name: NewItemName);
            Beverages.Add(entry);
            OnPropertyChanged(nameof(Beverages));
            Notify($"Added new beverage {entry.Name}");
        }
        public ICommand DecrementRecipe { get; set; }
        public void DecrementRecipeMethod()
        {
            var ingrediant = SelectedRecipeItem;
            if (ingrediant < 0 || ingrediant >= Recipe.Count) 
            { 
                Notify($"Error: Beverage Not Selected");
                return;
            }
            beverageService.ReduceIngrediant(beverage: SelectedBeverage, ingrediant: Recipe[ingrediant], 1);
            OnPropertyChanged(nameof(Recipe));
            Notify($"Updated beverage {SelectedBeverage.Name}. Reduced the doses of ingrediant {Recipe[ingrediant].Name}");
        }
        public ICommand IncrementRecipe { get; set; }
        public void IncrementRecipeMethod()
        {
            var ingrediant = SelectedRecipeItem;
            if (ingrediant < 0 || ingrediant >= Recipe.Count)
            {
                Notify($"Error: Cannot update beverage of undefined name");
                return;
            }
            beverageService.IncreaseIngrediant(beverage: SelectedBeverage, ingrediant: Recipe[ingrediant], amount: 1);
            OnPropertyChanged(nameof(Recipe));
            Notify($"Updated beverage {SelectedBeverage.Name}. Increased the doses of ingrediant {Recipe[ingrediant].Name}");
        }
        public ICommand RemoveBeverage { get; set; }
        public void RemoveBeverageMethod()
        {
            var beverage = SelectedBeverage;
            Beverages.Remove(beverage);
            beverageService.Remove(entity: beverage);
            OnPropertyChanged(nameof(Beverages));
            Notify($"Removed beverage {SelectedBeverage.Name}");
        }
        public ICommand BrewBeverage { get; set; }
        public void BrewBeverageMethod()
        {
            if (SelectedBeverage == null)
            {
                Notify("ERROR: No Beverage Selected");
                return;
            }
            if (!SelectedBeverage.Available)
            {
                Notify($"ERROR: Cannot brew beverage {SelectedBeverage.Name}. Insufficient Ingrediants");
                return;
            }
            for (int i = 0; i < Recipe.Count; i++)
            {
                var ingrediant = Recipe[i];
                if (ingrediant.Doses == 0)
                {
                    continue;
                }
                Ingrediants[i].Doses -= ingrediant.Doses;
                ingrediantService.Update( entry: Ingrediants[i] );
            }
            LoadData();
            Notify($"Brewed beverage {SelectedBeverage.Name}");
        }
    }
}
