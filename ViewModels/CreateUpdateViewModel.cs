using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AdoptPets.ViewModels
{
    public class CreateUpdateViewModel
    {
        public int adoptionId { get; set; }

        [Required]
        public string description { get; set; }

        public HttpPostedFileBase image { get; set; }
    }
}