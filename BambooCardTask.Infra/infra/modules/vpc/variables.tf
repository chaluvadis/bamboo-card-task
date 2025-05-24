
variable "environment" {
  description = "Deployment environment (dev, test, prod). Controls resource naming and selection."
  type        = string
  default     = "dev"
  validation {
    condition     = contains(["dev", "test", "prod"], lower(var.environment))
    error_message = "Environment must be one of: dev, test, prod."
  }
}

variable "name" {
  description = "VPC name. Defaults to 'bamboocard-vpc' if not set."
  type        = string
  default     = "bamboocard-vpc"
}

variable "cidr" {
  description = "VPC CIDR block. Must be a valid IPv4 CIDR."
  type        = string
  default     = "172.31.0.0/16"
  validation {
    condition     = can(regex("^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}/[0-9]+$", var.cidr))
    error_message = "CIDR must be a valid IPv4 CIDR block."
  }
}


variable "azs" {
  description = "List of availability zones to use for subnets."
  type        = list(string)
  default     = ["us-east-1a"]
}


variable "public_subnets" {
  description = "List of public subnet CIDRs. Must match number of AZs."
  type        = list(string)
  default     = ["172.31.0.0/16", "172.31.1.0/16"]
  validation {
    condition     = length(var.public_subnets) == length(var.azs)
    error_message = "Number of public subnets must match number of AZs."
  }
}


variable "private_subnets" {
  description = "List of private subnet CIDRs. Must match number of AZs."
  type        = list(string)
  default     = ["172.31.101.0/16", "172.31.102.0/16"]
  validation {
    condition     = length(var.private_subnets) == length(var.azs)
    error_message = "Number of private subnets must match number of AZs."
  }
}
