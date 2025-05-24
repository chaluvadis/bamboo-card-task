resource "aws_apigatewayv2_api" "http_api" {
  name          = "${var.name}-${var.environment}"
  protocol_type = "HTTP"
}


resource "aws_apigatewayv2_stage" "default" {
  api_id      = aws_apigatewayv2_api.http_api.id
  name        = "$default"
  auto_deploy = true
}

# Example route and integration for forwarding to an HTTP backend (e.g., EKS service or external URL)
resource "aws_apigatewayv2_integration" "http_backend" {
  api_id                 = aws_apigatewayv2_api.http_api.id
  integration_type       = "HTTP_PROXY"
  integration_method     = "ANY"
  integration_uri        = var.backend_url # Pass this as a variable (e.g., your EKS service endpoint)
  payload_format_version = "1.0"
}

resource "aws_apigatewayv2_route" "proxy" {
  api_id    = aws_apigatewayv2_api.http_api.id
  route_key = "ANY /{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.http_backend.id}"
}
