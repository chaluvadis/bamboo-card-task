---
## Managing Sensitive Data with Terraform Secrets

Sensitive data such as subnet IDs, IAM role ARNs, ECR repository ARNs, and EKS cluster ARNs should not be hardcoded in your Terraform code or committed to version control.

### Best Practices
- Use Terraform input variables and mark them as `sensitive = true` where appropriate.
- Use Terraform Cloud/Enterprise workspaces or a remote backend (e.g., S3 with encryption) to securely store state files.
- Use environment variables or a `.tfvars` file (excluded from version control) to provide sensitive values at runtime.
- Use AWS Secrets Manager or SSM Parameter Store for secrets, and reference them in your Terraform code.

### Example: Sensitive Variable Declaration
```hcl
variable "private_subnet_ids" {
  description = "List of private subnet IDs."
  type        = list(string)
  sensitive   = true
}

variable "iam_role_arn" {
  description = "IAM role ARN for EKS nodes."
  type        = string
  sensitive   = true
}

variable "ecr_repo_arn" {
  description = "ECR repository ARN."
  type        = string
  sensitive   = true
}

variable "eks_cluster_arn" {
  description = "EKS cluster ARN."
  type        = string
  sensitive   = true
}
```

### Example: Supplying Sensitive Data
- Use a `terraform.tfvars` file (add to `.gitignore`):
  ```hcl
  private_subnet_ids = ["subnet-xxx", "subnet-yyy"]
  iam_role_arn = "arn:aws:iam::123456789012:role/eks-node-group-role"
  ecr_repo_arn = "arn:aws:ecr:us-east-1:123456789012:repository/bamboocard-api-dev"
  eks_cluster_arn = "arn:aws:eks:us-east-1:123456789012:cluster/bamboocard-eks-dev"
  ```
- Or set as environment variables:
  ```sh
  export TF_VAR_private_subnet_ids='["subnet-xxx","subnet-yyy"]'
  export TF_VAR_iam_role_arn=arn:aws:iam::123456789012:role/eks-node-group-role
  export TF_VAR_ecr_repo_arn=arn:aws:ecr:us-east-1:123456789012:repository/bamboocard-api-dev
  export TF_VAR_eks_cluster_arn=arn:aws:eks:us-east-1:123456789012:cluster/bamboocard-eks-dev
  ```

### Output Handling
Mark outputs as sensitive to avoid accidental exposure:
```hcl
output "ecr_repo_arn" {
  value     = aws_ecr_repository.this.arn
  sensitive = true
}
```

### Further Reading
- [Terraform: Sensitive Data in State](https://developer.hashicorp.com/terraform/language/state/sensitive-data)
- [Terraform: Input Variables](https://developer.hashicorp.com/terraform/language/values/variables#sensitive-variables)
---
## Onboarding, Architecture, and CI/CD Integration (Summary)

### Onboarding Steps
1. Clone the repo and install prerequisites (Terraform, AWS CLI, kubectl, Docker).
2. Configure AWS credentials: `aws configure`.
3. Set your environment in `main.tf` (dev, test, prod).
4. Run `terraform init && terraform apply` in the `infra/` directory.
5. Build and push your Docker image to ECR (see infra/README.md for commands).
6. Deploy manifests to EKS using `kubectl`.

### Architecture Diagram
```
User/Client --> API Gateway --> EKS (Kubernetes) --> [App Pods]
ECR --> EKS (for image pulls)
EKS, API Gateway --> CloudWatch (logs/metrics)
All resources in VPC
```

### CI/CD Integration Example
See `infra/README.md` for a full GitHub Actions pipeline example.
This pipeline builds, pushes, and deploys your app to EKS on every push to `main`.

### Cloud Migration Plan

**Prerequisites:**
- AWS account with appropriate IAM permissions
- Docker installed locally
- Terraform installed
- kubectl and AWS CLI installed and configured

**Migration Steps:**
1. **Create a Docker Container for BambooCardTask.API**
    - Write a `Dockerfile` for the .NET API (use the official .NET SDK/runtime images).
    - Add health check and environment variable support in the Dockerfile.
    - Build the Docker image: `docker build -t bamboocard-api .`
    - Run and test the image locally: `docker run -p 8080:80 bamboocard-api`
    - Tag and push the image to a container registry (e.g., Amazon ECR):
        - `docker tag bamboocard-api <aws_account_id>.dkr.ecr.<region>.amazonaws.com/bamboocard-api-<environment>:latest`
        - `docker push <aws_account_id>.dkr.ecr.<region>.amazonaws.com/bamboocard-api-<environment>:latest`
2. **Deploy the container to Kubernetes**
    - Write Kubernetes manifests:
        - `deployment.yaml` (specify image, env vars, ports, readiness/liveness probes)
        - `service.yaml` (ClusterIP or LoadBalancer for API access)
        - (Optional) `ingress.yaml` for routing
    - Test locally with Minikube or Kind:
        - `kubectl apply -f deployment.yaml`
        - `kubectl apply -f service.yaml`
        - Access the API via NodePort/LoadBalancer/Ingress
3. **Deploy the Kubernetes setup to AWS EKS**
    - Use Terraform to provision an EKS cluster:
    - Write Terraform modules for VPC, ECR, EKS, IAM roles, node groups, CloudWatch, API Gateway
        - `terraform init && terraform apply`
    - Update kubeconfig: `aws eks update-kubeconfig --region <region> --name <cluster_name>`
    - Deploy manifests to EKS:
        - `kubectl apply -f deployment.yaml`
        - `kubectl apply -f service.yaml`
4. **Integrate with AWS API Gateway**
    - Deploy AWS Load Balancer Controller to EKS (if not already present).
    - Expose the API via a Kubernetes Service of type LoadBalancer or Ingress (with ALB annotations).
    - Create an API Gateway HTTP API or REST API.
    - Configure API Gateway integration with the EKS Load Balancer endpoint.
    - Test end-to-end connectivity from API Gateway to the containerized API.
5. **Horizontally Scale the API**
    - Enable metrics server in the EKS cluster (if not already enabled).
    - Write an HPA manifest (e.g., `hpa.yaml`) targeting CPU/memory utilization.
    - Apply the HPA: `kubectl apply -f hpa.yaml`
    - Test scaling by generating load and observing pod count changes.
    - Confirm traffic is distributed across pods via the Service/Load Balancer.
6. **Set Up Observability Dashboard**
    - Deploy monitoring stack:
        - Use Helm or manifests to install Prometheus and Grafana in the cluster.
        - (Or) Enable AWS CloudWatch Container Insights for EKS.
    - Configure Prometheus to scrape API metrics (add `/metrics` endpoint if needed).
    - Set up Grafana dashboards for API health, latency, error rates, and resource usage.
    - Create alerts for critical metrics (e.g., high error rate, high latency, pod restarts).
7. **Automate Everything with Terraform**
    - Write Terraform modules for:
        - ECR (container registry)
        - EKS (Kubernetes cluster)
        - VPC, subnets, security groups
        - IAM roles and policies
        - (Optional) API Gateway, monitoring resources
    - Use `terraform init`, `terraform plan`, and `terraform apply` to provision resources.
    - Store Terraform state securely (e.g., in S3 with DynamoDB locking).
    - Document all variables, outputs, and module usage for team onboarding.