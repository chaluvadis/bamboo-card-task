resource "aws_secretsmanager_secret" "eks_arn" {
  name        = "eks-cluster-arn-${var.environment}"
  description = "EKS cluster ARN for ${var.environment} environment."
}

resource "aws_secretsmanager_secret_version" "eks_arn" {
  secret_id     = aws_secretsmanager_secret.eks_arn.id
  secret_string = "NOT_SET" # No cluster ARN output available
}

resource "aws_secretsmanager_secret" "ecr_arn" {
  name        = "ecr-repo-arn-${var.environment}"
  description = "ECR repository ARN for ${var.environment} environment."
}

resource "aws_secretsmanager_secret_version" "ecr_arn" {
  secret_id     = aws_secretsmanager_secret.ecr_arn.id
  secret_string = module.ecr.repository_arn # Replace with your actual ECR repo ARN output
}

resource "aws_secretsmanager_secret" "iam_role_arn" {
  name        = "iam-role-arn-${var.environment}"
  description = "IAM role ARN for ${var.environment} environment."
}

resource "aws_secretsmanager_secret_version" "iam_role_arn" {
  secret_id     = aws_secretsmanager_secret.iam_role_arn.id
  secret_string = module.iam.role_arn # Replace with your actual IAM role ARN output
}
terraform {
  required_version = ">= 1.3.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 5.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

module "vpc" {
  source          = "./modules/vpc"
  name            = "bamboocard-vpc"
  cidr            = "172.31.0.0/16"
  azs             = ["us-east-1a", "us-east-1b"]
  public_subnets  = ["172.31.1.0/24", "172.31.2.0/24"]
  private_subnets = ["172.31.101.0/24", "172.31.102.0/24"]
  environment     = var.environment
}

module "ecr" {
  source      = "./modules/ecr"
  name        = "bamboocard-api"
  environment = var.environment
}

module "iam" {
  source      = "./modules/iam"
  role_name   = "bamboocard-eks-node-group"
  environment = var.environment
}

module "eks" {
  source             = "./modules/eks"
  environment        = var.environment
  cluster_name       = "bamboocard-eks"
  cluster_version    = "1.29"
  subnet_ids         = module.vpc.private_subnets
  vpc_id             = module.vpc.vpc_id
  node_instance_type = "t3.medium"
}

module "cloudwatch" {
  source            = "./modules/cloudwatch"
  log_group_name    = "/aws/eks/bamboocard-api"
  environment       = var.environment
  retention_in_days = 30
}

module "apigateway" {
  source      = "./modules/apigateway"
  name        = "bamboocard-api-gw"
  environment = var.environment
  backend_url = "http://<eks-service-endpoint>" # Replace with your EKS service endpoint
}
