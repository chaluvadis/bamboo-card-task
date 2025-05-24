output "api_id" {
  value       = aws_apigatewayv2_api.http_api.id
  description = "The API Gateway HTTP API ID."
}

output "api_endpoint" {
  value       = aws_apigatewayv2_api.http_api.api_endpoint
  description = "The endpoint URL of the API Gateway."
}
