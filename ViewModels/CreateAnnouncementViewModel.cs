using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AdoptPets.ViewModels
{
    public class CreateAnnouncementViewModel
    {
        [Required]
        public string animalName { get; set; }

        [Required]
        public int id_species { get; set; }

        [Required]
        public int id_gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? animalDateOfBirth { get; set; }

        [Required]
        public string description { get; set; }

        public List<int> selectedConditions { get; set; }

        public List<int> selectedVaccines { get; set; }

        public IEnumerable<HttpPostedFileBase> images { get; set; }

        public CreateAnnouncementViewModel()
        {
            selectedConditions = new List<int>();
            selectedVaccines = new List<int>();
        }
    }
}