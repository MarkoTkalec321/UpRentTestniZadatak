using System.Windows;
using System.Windows.Controls;

namespace UpRentTestniZadatak.View.UserControls
{
    public partial class UserCreationButtons : UserControl
    {
        public event RoutedEventHandler ExitClicked;
        public event RoutedEventHandler SaveClicked;
        public event RoutedEventHandler SaveExitClicked;
        public UserCreationButtons()
        {
            InitializeComponent();
        }
        private void Exit_Click(object sender, RoutedEventArgs e) => ExitClicked?.Invoke(this, e);
        private void Save_Click(object sender, RoutedEventArgs e) => SaveClicked?.Invoke(this, e);
        private void Save_Exit(object sender, RoutedEventArgs e) => SaveExitClicked?.Invoke(this, e);
    }
}
