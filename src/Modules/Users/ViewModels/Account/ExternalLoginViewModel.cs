using System.ComponentModel.DataAnnotations;

namespace Users.ViewModels.Account {
    public class ExternalLoginViewModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
