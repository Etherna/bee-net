# Code imported from Nethereum.Signer

Cloning this code from `Nethereum.Signer` until it will support BouncyCastle >= 2.0.  
This BouncyCastle version supports output to Span<byte>, that is a performance requirement for hashing.

Return to link the original library when possible.

Operations to do:
* Remove direct dependency to `NBitcoin.Secp256k1`, `Nethereum.Model` and `Nethereum.Util`
* Remove `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`
* Delete directory `BeeNet.Core/Nethereum` and its content
* Add back to BeeNet.Core the dependency `Nethereum.Signer`