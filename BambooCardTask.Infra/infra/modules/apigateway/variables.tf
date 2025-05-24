variable "backend_url" {
  description = "The HTTP backend URL or service endpoint for API Gateway integration (e.g., EKS service endpoint)."
  type        = string
}
variable "name" {
  description = "Base name of the API Gateway HTTP API. The environment will be appended."
  type        = string
  default     = "bamboocard-api-gw"
}

variable "environment" {
  description = "Deployment environment (dev, test, prod). Controls API Gateway naming."
  type        = string
  validation {
    condition     = contains(["dev", "test", "prod"], lower(var.environment))
    error_message = "Environment must be one of: dev, test, prod."
  }
}
