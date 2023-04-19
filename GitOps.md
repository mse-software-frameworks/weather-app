## GitOps Notes

>  Tested on WSL2

Install 

```
curl -sfL https://get.k3s.io | sh -
```

Start

```
sudo k3s server 
```

Check

>  Use `kubectl` without sudo
>
> ```
> sudo chmod 644 /etc/rancher/k3s/k3s.yaml
> ```

```
kubectl cluster-info && kubectl get nodes && kubectl get pods --all-namespaces
```

### Kubernetes Dashboard

Install

```
GITHUB_URL=https://github.com/kubernetes/dashboard/releases
VERSION_KUBE_DASHBOARD=$(curl -w '%{url_effective}' -I -L -s -S ${GITHUB_URL}/latest -o /dev/null | sed -e 's|.*/||')
sudo k3s kubectl create -f https://raw.githubusercontent.com/kubernetes/dashboard/${VERSION_KUBE_DASHBOARD}/aio/deploy/recommended.yaml
```

Dashboard RBAC Configuration & deployment

```
cd infrastructure
sudo k3s kubectl create -f dashboard.admin-user.yml -f dashboard.admin-user-role.yml
```

Obtain the Bearer Token

```
sudo k3s kubectl -n kubernetes-dashboard create token admin-user
```

Create channel

```
sudo k3s kubectl proxy
```

The Dashboard is now accessible at:

- http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/
- `Sign In` with the `admin-user` Bearer Token

![image-20230417192708488](.img/image-20230417192708488.png)

### Build Containers for Weather Producer & Weather Backend

```
cd WeatherApp
docker build -t weather-producer-image -f Dockerfile.producer .
```

> Remove `WeatherApp.Backend/data` before running `docker build`

```
docker build -t weather-backend-image -f Dockerfile.backend .
```



### Translate Docker Compose to Kubernetes Resources

Install Kompose

```
curl -L https://github.com/kubernetes/kompose/releases/download/v1.26.0/kompose-linux-amd64 -o kompose
chmod +x kompose
sudo mv ./kompose /usr/local/bin/kompose
```

Convert `docker-compose`

```
cd infrastructure
kompose convert
```

```
kubectl apply -f .
```

> Some manual modifications were made to the generated service files as some (internal) port were missing that are required for proper functionality.



Test if cluster is running but first proxy `kafka ui` to localhost

```
kubectl port-forward deployment/kafka-ui 8080:8080
```

Access UI at http://localhost:8080/ui



## Sources

* https://docs.k3s.io/quick-start
* https://boxofcables.dev/deploying-rancher-on-k3s-on-wsl2/
* https://www.guide2wsl.com/k3s/
* https://stackoverflow.com/a/73425733
* https://docs.k3s.io/installation/kube-dashboard
* https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows
* https://stackoverflow.com/a/72928176
* https://stackoverflow.com/questions/47928827/how-to-install-rocksdb-into-ubuntu
* https://kubernetes.io/docs/tasks/configure-pod-container/translate-compose-kubernetes/