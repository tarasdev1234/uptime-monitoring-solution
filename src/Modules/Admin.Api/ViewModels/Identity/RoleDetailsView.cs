using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Api.ViewModels.Identity {
    public class RoleDetailsView {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<string> Permissions { get; set; }
    }
}
