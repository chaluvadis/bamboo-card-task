output "vpc_id" {
  value       = module.vpc.vpc_id
  description = "VPC ID for the EKS cluster."
}

output "private_subnets" {
  value       = module.vpc.private_subnets
  description = "Private subnets for EKS nodes."
}

output "eks_cluster_name" {
  value       = module.eks.cluster_name
  description = "EKS cluster name."
}

output "ecr_repository_url" {
  value       = module.ecr.repository_url
  description = "ECR repository URL."
}

output "apigateway_api_id" {
  value       = module.apigateway.api_id
  description = "API Gateway HTTP API ID."
}
