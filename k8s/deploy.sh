#!/bin/bash

# az login
# az aks get-credentials --resource-group dotnetconfdemo --name dotnetconfdemo

kubectl apply -f helm-rbac.yaml
helm init --history-max 200 --service-account tiller --node-selectors "beta.kubernetes.io/os=linux"

# Deploy static IP Address
NODE_RESOURCE_GROUP=$(az aks show --resource-group dotnetconfdemo --name dotnetconfdemo --query nodeResourceGroup -o tsv)
echo $NODE_RESOURCE_GROUP
PUBLIC_IP=$(az network public-ip create --resource-group $NODE_RESOURCE_GROUP --name dotnetconfdemoIP --sku Standard --allocation-method static --dns-name dotnetconfdemo --query publicIp.ipAddress -o tsv)
# PUBLIC_IP=$(az network public-ip show --resource-group $NODE_RESOURCE_GROUP --name dotnetconfdemoIP --query ipAddress -o tsv)
echo $PUBLIC_IP

# Create a namespace for your ingress resources
kubectl create namespace weather

# Use Helm to deploy an NGINX ingress controller
helm install stable/nginx-ingress \
    --name weather-nginx \
    --namespace weather \
    --set controller.replicaCount=2 \
    --set controller.nodeSelector."beta\.kubernetes\.io/os"=linux \
    --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os"=linux \
    --set controller.service.loadBalancerIP="$PUBLIC_IP"
kubectl get service -l app=nginx-ingress --namespace weather

# Install Cert-Manager
./deploy-cert-manager.sh

# Create CA Cluster Issuer
kubectl apply -f cluster-issuer.yaml
kubectl apply -f certificates.yaml

# Add sceret access token
kubectl create secret generic accuweather-token \
    --from-file=accuweather-token \
    --namespace weather
kubectl describe secrets/accuweather-token --namespace weather

# Deploy weather service and ingress rule
kubectl apply -f weather.yaml
kubectl apply -f weather-ingress.yaml

# Check that certificate has been issued
kubectl describe certificate tls-secret --namespace weather