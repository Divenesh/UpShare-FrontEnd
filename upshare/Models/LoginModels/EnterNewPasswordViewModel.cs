using System.ComponentModel.DataAnnotations;

namespace upshare.Models
{
    public class EnterNewPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(
            100,
            ErrorMessage = "Password must be at least {2} characters long",
            MinimumLength = 8
        )]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare(
            "NewPassword",
            ErrorMessage = "The password and confirmation password do not match."
        )]
        public string ConfirmPassword { get; set; }
    }
}
