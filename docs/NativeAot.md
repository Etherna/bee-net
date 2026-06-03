# Native AOT / trimming compatibility

`SwarmSdk` and `SwarmSdk.Client` are **Native AOT and trim compatible**. Both declare
`<IsAotCompatible>true</IsAotCompatible>`, and the whole surface is verified by an ahead-of-time
publish (see [Verifying](#verifying)).

This document records the constraints that keep them AOT-safe. Breaking one of them usually does
**not** fail a normal `dotnet build` (the Roslyn trim/AOT analyzers skip `[GeneratedCode]` code and
only see hand-written code), so the AOT publish below is the source of truth.

## Verifying

`test/SwarmSdk.AotCompatibility` is a console probe that exists only to be published with Native
AOT. It adds `SwarmSdk` and `SwarmSdk.Client` as `TrimmerRootAssembly`, so ILC analyzes **every**
operation in the libraries (not just what `Main` reaches), and ILC — unlike the Roslyn analyzers —
does not skip generated code. Its `Main` also runs a small smoke test (Keccak + secp256k1 signing,
and a JSON response deserialization) so the native binary proves those paths at runtime.

```bash
dotnet publish test/SwarmSdk.AotCompatibility -c Release -r <rid>   # e.g. linux-x64, win-x64, osx-arm64
./test/SwarmSdk.AotCompatibility/bin/Release/net10.0/<rid>/publish/SwarmSdk.AotCompatibility
```

The project sets `TreatWarningsAsErrors=true` / `MSBuildTreatWarningsAsErrors=true`, so any
IL2026/IL3050/IL2075/etc. warning **fails the publish**. A clean publish + an `AOT smoke test PASSED`
exit means both libraries are AOT-clean. Run this after touching cryptography, JSON serialization,
or after regenerating the NSwag clients.

CI runs exactly this on every pull request and on pushes to `main`/`dev`
(`.github/workflows/aot-compatibility.yml`), so an AOT regression fails the build.

## Constraint 1 — BouncyCastle: concrete classes only, never the factory utilities

`BouncyCastle.Cryptography` is only AOT-safe when used through concrete primitive classes. Its
string/enum-keyed factory utilities (`DigestUtilities`, `CipherUtilities`, `SignerUtilities`,
`MacUtilities`, `SecureRandom.GetInstance`, …) call `System.Enum.GetValues(Type)` in their static
constructors, which is `RequiresDynamicCode` and **throws at runtime under Native AOT**
(`TypeInitializationException`). The package also ships no `net8.0+` build and is not marked
trimmable.

The SDK already follows this: BouncyCastle is used in exactly two files —
`src/SwarmSdk/Hashing/Hasher.cs` (`new KeccakDigest(256)`) and
`src/SwarmSdk/Hashing/Signer/Secp256k1.cs` (`new ECDsaSigner(...)`, `new Sha256Digest()`,
`SecNamedCurves.GetByName("secp256k1")`, `BigInteger`, `ECAlgorithms`, …), all direct
instantiation. **When adding cryptography, instantiate the BouncyCastle primitive directly; never
route through a `*Utilities` factory or any API that ends up in `Enum.GetValues`.**

## Constraint 2 — System.Text.Json must use source generation

Reflection-based `System.Text.Json` (any `JsonSerializer.Serialize/Deserialize` overload that takes
`JsonSerializerOptions` instead of `JsonTypeInfo`) is not AOT-safe. Use a source-generated
`JsonSerializerContext`.

- Core (`SwarmSdk`) already does this: `Manifest/ManifestJsonSerializerContext.cs`.
- The hand-written `JsonConverter<T>` types in `JsonConverters/` and the `TypeConverter`s in
  `TypeConverters/` are reflection-free and stay AOT-safe (the latter are referenced only through
  `[TypeConverter(...)]` attribute metadata, never via a reflective `TypeDescriptor.GetConverter`).

## Constraint 3 — manual AOT fixes on the NSwag generated clients

NSwag does not (yet) emit a source-generation context, so the generated clients need a few manual
post-generation fixes. These **must be re-applied after every regeneration** (`nswag run …`) until
NSwag supports Native AOT, at which point they — and the `*.Aot.cs` files — can be deleted.

**Hand-written companion files (not regenerated, edit freely):**

- `Clients/Bee/BeeGeneratedClient.Aot.cs`
- `Clients/Beehive/BeehiveGeneratedClient.Aot.cs`

Each declares a `JsonSerializerContext` listing the request/response DTO "roots" (nested types are
pulled in transitively), implements the generated `UpdateJsonSerializerSettings` partial hook to add
that context to the client's `JsonSerializerOptions.TypeInfoResolverChain`, and provides a
`SerializeBody<T>` helper used by the request-serialization edit below. If new operations add new
request body or response types, add them to the `[JsonSerializable]` list.

**Edits inside the generated `*GeneratedClient.cs` files (re-apply after regeneration):**

1. Every `JsonSerializer.SerializeToUtf8Bytes(body, JsonSerializerSettings)` → `SerializeBody(body)`.
   The helper resolves the `JsonTypeInfo` from the body's **static** type `T` (not `body.GetType()`),
   so a null body still serializes as `"null"` like the original — some bodies are optional/nullable
   (e.g. `TagsPatchAsync`, `WelcomeMessagePostAsync`) and have no null guard.
2. In `ReadObjectResponseAsync<T>`, the two reflection-based deserialize calls →
   the non-generic `JsonTypeInfo` overloads:
   - `(T)JsonSerializer.Deserialize(responseText, JsonSerializerSettings.GetTypeInfo(typeof(T)))!`
   - `(T)(await JsonSerializer.DeserializeAsync(responseStream, JsonSerializerSettings.GetTypeInfo(typeof(T)), cancellationToken).ConfigureAwait(false))!`
3. On `ConvertToString(object?, CultureInfo)`, add
   `[UnconditionalSuppressMessage("Trimming", "IL2075", …)]`. This method reflects over enum fields to
   read `[EnumMember]` for query/header values; Native AOT preserves enum field metadata and
   attributes, so it resolves correctly — the attribute documents that and silences the conservative
   warning.

These overloads (`GetTypeInfo`, and the `JsonTypeInfo`-based `Serialize`/`Deserialize`) carry no
`RequiresUnreferencedCode`/`RequiresDynamicCode` annotations, so the publish stays warning-free.
