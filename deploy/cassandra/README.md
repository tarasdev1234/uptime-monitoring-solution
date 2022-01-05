# Monitoring server.

Folder structure:

.                        
├── configs              # Service configs
│   ├── cassandra      
│   └── ...              
├── data                 # Persistent data storage
│   ├── cassandra           
│   └── ...              
├── docker-compose.yml   
├── .env                 # Environment variables for docker-compose.yml
└── README.md            # This file

*IMPORTANT* Before first start please make sure that all subfolders in ./data are empty.

All necessary services are deployed as docker containers.
For most services there are config files in corresponding subdirectory in ./configs folder.

Before starting services some adjustment in config files should be made.
All changes in config file should be applied to corresponding files in ./configs.

# Configuring

## Cassandra
For properly configuration you should adjust next environment variables in ./.env file:
HOST_NAME - should be set to current server host name (e.g. cassandra1.ny1.uptime.engineer)
DC - should be set to current datacenter name (e.g. ny1)
RACK - should be set to rack name within current data center
CASSANDRA_SEEDS - CSV list with seed nodes
                  For seed nodes you can select one arbitrtary node from each data center
                  For example we have 3 data centers (DC1, DC2, DC3) with 3 cassandra nodes in each
                  As seed nodes we can select node1.DC1 node1.DC2 node2.DC3
                  So CASSANDRA_SEEDS value will be next:
                  CASSANDRA_SEEDS=node1.DC1,node1.DC2,node2.DC3
CASSANDRA_DATA_DIRECTORY - directory where Cassandra will store database files

# Start
After all configuration is done services can be started with next command started from directory with docker-compose.yml
```
docker-compose up -d
```

# After start
After all instances of Cassandra has been deployed you should create workspace for Uptime.Monitoring service.
It can be done with cqlsh utility.
First of all connect to Cassandra docker container:
```
docker-compose exec cassandra /bin/bash
```
This command should be executed from directory where docker-compose.yml file placed.
Next you can create keyspace:
```
cqlsh -e "CREATE KEYSPACE uptime WITH REPLICATION = { 'class':'NetworkTopologyStrategy', 'ny1':2 }"
```
Read more about keyspace configuring here: https://docs.datastax.com/en/cql-oss/3.3/cql/cql_reference/cqlCreateKeyspace.html

# Network configuration
Iptables must be configured to allow connections to Cassandra ports (see docker-compose.yml for ports list) only from internal netwrok of from specific servers (e.g. monitoring server, web server and other Cassandra servers, for now Cassandra used only by them).
Please note that docker also use iptables to route request to its internal network and can bypass some other rules.