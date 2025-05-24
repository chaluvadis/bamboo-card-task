resource "aws_ecr_repository" "this" {
  name                 = "${var.name}-${var.environment}"
  image_tag_mutability = "IMMUTABLE" # Prevents overwriting tags, best practice for security
  image_scanning_configuration {
    scan_on_push = true
  }
  encryption_configuration {
    encryption_type = "AES256" # Enable encryption at rest
  }
  force_delete = false # Prevent accidental deletion of repository with images
}

resource "aws_ecr_lifecycle_policy" "this" {
  repository = aws_ecr_repository.this.name
  policy     = <<EOF
{
  "rules": [
    {
      "rulePriority": 1,
      "description": "Expire untagged images older than 14 days",
      "selection": {
        "tagStatus": "untagged",
        "countType": "sinceImagePushed",
        "countUnit": "days",
        "countNumber": 14
      },
      "action": { "type": "expire" }
    }
  ]
}
EOF
}

