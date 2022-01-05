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

# Configuring

## Zookeeper
For properly configuration you should adjust next environment variables in ./.env file:
ZOO_MY_ID - unique id of current Zookeeper instance, must be integer between 1 and 255
ZOO_SERVERS - list of ALL Zookeeper servers, including current one. This string hasn next format:
              server.<ID1>=<address1>:2888:3888;2181 server.<ID2>=<address2>:2888:3888;2181
              where:
              <ID> - id of specific Zookeeper server (see ZOO_MY_ID)
              <address1> - address of specific Zookeeper server (ip or hostname). For current server it must be set to 0.0.0.0
              multiple entries are separated by space
              For example:
              If current server has ID 1 with hostname kafka1.ny1.uptime.engineer and there are two more servers, then env variables should be next:
              ZOO_MY_ID=1
              ZOO_SERVERS=server.1=0.0.0.0:2888:3888;2181 server.2=kafka2.ny1.uptime.engineer:2888:3888;2181 server.3=kafka3.ny1.uptime.engineer:2888:3888;2181

## Kafka
For properly configuration you should adjust next environment variables in ./.env file:
HOST_NAME - current host name. E.g.: HOST_NAME=kafka1.ny1.uptime.engineer
KAFKA_BROKER_ID - unique id of current Kafka instance, must be integer
KAFKA_ZOOKEEPER_CONNECT - CSV list of Zookeeper servers. E.g.: KAFKA_ZOOKEEPER_CONNECT=kafka1.ny1.uptime.engineer:2181,kafka2.ny1.uptime.engineer:2181,kafka3.ny1.uptime.engineer:2181

# Start
After all configuration is done services can be started with next command started from directory with docker-compose.yml
```
docker-compose up -d
```

# Network configuration
Iptables must be configured to allow connections to Kafka and Zookeeper ports (see docker-compose.yml for ports list) only from internal netwrok of from specific servers (e.g. monitoring server, web server or other Kafka servers, for now Kafka used only by them).
Also make sure that iptables allow connections to host from Docker subnet.
Please note that docker also use iptables to route request to its internal network and can bypass some other rules.