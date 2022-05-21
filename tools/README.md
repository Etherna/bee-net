Use client generation tool
--------------------------

NSwag doesn't support multiple OpenAPI configuration files. They needs to be merged.

We are using `speccy` tool (https://github.com/wework/speccy) with these command:

```
speccy resolve -i .\openapi\Swarm.yaml -o GatewaySwarm.yaml
speccy resolve -i .\openapi\SwarmDebug.yaml -o DebugSwarm.yaml
```

How to versioning openapi/*.yaml: 
    - copy *.yaml from official https://github.com/ethersphere/bee/tree/master/openapi in tools/original-open-api
    - merge tools/original-open-api in new feature/BNET-xx-Beex.x.x



