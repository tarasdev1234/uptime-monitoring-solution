# Structure

.
├── .build                       # Build output directory
│   ├── bin                      # Binaries
│   ├── obj                      # Object files
│   └── publish                  # Output directory for published services
├── src                          # Source files
├── docker                       # Compose file and configs for local development docker environment
│   ├── certificates             # Dev certificates for traefik (for localhost domain)
│   ├── configs                  # Config folder
│   ├── data                     # Here runned docker services will store there data (except MS Sql Server it has some problem with such folders)
│   ├── .env                     # Environment variables for docker
│   └── docker-compose.yml       # Docker compose file
├── .dockerignore                # List of flies that will be ignoring while building docker images
├── Uptime.Monitoring.Module.sln # Solution for developing monitoring modules
└── UptimeWeb.sln                # Solution for developing web module and authority

# Build

All projects builds as general .Net Core projects. Binaries will be placed in '.build/bin' directory

# Build docker images

To generate docker image for certain project: 

1. Publish that project. 
   It can be done with Visual Studio. Necessary publish profiles already configured with name "FolderProfile".

2. Build and export docker image:
        docker build -t %project_name%:dev -f %path_to_dockerfile% . & docker save %project_name% -o %project_name%.tar

   For example:
        docker build -t reliablesite.authority:dev -f .\src\Authority\Reliablesite.Authority\Dockerfile . & docker save reliablesite.authority -o reliablesite.authority.tar
        docker build -t uptimeweb:dev -f .\src\UptimeWeb\Dockerfile . & docker save uptimeweb -o uptimeweb.tar
        docker build -t uptime.coordinator:dev -f .\src\Coordinator\Uptime.Coordinator\Dockerfile . & docker save uptime.coordinator -o uptime.coordinator.tar
        docker build -t uptime.monitoring:dev -f .\src\Monitoring\Uptime.Monitoring\Dockerfile . & docker save uptime.monitoring -o uptime.monitoring.tar
        docker build -t uptime.notifications:dev -f .\src\Notifications\Uptime.Notifications\Dockerfile . & docker save uptime.notifications -o uptime.notifications.tar

   This commands should be run from solution directory.

After this you will get tar archive that can be imported in any docker.

# Run

This solution contains multiple executable projects which can be runned as separate services.
So generally for development it is necessary to run multiple projects at once. 
For example to work with UptimeWeb you should also run Reliablesite.Authority to be able authorize into UptimeWeb.
It is possible to configure multiple startup projects in Visual Studio, or run one of the services as docker container (see "Build docker images" section).

# Services

## UptimeWeb

This service provide next functionality:
- Host most of web pages: landing page, dashboard, admin etc.
- Admin and monitoring API

Startup project UptimeWeb dynamicly load other assmeblies that provides specific functionality:
- Modules/Admin.Api - administration api
- Modules/Admin.Web - administration web
- Modules/Uptime.Plugin - monitoring api
- Themes/Uptime.Theme - monitoring web (dashboard)

## Reliablesite.Authority

Authority service based on IdentityServer4.
It hosts users personal information, user API and provide authorization through OpenID.

Reliablesite.Authority.Client project contains HTTP client for User API. It generates automatically with NSwag based on OpenAPI specification.

## Uptime.Notifications

This service provide email notification functionality.
It use Razor as render engine for mails.
Templates can be stored as .cshtml files in Uptime.Notifications.Email\Templates folder or as string in database (MS Sql Server).
Each template should be registered with provided API.

Uptime.Notifications interacts with other services through Apache Kafka. It listens topic "Notifications" and trying to send notification if any approprite message found.
For now there are two services that send messages to Uptime.Notifications service: Reliablesite.Authority (email confirmations) and Uptime.Monitoring.

## Uptime.Monitoring

This service can monitor status of different endpoints using different protocols\utils (HTTP\HTTPS requests, ping request, raw tcp connection).
It can host multiple monitoring tasks at one time, trigger them by schedule.
Each check collect information about endpoint stauts and store it in Apache Cassandra database. Multiple consecutive successfull connections will be accumulated as one record. But each unseccessfull connection will generate separate record.

On changing monitoring task state Uptime.Monitoring send message through Apache Kafka to Uptime.Coordinator.

This service listen multiple Apache Kafka topics to start\stop monitors and retry checks.
Each message for changing monitor status uses monitor id as key. So every message for conrete monitor will be consumed by specific Uptime.Monitoring service.

At startup Uptime.Monitoring service trying to connect to Uptime.Coordinator and obtain actual information about monitoring tasks assigned to current instance of Uptime.Monitoring.
It may be usefull when Uptime.Monitoring crashed.

## Uptime.Coordinator

This service manage information about started\stoped monitoring tasks. Also it handle information about crashed\stoped monitoring services consumed from Hashicorp Consul. Uptime.Coordinator provides callback endpoint which triggered every time when Consul notice that some service is down.
When it happens Uptime.Coordinator trying to start failed monitoring tasks on other services. It send message to "Notifications" topic in Apache Kafka and one of availabe Uptime.Monitoring consume it.

Uptime.Coordinator use database to store information about current activities.

Uptime.Coordinator.Client project contains HTTP client for this service API.  It generates automatically with NSwag based on OpenAPI specification.

# Requests routing

All HTTP\HTTPS request routes through Traefik. Traefik automatically read information about available services and there routing rules from Hashicorp Consul. Each service participating in routing should configure specific tags in "Consul" config section in "appsettings.json".
See https://doc.traefik.io/traefik/providers/consul-catalog/ for more information about Traefik integration with Consul.

# Creating new service

New service can be create as simple .Net Core application project.
Some base features, such as:
- Logging configuration
- Healthcheck enpoints
- OpenAPI configuration
- Endpoints configuration
- Integration with Consul
and some other can be found in Reliablesite.Service.Core and Reliablesite.Service.Model projects as extensions methods.