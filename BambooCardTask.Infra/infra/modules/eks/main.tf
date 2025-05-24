module "eks" {
  source          = "terraform-aws-modules/eks/aws"
  version         = "20.8.4"
  cluster_name    = "${var.cluster_name}-${var.environment}"
  cluster_version = var.cluster_version
  vpc_id          = var.vpc_id
  subnet_ids      = var.subnet_ids

  eks_managed_node_groups = {
    default = {
      desired_size   = 1
      max_size       = 2
      min_size       = 1
      instance_types = [var.node_instance_type]
    }
  }

  tags = {
    Environment = var.environment
  }
}
