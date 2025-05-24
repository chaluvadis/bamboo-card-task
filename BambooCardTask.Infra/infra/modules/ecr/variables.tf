variable "name" {
  description = "The base name of the ECR repository. The environment will be appended."
  type        = string
  default     = "bamboocard-api"
}

variable "environment" {
  description = "Deployment environment (dev, test, prod). Controls repository naming."
  type        = string
  validation {
    condition     = contains(["dev", "test", "prod"], lower(var.environment))
    error_message = "Environment must be one of: dev, test, prod."
  }
}
