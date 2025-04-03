using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618, CS9264

namespace Nethereum.Signer.Crypto
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public class ECDSASignature
    {
        private const string InvalidDERSignature = "Invalid DER signature";

        public ECDSASignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        public ECDSASignature(BigInteger[] rs)
        {
            R = rs[0];
            S = rs[1];
        }

        public ECDSASignature(byte[] derSig)
        {
            try
            {
                var decoder = new Asn1InputStream(derSig);
                var seq = decoder.ReadObject() as DerSequence;
                if (seq == null || seq.Count != 2)
                    throw new FormatException(InvalidDERSignature);
                R = ((DerInteger) seq[0]).Value;
                S = ((DerInteger) seq[1]).Value;
            }
            catch (Exception ex)
            {
                throw new FormatException(InvalidDERSignature, ex);
            }
        }

        public BigInteger R { get; }

        public BigInteger S { get; }

        public byte[] V { get; set; }

        public bool IsLowS => S.CompareTo(ECKey.HALF_CURVE_ORDER) <= 0;

        public static ECDSASignature FromDER(byte[] sig)
        {
            return new ECDSASignature(sig);
        }

        public static bool IsValidDER(byte[] bytes)
        {
            try
            {
                FromDER(bytes);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Enforce LowS on the signature
        /// </summary>
        public ECDSASignature MakeCanonical()
        {
            if (!IsLowS)
                return new ECDSASignature(R, ECKey.CURVE_ORDER.Subtract(S));
            return this;
        }

        /**
        * What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream
        * of the type used by Bitcoin we have to encode them using DER encoding, which is just a way to pack the two
        * components into a structure.
        */

        public byte[] ToDER()
        {
            // Usually 70-72 bytes.
            var bos = new MemoryStream(72);
            using (var seq = new DerSequenceGenerator(bos))
            {
                seq.AddObject(new DerInteger(R));
                seq.AddObject(new DerInteger(S));
            }

            return bos.ToArray();
        }
    }
}