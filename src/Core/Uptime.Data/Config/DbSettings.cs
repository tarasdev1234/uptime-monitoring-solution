using System;
using System.Collections.Generic;
using System.Text;

namespace Uptime.Data.Config {
    public class DbSettings {
        public string DtabaseProvider { get; set; }

        public string ConnectionString { get; set; }

        public string TablePrefix { get; set; }

        public string MigrationsAssembly { get; set; }
    }
}
