using System.ComponentModel.DataAnnotations;

namespace UpRentTestniZadatak.Model.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public int CreatedByUserId { get; set; }
        public int? ModifiedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Locked { get; set; }
        public bool Visible { get; set; }
        public int Version { get; set; }
    }
}
