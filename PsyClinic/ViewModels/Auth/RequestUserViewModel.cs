using System.ComponentModel.DataAnnotations;

namespace PsyClinic.Api.ViewModels.Auth
{
    public class RequestUserViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(3)]
        public required string Name { get; init; }

        [Required]
        [MinLength(6)]
        public required string Password { get; init; }

        [Required]
        [Phone]
        public required string Phone { get; init; }

        [Required]
        public required string FederalRegistration { get; set; }
    }
}
