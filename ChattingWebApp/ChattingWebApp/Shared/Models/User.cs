using System.ComponentModel.DataAnnotations;

namespace ChattingWebApp.Shared.Models
{
    public partial class User
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Nickname { get; set; }

        [Required]
        [StringLength(maximumLength: 8000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must have at least one uppercase, one lowcase letter and a number")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string RepeatPassword { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
