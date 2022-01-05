import HomePage from "components/home-page"
import Users from "components/users/user"
import Roles from "components/users/roles"
import Brands from "components/brands"
import Departments from "components/departments"
import Companies from "components/users/companies"
import PopServers from "components/email/pop"
import SmtpServers from "components/email/smtp"
import Autoresponders from "components/email/autoresponders"
import Tickets from "components/tickets/tickets"
import Articles from "components/kb/articles"
import Cats from "components/kb/cats"
import Currencies from "components/billing/currencies"
import ProductGroups from "components/billing/product-groups"
import Products from "components/billing/products"
import Themes from "components/themes"
import Plugins from "components/plugins"
import Tags from "components/kb/tags"
import UptimeServers from "components/uptime/servers"
import Dynamic from "components/dynamic"

export const routes = [
    {
        name: "home",
        path: "/",
        component: HomePage,
        display: "Home",
    }, {
        name: "user",
        path: "/user",
        component: Users,
        display: "Users",
    }, {
        name: "brands",
        path: "/brands",
        component: Brands,
        display: "Brands",
    }, {
        name: "departments",
        path: "/departments",
        component: Departments,
        display: "Departments",
    }, {
        name: "companies",
        path: "/companies",
        component: Companies,
        display: "Companies",
    }, {
        name: "popservers",
        path: "/email/pop",
        component: PopServers,
        display: "POP Servers",
    }, {
        name: "smtpservers",
        path: "/email/smtp",
        component: SmtpServers,
        display: "SMTP Servers",
    }, {
        name: "roles",
        path: "/roles",
        component: Roles,
        display: "Roles",
    }, {
        name: "responders",
        path: "/email/responders",
        component: Autoresponders,
        display: "Autoresponders",
    }, {
        name: "tickets",
        path: "/tickets/:status",
        props: true,
        component: Tickets,
        display: "Tickets",
    }, {
        name: "articles",
        path: "/kb/articles",
        component: Articles,
        display: "Articles",
    }, {
        name: "cats",
        path: "/kb/categories",
        component: Cats,
        display: "Categories",
    }, {
        name: "tags",
        path: "/kb/tags",
        component: Tags,
        display: "Tags",
    }, {
        name: "currencies",
        path: "/billing/currencies",
        component: Currencies,
        display: "Currencies",
    }, {
        name: "productGroups",
        path: "/billing/product-groups",
        component: ProductGroups,
        display: "Product Groups",
    }, {
        name: "products",
        path: "/billing/products",
        component: Products,
        display: "Products",
    }, {
        name: "themes",
        path: "/themes",
        component: Themes,
        display: "Themes",
    }, {
        name: "plugins",
        path: "/plugins",
        component: Plugins,
        display: "Plugins",
    }, {
        name: "uptimeservers",
        path: "/uptime/servers",
        component: UptimeServers,
        display: "Servers",
    },
    //{
    //    name: "pluginsettings",
    //    path: "/plugin/:id",
    //    component: Dynamic,
    //    display: "Servers",
    //},
]
