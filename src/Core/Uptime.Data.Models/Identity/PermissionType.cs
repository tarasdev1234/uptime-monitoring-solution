using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.Data.Models.Identity {
    public class PermissionType {
        public const long KB_ARTICLES_VIEW = 100;
        public const long KB_ARTICLES_UPDATE = 101;
        public const long KB_ARTICLES_DELETE = 102;
        public const long KB_ARTICLES_CREATE = 103;

        public const long KB_COMMENTS_VIEW = 200;
        public const long KB_COMMENTS_UPDATE = 201;
        public const long KB_COMMENTS_DELETE = 202;
        public const long KB_COMMENTS_CREATE = 203;

        public const long USERS_VIEW = 300;
        public const long USERS_UPDATE = 301;
        public const long USERS_DELETE = 302;
        public const long USERS_CREATE = 303;

        public const long TICKETS_VIEW = 400;
        public const long TICKETS_UPDATE = 401;
        public const long TICKETS_DELETE = 402;
        public const long TICKETS_CREATE = 403;

        public long Id { get; set; }

        public long SecureObjectId { get; set; }

        [JsonIgnore]
        public SecureObject SecureObject { get; set; }

        public string Name { get; set; }
    }
}
