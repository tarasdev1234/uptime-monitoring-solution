using Users.Models.Account;

namespace Users.ViewModels.Account {
    public class LogoutViewModel : LogoutInputModel {
        public bool ShowLogoutPrompt { get; set; }
    }
}
