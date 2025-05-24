# Prometheus & Grafana Module

This module is a placeholder. In production, use Helm provider or Kubernetes manifests to deploy Prometheus and Grafana to your EKS cluster.

Example (using Helm provider):

```
resource "helm_release" "prometheus" {
  name       = "prometheus"
  repository = "https://prometheus-community.github.io/helm-charts"
  chart      = "kube-prometheus-stack"
  version    = "56.6.2"
  namespace  = "monitoring"
}

resource "helm_release" "grafana" {
  name       = "grafana"
  repository = "https://grafana.github.io/helm-charts"
  chart      = "grafana"
  version    = "7.3.7"
  namespace  = "monitoring"
}
```

You can expand this module with variables for configuration as needed.
