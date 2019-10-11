#!/bin/bash

# az login
az group create --name dotnetconfdemo --location westeurope
az acr create --resource-group dotnetconfdemo --name dotnetconfdemo --sku Basic
az aks create --resource-group dotnetconfdemo --name dotnetconfdemo --node-count 1 --enable-addons monitoring --generate-ssh-keys --attach-acr dotnetconfdemo
az aks install-cli
az aks get-credentials --resource-group dotnetconfdemo --name dotnetconfdemo
kubectl get nodes