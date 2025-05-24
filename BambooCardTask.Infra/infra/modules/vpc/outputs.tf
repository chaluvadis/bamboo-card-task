output "vpc_id" {
  value       = module.vpc.vpc_id
  description = "The ID of the VPC."
  sensitive   = true
}

output "vpc_arn" {
  value       = module.vpc.vpc_arn
  description = "The ARN of the VPC."
  sensitive   = true
}

output "private_subnets" {
  value       = module.vpc.private_subnets
  description = "List of private subnet IDs."
  sensitive   = true
}

output "public_subnets" {
  value       = module.vpc.public_subnets
  description = "List of public subnet IDs."
  sensitive   = true
}

output "nat_gateway_ids" {
  value       = module.vpc.natgw_ids
  description = "List of NAT Gateway IDs."
  sensitive   = true
}

output "igw_id" {
  value       = module.vpc.igw_id
  description = "Internet Gateway ID."
  sensitive   = true
}
