using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace upshare.Models.User;

public class UserDetailsModel
{

    [Required(ErrorMessage = "ID is required")]
    public string id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date Joined is required")]
    public DateTime dateJoined { get; set; } = DateTime.Now;

    public IFormFile? profilePicture { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string firstname { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string lastname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string phoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    public string city { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    public string state { get; set; } = string.Empty;

    public string country { get; set; } = string.Empty;
}
