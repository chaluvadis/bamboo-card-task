resource "aws_cloudwatch_log_group" "eks_logs" {
  name              = "${var.log_group_name}-${var.environment}"
  retention_in_days = var.retention_in_days
}
