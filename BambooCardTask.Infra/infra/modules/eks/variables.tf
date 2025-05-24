variable "subnet_ids" {
  description = "List of subnet IDs for the EKS cluster."
  type        = list(string)
}
variable "environment" {
  description = "Deployment environment (dev, test, prod). Controls resource naming and node group settings."
  type        = string
  validation {
    condition     = contains(["dev", "test", "prod"], lower(var.environment))
    error_message = "Environment must be one of: dev, test, prod."
  }
}

variable "cluster_name" {
  description = "EKS cluster name. Should be unique per environment."
  type        = string
}

variable "cluster_version" {
  description = "EKS Kubernetes version"
  type        = string
  default     = "1.29"
}


variable "vpc_id" {
  description = "VPC ID for EKS cluster. Must be a valid VPC ID (e.g., vpc-xxxxxx)."
  type        = string
  validation {
    condition     = can(regex("^vpc-[a-zA-Z0-9]+$", var.vpc_id))
    error_message = "vpc_id must be a valid VPC ID (e.g., vpc-xxxxxx)."
  }
}

variable "node_instance_type" {
  description = "EC2 instance type for worker nodes. Can be overridden per environment via node_group_settings."
  type        = string
  default     = "t3.medium"
}

variable "node_group_settings" {
  description = "Custom node group settings map (per environment). Example: { dev = { desired_capacity = 1, ... }, prod = { ... } }"
  type        = map(any)
  default     = {}
}
