// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('main.js', {
    window: {
        location: {
            href: () => globalThis.window.location.href
        }
    }
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

let utf8Encode = new TextEncoder();
let array = utf8Encode.encode("Bzz over the world!");

const hash = await exports.BeeNetWasmUtil.GetHashStringAsync(
    array,
    "text/plain",
    "hello.txt"
);
console.log(hash);

document.getElementById('out').innerHTML = hash;
await dotnet.run();