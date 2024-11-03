using System.Windows;

namespace UpRentTestniZadatak.Helpers
{
    public static class MessageBoxHelper
    {
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ConfirmExitWithoutSaving()
        {
            return MessageBox.Show("Niste unijeli podatke. Želite li izaći bez unosa ?",
                            "Potvrda odabira",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
        }

        public static MessageBoxResult ConfirmSaveUnsavedChanges()
        {
            return MessageBox.Show("Imate podatke koji nisu spremljeni. Želite li ih spremiti ?",
                            "Potvrda odabira",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
        }

        public static MessageBoxResult SaveSameData()
        {
            return MessageBox.Show("Podaci su isti. Želite li ih zadržati ?",
                              "Potvrda spremanja",
                              MessageBoxButton.YesNo,
                              MessageBoxImage.Warning);
        }

        public static void UserSuccessfullyAdded()
        {
            MessageBox.Show("Korisnik uspješno dodan!");
        }

        public static void DataSuccessfullyChanged()
        {
            MessageBox.Show("Podaci uspješno promijenjeni!");
        }

        public static void ShowUserNotSelectedWarning()
        {
            MessageBox.Show("Molimo odaberite korisnika kojemu hoćete izmijeniti podatke.", "Korisnik nije selektiran",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public static void ShowUserNotFoundError()
        {
            MessageBox.Show("Korisnik nije pronađen.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ConfirmUserDeletion(int userCount)
        {
            var result = MessageBox.Show($"Jeste li sigurni želite li izbrisati korisnika {userCount} ?",
                                          "Potvrda odabira",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        public static void UserDeletionInformation(int selectedUsersCount)
        {
            MessageBox.Show($"{selectedUsersCount} korisnika je izbrisano.", "Uspješna akcija",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
        }

        public static void UserDeletionError()
        {
            MessageBox.Show("Došlo je do pogreške prilikom brisanja korisnika. Molimo pokušajte ponovo.", "Pogreška",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
        }

        public static void ShowUsersNotSelectedWarning()
        {
            MessageBox.Show("Molimo odaberite korisnike koje želite izbrisati.", "Korisnici nisu selektirani",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public static void ShowUsernameLengthError()
        {
            ShowError("Molimo unesite korisničko ime (max 20 znakova).");
        }
    }
}
