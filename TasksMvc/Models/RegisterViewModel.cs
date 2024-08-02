using System.ComponentModel.DataAnnotations;

namespace TasksMvc.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field needs to be a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
