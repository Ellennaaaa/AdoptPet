using System;
using System.ComponentModel.DataAnnotations;

namespace AdoptPets.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string name { get; set; }

        [Required]
        public string surname { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password")]
        public string confirmPassword { get; set; }

        [Required]
        public string phoneNumber { get; set; }

        [Required]
        public int familyMembers { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }

        [Required]
        public int id_residence { get; set; }

        [Required]
        public int id_city { get; set; }

        [Required]
        public string address { get; set; }

        [Required]
        public int id_job { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal salary { get; set; }
    }
}