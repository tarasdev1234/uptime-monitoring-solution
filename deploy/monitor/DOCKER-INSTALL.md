This document based on next article:
https://docs.docker.com/engine/install/ubuntu/

# Installing docker
1. Add Docker’s official GPG key:
```
$ curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
```

2. Adding docker stable repository:
```
$ sudo add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) \
   stable"
```

3. Update package index
```
$ sudo apt-get update
```

4. Installing docker engine:
```
sudo apt-get install docker-ce docker-ce-cli containerd.io
```

# Installing docker-compose
```
$ sudo curl -L "https://github.com/docker/compose/releases/download/1.27.4/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
$ sudo chmod +x /usr/local/bin/docker-compose
```

# Loading additional docker images
You can find additional docker images that must be loaded in ./images folder.
To load images just run script ./images/load-images.sh