# Web server.

Folder structure:
```
.                        
├── configs              # Service configs
│   ├── authority
│   ├── consul           
│   ├── notifications       
│   └── ...              
├── data                 # Persistent data storage
│   ├── consul           
│   └── ...              
├── images               # Necessary docker images
├── docker-compose.yml   
├── .env                 # Environment variables for docker-compose.yml
└── README.md            # This file
```

*IMPORTANT* Before first start please make sure that all subfolders in ./data are empty.

First of all Docker should be installed on server and custom docker images must be loaded. See DOCKER-INSTALL.md.

All necessary services are deployed as docker containers.
For most services there are config files in corresponding subdirectory in ./configs folder.
For example Consul's configs are located in ./configs/consul folder.

Before starting services some adjustment in config files should be made.
All changes in config file should be applied to corresponding files in ./configs.

# Docker images
To see how to build docker images go to README.md file in project repository root.
All necessary images should be uploaded into images folder. Uploaded images can be imported to docker with script ./images/load-images.sh

## Necessary images:
* uptimeweb
* reliablesite.authority
* uptime.notifications

# Configuring

## Consul server.
Next parameters in ./configs/consul/config.json should be adjusted:
* datacenter - datacenter where server is located (e.g. "ny1", "la1")
* node_name - unique name of node (e.g. "web1.la1.uptime.engineer")
* retry_join - list of all consul servers in current datacenter
* retry_join_wan - list of all consul servers in other datacenters 
* encrypt - the secret key to use for encryption of Consul network traffic (see https://www.consul.io/docs/agent/options#_encrypt)

Also you need specify next environment variables in ./.env file:
* CONSUL_INTERFACE - name of interface for communication between consuls nodes (default "enp1s0f1")

## Authority service.
Next parameters in ./configs/authority/appsettings.json should be adjusted:
* Service.InstanceId - service instance id, should be unique among all service instances
* ConnectionStrings.DefaultConnection - connection string to MS SQL Server database for current service
* KafkaSettings.Hosts - CSV list of Kafka servers (e.g. "kafka1.ny1.uptime.engineer:9092,kafka2.ny1.uptime.engineer:9092,kafka3.ny1.uptime.engineer:9092")
* CoordinatorClientSettings.Url - url of current host (e.g. "http://web1.la1.uptime.engineer")
* Consul.Host - current host (e.g. "web1.la1.uptime.engineer")
* Consul.Discovery.HostName - current host (e.g. "web1.la1.uptime.engineer")

## Notifications service.
Next parameters in ./configs/coordinator/appsettings.json should be adjusted:
* Service.InstanceId - service instance id, should be unique among all service instances
* ConnectionString.Templates - connection string to MS SQL Server database for current service
* KafkaSettings.Hosts - CSV list of Kafka servers (e.g. "kafka1.ny1.uptime.engineer:9092,kafka2.ny1.uptime.engineer:9092,kafka3.ny1.uptime.engineer:9092")
* SmtpSettings - smpt server connection settings for sending emails (notifications, authorization confiramtions etc.)
* Consul.Host - current host (e.g. "web1.la1.uptime.engineer")
* Consul.Discovery.HostName - current host (e.g. "web1.la1.uptime.engineer")

## Uptime web service
* ConnectionStrings.Monitoring - connection string to MS SQL Server database for current service
* ConnectionStrings.Monitoring - connection string to MS SQL Server database for monitoring service (see monitoring service settings)
* ConnectionStrings.Events - connection string to Cassandra database instances with monitoring events (should be same database as for monitoring service)

## Traefik
Next parameters in ./configs/traefik/traefik.yml should be adjusted:
* entryPoints.websecure.http.tls.domains.main - current host (e.g. "web1.la1.uptime.engineer")
* providers.consulCatalog.endpoint.address - should be set to "http://%hostname%:8500" where %hostname% should be replaced with current hostname

Next parameters in ./configs/traefik/dynamic.yml should be adjusted:
* http.middlewares.internal-ip-only.ipWhiteList.sourceRange - here should be added ip addresses of all servers that should have access to service's internal API, also be sure to add docker network range
* http.services.consul-http - should be set to "http://%hostname%:8500" where %hostname% should be replaced with current hostname
* http.services.uptimeweb - should contain url of uptimeweb service

Also you need specify next environment variables in ./.env file:
* CF_DNS_API_TOKEN - Cloudflare API token with DNS:Edit permission

# Start
After all configuration is done services can be started with next command started from directory with docker-compose.yml
```
docker-compose up -d
```

# Network configuration
Iptables must be configured to allow connections to Consul ports (see docker-compose.yml for ports list) only from internal netwrok of from specific servers (e.g. Consul clients and other Consul servers).
*IMPORTANT* Keep in mind that Consul do not use Docker subnet, but share host netwrok.
Direct ports for services Uptimeweb, Reliablesite.Authority and Uptime.Notifications (see docker-compose.yml for ports list) should be avaialble only from Docker network or from other servers.
HTTP\HTTPS ports should be available from anywhere.
Also make sure that iptables allow connections to host from Docker subnet.
Please note that docker also use iptables to route request to its internal network and can bypass some other rules.