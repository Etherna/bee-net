Use client generation tool
--------------------------

NSwag doesn't support multiple OpenAPI configuration files. They needs to be merged.

We are using `speccy` tool (https://github.com/wework/speccy) with these command:

```
speccy resolve -i .\openapi\Swarm.yaml -o GatewaySwarm.yaml
speccy resolve -i .\openapi\SwarmDebug.yaml -o DebugSwarm.yaml
```

How to versioning openapi/*.yaml



