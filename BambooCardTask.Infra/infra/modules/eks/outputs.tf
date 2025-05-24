output "cluster_name" {
  value       = module.eks.cluster_name
  description = "The EKS cluster name."
}

output "cluster_arn" {
  value       = module.eks.cluster_arn
  description = "The EKS cluster ARN."
}

output "cluster_endpoint" {
  value       = module.eks.cluster_endpoint
  description = "The endpoint for your EKS Kubernetes API server."
}

output "cluster_certificate_authority_data" {
  value       = module.eks.cluster_certificate_authority_data
  description = "Base64 encoded certificate data required to communicate with the cluster."
}

output "oidc_provider_arn" {
  value       = module.eks.oidc_provider_arn
  description = "The ARN of the OIDC provider if enabled."
}

output "node_group_role_arn" {
  value       = module.eks.eks_managed_node_groups["default"].iam_role_arn
  description = "The ARN of the IAM role used by the EKS managed node group."
}
