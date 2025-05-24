output "role_arn" {
  value       = aws_iam_role.eks_node_group.arn
  description = "The ARN of the IAM role."
}

output "role_name" {
  value       = aws_iam_role.eks_node_group.name
  description = "The name of the IAM role."
}
