using System.Windows;
using System.Windows.Controls;

namespace UpRentTestniZadatak.View.UserControls
{
    public partial class UserActionButtons : UserControl
    {
        public event RoutedEventHandler AddClicked;
        public event RoutedEventHandler EditClicked;
        public event RoutedEventHandler DeleteClicked;

        public UserActionButtons()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddClicked?.Invoke(this, e);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, e);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(this, e);
        }
    }

}
