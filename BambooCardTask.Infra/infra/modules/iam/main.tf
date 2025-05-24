
locals {
  # Attach different policies based on environment
  secrets_policy = (
    var.environment == "prod" ? "arn:aws:iam::aws:policy/SecretsManagerReadWrite" :
    var.environment == "test" ? "arn:aws:iam::aws:policy/SecretsManagerReadWrite" :
    "arn:aws:iam::aws:policy/SecretsManagerReadWrite"
  )
  logs_policy = (
    var.environment == "prod" ? "arn:aws:iam::aws:policy/CloudWatchLogsReadOnlyAccess" :
    var.environment == "test" ? "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess" :
    "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess"
  )
  ecs_policy = (
    var.environment == "prod" ? "arn:aws:iam::aws:policy/AmazonECS_FullAccess" :
    var.environment == "test" ? "arn:aws:iam::aws:policy/AmazonECS_FullAccess" :
    "arn:aws:iam::aws:policy/AmazonECS_FullAccess"
  )
}

resource "aws_iam_role_policy_attachment" "ecs_task_secrets" {
  role       = aws_iam_role.eks_node_group.name
  policy_arn = local.secrets_policy
}

resource "aws_iam_role_policy_attachment" "ecs_task_logs" {
  role       = aws_iam_role.eks_node_group.name
  policy_arn = local.logs_policy
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution" {
  role       = aws_iam_role.eks_node_group.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy_attachment" "ecs_service" {
  role       = aws_iam_role.eks_node_group.name
  policy_arn = local.ecs_policy
}





resource "aws_iam_role" "eks_node_group" {
  name               = "${var.role_name}-${var.environment}"
  assume_role_policy = data.aws_iam_policy_document.ecs_assume_role_policy.json
}

data "aws_iam_policy_document" "ecs_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["ecs.amazonaws.com"]
    }
  }
}
