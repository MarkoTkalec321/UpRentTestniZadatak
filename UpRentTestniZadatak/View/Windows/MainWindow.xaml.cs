using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using UpRentTestniZadatak.Helpers;
using UpRentTestniZadatak.ViewModel;
using User = UpRentTestniZadatak.Model.Entities.User;

namespace UpRentTestniZadatak
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<UserViewModel> UserViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadUsers();
            DataContext = this;
        }

        private void LoadUsers()
        {
            var context = App.ServiceProvider.GetRequiredService<AppDbContext>();
            var users = context.User.ToList();
            UserViewModel = new ObservableCollection<UserViewModel>(users.Select(CreateUserViewModel));
            UserDataGrid.ItemsSource = UserViewModel;
        }

        private UserViewModel CreateUserViewModel(User user)
        {
            var context = App.ServiceProvider.GetRequiredService<AppDbContext>();
            var userIdToUsername = context.User.ToDictionary(u => u.UserId, u => u.Username);

            return new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                CreatedDate = user.CreatedDate,
                CreatedByUsername = userIdToUsername[user.CreatedByUserId],
                ModifiedDate = user.ModifiedDate,
                ModifiedByUsername = user.ModifiedByUserId.HasValue ? userIdToUsername[user.ModifiedByUserId.Value] : string.Empty
            };
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUserCreationWindow("Add user");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is UserViewModel selectedUserViewModel)
            {
                OpenEditUserWindow(selectedUserViewModel.UserId);
            }
            else
            {
                MessageBoxHelper.ShowUserNotSelectedWarning();
            }
        }

        private void OpenUserCreationWindow(string mode)
        {
            var userCreationWindowFactory = App.ServiceProvider.GetRequiredService<Func<string, UserCreationWindow>>();
            var userCreationWindow = userCreationWindowFactory(mode);
            userCreationWindow.Show();
            Close();
        }

        private void OpenEditUserWindow(int userId)
        {
            var context = App.ServiceProvider.GetRequiredService<AppDbContext>();
            var userToEdit = context.User.Find(userId);

            if (userToEdit != null)
            {
                var editWindow = App.ServiceProvider.GetRequiredService<Func<string, UserCreationWindow>>()("Edit user");
                editWindow.LoadUser(userToEdit);
                editWindow.Show();
                Close();
            }
            else
            {
                MessageBoxHelper.ShowUserNotFoundError();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUsers = UserDataGrid.SelectedItems.OfType<UserViewModel>().ToList();

            if (selectedUsers.Any())
            {
                if (MessageBoxHelper.ConfirmUserDeletion(selectedUsers.Count))
                {
                    SoftDeleteUsers(selectedUsers);
                }
            }
            else
            {
                MessageBoxHelper.ShowUsersNotSelectedWarning();
            }
        }

        private void SoftDeleteUsers(List<UserViewModel> selectedUsers)
        {
            var context = App.ServiceProvider.GetRequiredService<AppDbContext>();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var userViewModel in selectedUsers)
                    {
                        var userToSoftDelete = context.User.Find(userViewModel.UserId);
                        if (userToSoftDelete != null)
                        {
                            SoftDeleteUser(userToSoftDelete, context);
                        }
                    }

                    context.SaveChanges();
                    transaction.Commit();

                    MessageBoxHelper.UserDeletionInformation(selectedUsers.Count);


                    LoadUsers();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();

                    MessageBoxHelper.UserDeletionError();
                    Debug.WriteLine(ex.InnerException?.Message);
                }
            }
        }

        private void SoftDeleteUser(User user, AppDbContext context)
        {
            user.Visible = false;
            var userRoles = context.UserRole.Where(ur => ur.UserId == user.UserId).ToList();
            foreach (var userRole in userRoles)
            {
                userRole.Visible = false;
            }
        }
    }
}