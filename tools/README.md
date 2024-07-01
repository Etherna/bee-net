# Use client generation tool

NSwag doesn't support multiple OpenAPI configuration files. They needs to be merged.

We are using `speccy` tool (https://github.com/wework/speccy) with these command:

```shell
speccy resolve -i ./openapi/Swarm.yaml -o GatewaySwarm.yaml
```

How to version openapi/*.yaml:  
- copy *.yaml from official https://github.com/ethersphere/bee/tree/master/openapi in tools/original-open-api  
- merge tools/original-open-api in the new working branch
- update definition as required

# Use Nswag

Use Nswag to generate client code. Use NSwagStudio for Windows, or NSwag Cli with others.

NSwag Cli:

```shell
nswag run bee-api.nswag
```