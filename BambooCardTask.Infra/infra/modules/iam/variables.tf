variable "role_name" {
  description = "Base name of the IAM role for ECS/EKS node group. The environment will be appended."
  type        = string
  default     = "bamboocard-eks-node-group"
}

variable "environment" {
  description = "Deployment environment (dev, test, prod). Controls policy attachment and resource naming."
  type        = string
  validation {
    condition     = contains(["dev", "test", "prod"], lower(var.environment))
    error_message = "Environment must be one of: dev, test, prod."
  }
}
