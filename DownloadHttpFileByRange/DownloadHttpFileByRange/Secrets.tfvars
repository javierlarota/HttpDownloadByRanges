container_env_vars = [
  { "name": "AGENT_SERVICE_REQUIRE_REGISTRATION", "value": "true" },
  { "name": "AggregationPollingIntervalInMinutes", "value": "1" },
  { "name": "TenantServiceBaseUrl", "value": "http://eureka-tenant-service.eureka-dev-cluster:80" },
  { "name": "CustomerServiceBaseUrl", "value": "http://eureka-customer-service.eureka-dev-cluster:80" },
  { "name": "RunAggregation", "value": "true" }
]

memory = "1024" # 1 GB

cpu = 512 # 0.5 vCPU
