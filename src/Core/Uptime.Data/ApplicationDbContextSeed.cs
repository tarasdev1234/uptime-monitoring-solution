using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data.Models;
using Uptime.Data.Models.Billing;
using Uptime.Data.Models.Branding;
using Uptime.Data.Models.Identity;
using Uptime.Data.Models.KnowledgeBase;
using Uptime.Data.Models.Support;
using Uptime.Events;

namespace Uptime.Data {
    public class ApplicationDbContextSeed {
        public async Task SeedAsync (ApplicationDbContext context, string brand, string brandUrl) {
            context.Database.OpenConnection();
            
            try {
                if (!context.TicketStatuses.Any()) {
                    context.TicketStatuses.AddRange(GetTicketStatuses());

                    await context.SaveChangesAsync();
                }

                if (!context.Brands.Any()) {
                    context.Brands.AddRange(GetBrands(brand, brandUrl));

                    await context.SaveChangesAsync();
                }

                if (!context.Tickets.Any()) {
                    context.Tickets.Add(GetDefaultTicket());

                    await context.SaveChangesAsync();
                }

                if (!context.CommentTypes.Any()) {
                    context.CommentTypes.AddRange(GetCommentTypes());

                    context.SaveChanges();
                }

                if (!context.CurrencyFormats.Any()) {
                    context.CurrencyFormats.AddRange(GetCurrencyFormats());

                    context.SaveChanges();
                }

                if (!context.Currencies.Any()) {
                    context.Currencies.AddRange(GetCurrencies());

                    context.SaveChanges();
                }

                if (!context.SecureObjects.Any()) {
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[SecureObjects] ON");

                    context.SecureObjects.AddRange(GetSecureObjects());
                    context.SaveChanges();

                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[SecureObjects] OFF");

                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[PermissionTypes] ON");

                    context.PermissionTypes.AddRange(GetPermissionTypes());
                    context.SaveChanges();

                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[PermissionTypes] OFF");
                }

//                if (!context.KbCategories.Any()) {
//                    var cat = GetDefaultKbCat();
//
//                    context.KbCategories.Add(cat);
//                    context.KbArticles.Add(GetKbArticle(cat));
//
//                    context.SaveChanges();
//                }
            } finally {
                context.Database.CloseConnection();
            }
        }

        private KbCategory GetDefaultKbCat () {
            return new KbCategory() {
                Name = "Default",
            };
        }

        private KbArticle GetKbArticle (KbCategory kbCat) {
            return new KbArticle() {
                KbCategory = kbCat,
                AuthorId = 1,
                Title = "This is the default KB article!",
                Slug = "first",
                DateCreated = DateTime.Now,
                ShowInAll = true,
                IsPublished = true,
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            };
        }

        private IEnumerable<SecureObject> GetSecureObjects () {
            return new List<SecureObject>() {
                new SecureObject() {
                    Id = SecureObject.KB_ARTICLES,
                    Name = "Knowledge Base"
                },
                new SecureObject() {
                    Id = SecureObject.KB_COMMENTS,
                    Name = "KB Comments"
                },
                new SecureObject() {
                    Id = SecureObject.TICKETS,
                    Name = "Tickets"
                },
                new SecureObject() {
                    Id = SecureObject.USERS,
                    Name = "Users"
                },
            };
        }

        private IEnumerable<PermissionType> GetPermissionTypes () {
            return new List<PermissionType>() {
                new PermissionType() { SecureObjectId = SecureObject.KB_ARTICLES, Id = PermissionType.KB_ARTICLES_CREATE, Name = "Create"},
                new PermissionType() { SecureObjectId = SecureObject.KB_ARTICLES, Id = PermissionType.KB_ARTICLES_DELETE, Name = "Delete"},
                new PermissionType() { SecureObjectId = SecureObject.KB_ARTICLES, Id = PermissionType.KB_ARTICLES_UPDATE, Name = "Update"},
                new PermissionType() { SecureObjectId = SecureObject.KB_ARTICLES, Id = PermissionType.KB_ARTICLES_VIEW, Name = "Read"},

                new PermissionType() { SecureObjectId = SecureObject.KB_COMMENTS, Id = PermissionType.KB_COMMENTS_CREATE, Name = "Create"},
                new PermissionType() { SecureObjectId = SecureObject.KB_COMMENTS, Id = PermissionType.KB_COMMENTS_DELETE, Name = "Delete"},
                new PermissionType() { SecureObjectId = SecureObject.KB_COMMENTS, Id = PermissionType.KB_COMMENTS_UPDATE, Name = "Update"},
                new PermissionType() { SecureObjectId = SecureObject.KB_COMMENTS, Id = PermissionType.KB_COMMENTS_VIEW, Name = "Read"},

                new PermissionType() { SecureObjectId = SecureObject.TICKETS, Id = PermissionType.TICKETS_CREATE, Name = "Create"},
                new PermissionType() { SecureObjectId = SecureObject.TICKETS, Id = PermissionType.TICKETS_DELETE, Name = "Delete"},
                new PermissionType() { SecureObjectId = SecureObject.TICKETS, Id = PermissionType.TICKETS_UPDATE, Name = "Update"},
                new PermissionType() { SecureObjectId = SecureObject.TICKETS, Id = PermissionType.TICKETS_VIEW, Name = "Read"},

                new PermissionType() { SecureObjectId = SecureObject.USERS, Id = PermissionType.USERS_CREATE, Name = "Create"},
                new PermissionType() { SecureObjectId = SecureObject.USERS, Id = PermissionType.USERS_DELETE, Name = "Delete"},
                new PermissionType() { SecureObjectId = SecureObject.USERS, Id = PermissionType.USERS_UPDATE, Name = "Update"},
                new PermissionType() { SecureObjectId = SecureObject.USERS, Id = PermissionType.USERS_VIEW, Name = "Read"},
            };
        }

        private IEnumerable<Currency> GetCurrencies () {
            return new List<Currency>() {
                new Currency() { Code = "USD", ConvertRate = 1.0f, FormatId = 1, Prefix = "$", Suffix = "USD", Base = true },
            };
        }

        private IEnumerable<CurrencyFormat> GetCurrencyFormats () {
            return new List<CurrencyFormat>() {
                new CurrencyFormat() { Pattern = "#.##", Culture = "en-US", Format = "12345.68" },
                new CurrencyFormat() { Pattern = "##,#.##", Culture = "en-US", Format = "12,345.68" },
                new CurrencyFormat() { Pattern = "##,#.##", Culture = "da-DK", Format = "12.345,68" },
                new CurrencyFormat() { Pattern = "##,#", Culture = "en-US", Format = "12,346" },
            };
        }

        private IEnumerable<CommentType> GetCommentTypes () {
            return new List<CommentType>() {
                new CommentType() { Name = "general", DisplayName = "General" },
            };
        }

        private Ticket GetDefaultTicket () {
            return new Ticket() {
                Subject = "Demo",
                DateOpened = DateTime.Now,
                CustomerEmail = "demo@user.com",
                StatusId = 1,
                DepartmentId = 1,
                Messages = new List<TicketMessage>() {
                    new TicketMessage() {
                        Date = DateTime.Now,
                        From = "some@email.com",
                        To = "demo@user.com",
                        Subject = "Demo",
                        Body = "Demo message content."
                    }
                },
                Events = new List<TicketEvent>() {
                    new TicketEvent() {
                        Type = TicketEvents.CREATED,
                        Date = DateTime.Now
                    }
                }
            };
        }

        private IEnumerable<Brand> GetBrands (string brand, string brandUrl) {
            return new List<Brand>() {
                new Brand() {
                    Name = brand,
                    Url = brandUrl,
                    Theme = "Uptime.Theme",
                    Departments = new List<Department>() {
                        new Department() { Name = "Billing Department" },
                        new Department() { Name = "Support Department" },
                        new Department() { Name = "Awesome Department" },
                    },
                    SmtpServer = new SmtpServer() {
                        Name = "Default Server",
                        ServerName = "smtp.gmail.com",
                        ServerPort = 465,
                        Encryption = EncryptionMethod.SSl,
                        Default = true
                    }
                }
            };
        }

        private IEnumerable<TicketStatus> GetTicketStatuses () {
            // TODO: set status constants to equal to status ids in db
            return new List<TicketStatus>() {
                new TicketStatus() { Name = "open", DisplayName = "Open", Order = 1, IsOpen = true, IsActive = true, IsLocked = false },
                new TicketStatus() { Name = "waiting", DisplayName = "Waiting", Order = 2, IsOpen = true, IsActive = false, IsLocked = false },
                new TicketStatus() { Name = "closed", DisplayName = "Closed", Order = 3, IsOpen = false, IsActive = false, IsLocked = false },
                new TicketStatus() { Name = "locked", DisplayName = "Locked", Order = 4, IsOpen = false, IsActive = false, IsLocked = true },
            };
        }
    }
}
