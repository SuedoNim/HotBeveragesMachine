using System.Windows;
using System.Windows.Controls;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowVM mainWindowVM)
        {
            InitializeComponent();
            DataContext = mainWindowVM;
        }

        private void TextBlock_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            bool
                condition1 = textBox != null,
                condition2 = int.TryParse(textBox?.Text + e.Text, out int val),
                condition3 = val <= 10;
            e.Handled = !(condition1
                && condition2
                && condition3);
        }

        private void BreveragesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not MainWindowVM @vm 
                || sender is not ListView)
            {
                return;
            }
            vm.LoadBeverageIngrediants();
            e.Handled = true;
        }
    }
}
