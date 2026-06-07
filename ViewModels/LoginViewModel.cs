using System.ComponentModel.DataAnnotations;

namespace AdoptPets.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string usernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}