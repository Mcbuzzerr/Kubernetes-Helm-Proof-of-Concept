# Pre-Requisites

- [Docker](https://www.docker.com/products/docker-desktop) - containerization
- [Kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/) - kubernetes command line tool (Should be installed with minikube)
- [Helm](https://helm.sh/docs/intro/install/) - kubernetes package manager
- [Minikube](https://kubernetes.io/docs/tasks/tools/install-minikube/) - single node kubernetes cluster (for local development)
- [AWS CLI](https://aws.amazon.com/cli/) - AWS command line tool (for use with AWS EKS)
- [Eksctl](https://eksctl.io/) - AWS EKS command line tool (for use with AWS EKS)

## Setting Up Minikube (Skip if doing AWS)

```
minikube start
```

If you get a warning regarding docker and contexts after running minikube start, you can run `docker context use default` to make it go away. Or simply ignore it and everything should still work. Keyword: should.

Point our docker client to the minikube docker daemon (This only lasts for the current shell session)

Note: `minikube docker-env` will output the command to run to set the docker daemon to the minikube daemon, if your shell is not bash, you will need to run the command manually instead of using eval

```
eval $(minikube docker-env)
```

To verify that the docker daemon is pointing to the minikube daemon, run `docker images ls` and you should see a list of images that are kubernetes related.

## Setting Up AWS (Skip if doing Minikube)

To create an EKS Cluster you can use the console or eksctl

To create a cluster with eksctl, run the following command

```
eksctl create cluster --name %CLUSTER_NAME% --region %REGION% --nodegroup-name %NODEGROUP_NAME% --node-type t2.small --nodes 3 --nodes-min 1 --nodes-max 5 --managed
```

Point our kubectl client to the EKS cluster

```
aws eks update-kubeconfig --region %REGION% --name %CLUSTER_NAME%
```

Login/Connect to the ECR (Make this in the AWS console or using the AWS CLI, default settings are ok)
```
aws ecr get-login-password --region %REGION% | docker login --username AWS --password-stdin %ACCTNUM%.dkr.ecr.%REGION%.amazonaws.com/%ECR_CONTAINER_NAME%
```

## Build our docker images
```
docker build -t api-image ./WebApplication2 -f ./WebApplication2/WebApplication2/Dockerfile
docker build -t test-image ./TestApplication -f ./TestApplication/TestApplication/Dockerfile
```

Note: For minikube we don't need to push our images to a registry because we pointed our docker client to the minikube daemon so it already has access to them. For AWS EKS we need to push our images to AWS ECR.

## Push our images to AWS ECR

Tag our images
```
docker tag %IMAGE_NAME%:%VERSION% %ACCTNUM%.dkr.ecr.%REGION%.amazonaws.com/%ECR_CONTAINER_NAME%:%IMAGE_NAME%
```
Push our images
```
docker push %ACCTNUM%.dkr.ecr.%REGION%.amazonaws.com/%ECR_CONTAINER_NAME%:%IMAGE_NAME%
```

## Run helm install to install the chart


```
helm install unique-release-name ProofOfConceptHelm/
```

The deployment can have its values changed from the default (values.yaml) by using the --values or -f flag and specifying the path to which values file to use


# Test the deployment

Run the following helm command to run the test hook

```
helm test unique-release-name
```

If you want to run the tests automatically when you install the chart, you need to change the annotations in the test job templates to "post-install" instead of "test-success"

To change the filter that the filtered test job uses, change the `--filter` argument in the test job template

# Get the test results

Pod names are the image name with a random string appended to the end

Use get pods to find the name of your test job pod

```
kubectl get pods
```
Output should look something like this
```
NAME                              READY   STATUS      RESTARTS   AGE
api-deployment-6d46dfbc47-8fvgm   1/1     Running     0          33s
test-job-9plg7                    0/1     Completed   0          33s
filtered-test-job-9plg7           0/1     Completed   0          33s
```
___

Then check it with the logs command

```
kubectl logs [TEST JOB POD NAME]
```

The output should look something like this

```
Determining projects to restore...
  All projects are up-to-date for restore.
  ContainerizedXUnit -> /src/bin/Debug/net8.0/ContainerizedXUnit.dll
Test run for /src/bin/Debug/net8.0/ContainerizedXUnit.dll (.NETCoreApp,Version=v8.0)
Microsoft (R) Test Execution Command Line Tool Version 17.8.0 (x64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
Results File: /src/TestResults/_test-job-9plg7_2024-01-23_21_05_10.trx

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: < 1 ms - ContainerizedXUnit.dll (net8.0)
```

# Random helpful commands

To get a list of all pods, services, deployments, etc. run `kubectl get all`

To throw away the current minikube cluster to start fresh, run `minikube delete`

To access the minikube cluster from outside run `minikube tunnel` (This requires a load balancer to be set up in the cluster)

To throw away the helm release, run `helm delete unique-release-name`

To update the helm release, run `helm upgrade unique-release-name YourHelmDirectory/`