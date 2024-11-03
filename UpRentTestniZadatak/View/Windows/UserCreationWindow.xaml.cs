using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using UpRentTestniZadatak.Helpers;
using UpRentTestniZadatak.Model.Entities;
using UpRentTestniZadatak.ViewModel;
using User = UpRentTestniZadatak.Model.Entities.User;

namespace UpRentTestniZadatak
{
    public partial class UserCreationWindow : Window
    {
        private readonly AppDbContext _context;
        public string Mode { get; }
        private User _currentUser = new User();
        public ObservableCollection<RoleDataViewModel> Roles { get; set; }

        public UserCreationWindow(AppDbContext context, string mode)
        {
            InitializeComponent();
            _context = context;
            Mode = mode;

            LoadRolesFromDb();

            DataContext = this;
        }

        private void LoadRolesFromDb()
        {
            var rolesFromDb = _context.Role
                .Where(r => r.Visible)
                .Select(r => new RoleDataViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Checked = false
                })
                .ToList();

            Roles = new ObservableCollection<RoleDataViewModel>(rolesFromDb);
            DataGridRoles.ItemsSource = Roles;
        }

        public void LoadUser(User user)
        {
            _currentUser = user;
            usernameTextBox.Text = user.Username;

            LoadUserRolesFromDb(user);

        }

        private void LoadUserRolesFromDb(User user)
        {
            foreach (var role in Roles)
            {
                role.Checked = _context.UserRole
                    .Any(ur => ur.UserId == user.UserId && ur.RoleId == role.RoleId);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isSaveExitButtonClicked = sender == this;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Mode == "Add user")
                    {
                        if (IsUsernameNullOrEmpty() || IsUsernameLengthValid())
                        {
                            MessageBoxHelper.ShowUsernameLengthError();
                            return;
                        }

                        CreateAndAddUserToDb();
                        CreateAndAddUserRoleToDb();

                        _context.SaveChanges();
                        transaction.Commit();

                        MessageBoxHelper.UserSuccessfullyAdded();

                        if (isSaveExitButtonClicked)
                        {
                            CloseUserCreationOpenMainWindow();
                        }
                    }

                    if (Mode == "Edit user")
                    {
                        if (IsUsernameNullOrEmpty() || IsUsernameLengthValid())
                        {
                            MessageBoxHelper.ShowUsernameLengthError();
                            return;
                        }

                        if (HasUserChangedData() == true && !IsUsernameNullOrEmpty())
                        {
                            UpdateAndAddUserDataToDb();

                            _context.SaveChanges();

                            ClearExistingRoles();
                            CreateAndAddUserRoleToDb();

                            _context.SaveChanges();
                            transaction.Commit();

                            MessageBoxHelper.DataSuccessfullyChanged();

                            if (isSaveExitButtonClicked)
                            {
                                CloseUserCreationOpenMainWindow();
                            }
                        }
                        else
                        {
                            transaction.Rollback();
                            if (MessageBoxHelper.SaveSameData() == MessageBoxResult.Yes)
                            {
                                CloseUserCreationOpenMainWindow();
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Debug.WriteLine("An error occurred while saving data: " + ex.Message);
                }
            }
        }

        private bool IsUsernameNullOrEmpty()
        {
            return string.IsNullOrWhiteSpace(usernameTextBox.Text);
        }

        private bool IsUsernameLengthValid()
        {
            return usernameTextBox.Text.Length > 20;
        }

        private void CreateAndAddUserToDb()
        {
            _currentUser = new User
            {
                Username = usernameTextBox.Text,
                CreatedDate = DateTime.Now,
                CreatedByUserId = 1,
                ModifiedByUserId = 1,
                Locked = false,
                Visible = true,
            };
            _context.User.Add(_currentUser);
            _context.SaveChanges();
        }

        private void CreateAndAddUserRoleToDb()
        {
            foreach (var role in Roles)
            {
                if (role.Checked)
                {
                    var userRole = new UserRole
                    {
                        UserId = _currentUser.UserId,
                        RoleId = role.RoleId,
                        Comment = "Comment",
                        CreatedByUserId = 1,
                        CreatedDate = DateTime.Now,
                        Locked = false,
                        Visible = true,
                        Version = 1,
                    };
                    _context.UserRole.Add(userRole);
                }
            }
        }

        private List<int> GetUserRolesFromDatabase()
        {
            return _context.UserRole
                .Where(ur => ur.UserId == _currentUser.UserId)
                .Select(ur => ur.RoleId)
                .OrderBy(id => id)
                .ToList();
        }

        private List<int> GetSelectedRoles()
        {
            return Roles
                .Where(r => r.Checked)
                .Select(r => r.RoleId)
                .OrderBy(id => id)
                .ToList();
        }

        private bool HasUserChangedData()
        {
            return _currentUser.Username != usernameTextBox.Text ||
                   !GetUserRolesFromDatabase().SequenceEqual(GetSelectedRoles());
        }

        private void UpdateAndAddUserDataToDb()
        {
            _currentUser.Username = usernameTextBox.Text;
            _currentUser.ModifiedDate = DateTime.Now;
            _context.User.Update(_currentUser);
        }

        private void ClearExistingRoles()
        {
            var existingRoles = _context.UserRole.Where(ur => ur.UserId == _currentUser.UserId).ToList();
            _context.UserRole.RemoveRange(existingRoles);
        }

        private void CloseUserCreationOpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var newRoles = Roles.Where(r => r.Checked).Select(r => r.RoleId).ToList();

            if (Mode == "Add user")
            {
                if (IsUsernameNullOrEmpty() && !newRoles.Any())
                {
                    if (MessageBoxHelper.ConfirmExitWithoutSaving() == MessageBoxResult.No)
                    {
                        Save_Click(sender, e);
                    }
                    else
                    {
                        CloseUserCreationOpenMainWindow();
                    }
                }
                else
                {
                    if (HasUserChangedData())
                    {
                        if (MessageBoxHelper.ConfirmSaveUnsavedChanges() == MessageBoxResult.Yes)
                        {
                            if (IsUsernameNullOrEmpty() || IsUsernameLengthValid())
                            {
                                MessageBoxHelper.ShowUsernameLengthError();
                                return;
                            }
                            else
                            {
                                Save_Click(sender, e);

                                CloseUserCreationOpenMainWindow();
                            }
                        }
                        else
                        {
                            CloseUserCreationOpenMainWindow();
                        }
                    }
                    else
                    {
                        CloseUserCreationOpenMainWindow();

                    }
                }
            }
            
            if (Mode == "Edit user")
            {
                if (IsUsernameNullOrEmpty() || IsUsernameLengthValid())
                {
                    MessageBoxHelper.ShowUsernameLengthError();
                    return;
                }

                if (HasUserChangedData() == true && !IsUsernameNullOrEmpty())
                {

                    if (MessageBoxHelper.ConfirmSaveUnsavedChanges() == MessageBoxResult.Yes)
                    {
                        if (IsUsernameNullOrEmpty() || IsUsernameLengthValid())
                        {
                            MessageBoxHelper.ShowUsernameLengthError();
                            return;
                        }
                        else
                        {
                            Save_Click(sender, e);

                            CloseUserCreationOpenMainWindow();
                        }
                    }
                    else
                    {
                        CloseUserCreationOpenMainWindow();
                    }
                }
                else
                {
                    CloseUserCreationOpenMainWindow();

                }
            }

        }

        private void Save_Exit(object sender, RoutedEventArgs e)
        {  
            if (IsUsernameNullOrEmpty() || IsUsernameLengthValid())
            {
                MessageBoxHelper.ShowUsernameLengthError();
                return;
            }
            else
            {
                Save_Click(this, e);
            }
        }
    }
}
