using Newtonsoft.Json;
using System;

namespace Users.Security {
    public class Permission {
        [JsonIgnore]
        public const string ClaimType = "Permission";

        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public Permission (string name, string descr, string type) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = descr ?? throw new ArgumentNullException(nameof(descr));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
