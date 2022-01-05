using System;
using System.Collections.Generic;
using System.Text;

namespace Users.ViewModels {
    public class ErrorViewModel {
        public string RequestId { get; set; }
        
        public string Error { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
