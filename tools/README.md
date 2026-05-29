# Use client generation tool

NSwag doesn't support multiple OpenAPI configuration files. They need to be merged into a single `bee-openapi.yaml`.

We are using `redocly` (https://github.com/Redocly/redocly-cli) to **bundle** the files:

```shell
npx @redocly/cli bundle ./bee-openapi/Swarm.yaml -o bee-openapi.yaml
```

Do **not** use a tool that *dereferences* (fully inlines) the spec, such as the
previously used `speccy resolve`. Dereferencing flattens every `$ref` and drops the
`components/schemas` section, so NSwag emits one anonymous class per occurrence
(hundreds of duplicated `ResponseN` types). `redocly bundle` merges the files into one
document while preserving `components` and the internal `$ref`s, so NSwag generates a
single, unified named DTO per schema (e.g. `ReferenceResponse`, `NewTagResponse`).

How to version bee-openapi/*.yaml:  
- copy *.yaml from official https://github.com/ethersphere/bee/tree/master/openapi in tools/original-open-api  
- merge tools/original-open-api in the new working branch
- update definition as required

# Use Nswag

Use Nswag to generate client code. Use NSwagStudio for Windows, or NSwag Cli with others.

NSwag Cli:

```shell
nswag run bee-api.nswag
nswag run beehive-api.nswag
```