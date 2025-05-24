variable "log_group_name" {
  description = "Base name of the CloudWatch log group. The environment will be appended."
  type        = string
  default     = "/aws/eks/bamboocard-api"
}

variable "environment" {
  description = "Deployment environment (dev, test, prod). Controls log group naming."
  type        = string
  validation {
    condition     = contains(["dev", "test", "prod"], lower(var.environment))
    error_message = "Environment must be one of: dev, test, prod."
  }
}

variable "retention_in_days" {
  description = "Number of days to retain logs."
  type        = number
  default     = 30
}
