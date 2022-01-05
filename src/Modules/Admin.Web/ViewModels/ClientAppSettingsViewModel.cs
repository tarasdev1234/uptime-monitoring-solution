namespace Admin.Web.ViewModels {
    public class ClientAppSettingsViewModel {
        public string AuthUrl { get; set; }

        public string ApiUrl { get; set; }

        public string AppUrl { get; set; }

        public string ClientPortalUrl { get; set; }

        public string IdToken { get; set; }

        public string AccessToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public string EmailHash { get; set; }
    }
}
