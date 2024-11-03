using System.ComponentModel.DataAnnotations;
using UpRentTestniZadatak.MVVM;

namespace UpRentTestniZadatak.ViewModel
{
    public class RoleDataViewModel : ViewModelBase
    {
        private bool _checked;

        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnPropertyChanged(nameof(Checked));
                }
            }
        }
    }
}

