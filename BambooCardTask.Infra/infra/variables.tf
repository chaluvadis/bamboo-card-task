variable "environment" {
  description = "Deployment environment (dev, test, prod)."
  type        = string
  default     = "dev"
}
variable "aws_region" {
  description = "AWS region to deploy resources in."
  type        = string
  default     = "us-east-1"
}
