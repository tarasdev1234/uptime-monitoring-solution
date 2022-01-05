# Monitoring server.

Folder structure:
```
.                        
├── configs              # Service configs
│   ├── consul           
│   ├── monitoring       
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
* uptime.monitoring
* uptime.coordinator

# Configuring

## Consul client.
Next parameters in ./configs/consul/config.json should be adjusted:
* datacenter - datacenter where server is located (e.g. "ny1", "la1")
* node_name - human readable name of node (e.g. "monitor1.ny1")
* retry_join - list of all consul servers in current datacenter

Also you need specify next environment variables in ./.env file:
* CONSUL_INTERFACE - name of interface for communication between consuls nodes (default "enp1s0f1")

## Monitoring service.
Next parameters in ./configs/monitoring/appsettings.json should be adjusted:
* Service.InstanceId - service instance id, should be unique among all service instances
* ConnectionStrings.Monitoring - connection string to MS SQL Server database for current service
* ConnectionStrings.Events - connection string to Cassandra database with monitoring events
* KafkaSettings.Hosts - CSV list of Kafka servers
* CoordinatorClientSettings.Url - url of current host (e.g. "http://monitor1.ny1.uptime.engineer")
* Consul.Host - current host (e.g. "monitor1.ny1.uptime.engineer")
* Consul.Discovery.HostName - current host (e.g. "monitor1.ny1.uptime.engineer")

## Coordinator service.
Next parameters in ./configs/coordinator/appsettings.json should be adjusted:
* Service.InstanceId - service instance id, should be unique among all service instances
* ConnectionString - connection string to MS SQL Server database for current service
* KafkaSettings.Hosts - CSV list of Kafka servers
* Consul.Host - current host (e.g. "monitor1.ny1.uptime.engineer")
* Consul.Discovery.HostName - current host (e.g. "monitor1.ny1.uptime.engineer")

## Traefik
Next parameters in ./configs/traefik/traefik.yml should be adjusted:
* entryPoints.websecure.http.tls.domains.main - current host (e.g. "monitor1.ny1.uptime.engineer")
* providers.consulCatalog.endpoint.address - should be set to "http://%hostname%:8500" where %hostname% should be replaced with current hostname

Also you need specify next environment variables in ./.env file:
* CF_DNS_API_TOKEN - Cloudflare API token with DNS:Edit permission

# Start
After all configuration is done services can be started with next command started from directory with docker-compose.yml
```
docker-compose up -d
```

# Network configuration
Iptables must be configured to allow connections to Consul ports (see docker-compose.yml for ports list) only from internal netwrok of from specific servers (e.g. Consul server).
*IMPORTANT* Keep in mind that Consul do not use Docker subnet, but share host netwrok.
Direct ports for services Uptime.Coordinator and Uptime.Monitoring (see docker-compose.yml for ports list) should be avaialble only from Docker network or from other servers.
HTTP\HTTPS ports should be available from anywhere.
Also make sure that iptables allow connections to host from Docker subnet.
Please note that docker also use iptables to route request to its internal network and can bypass some other rules.