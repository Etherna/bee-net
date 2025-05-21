using System;
using Nethereum.Model;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Math;
using System.Diagnostics.CodeAnalysis;

namespace Nethereum.Signer
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public class EthECDSASignature : ISignature
    {
        internal EthECDSASignature(BigInteger r, BigInteger s)
        {
            ECDSASignature = new ECDSASignature(r, s);
        }

        public EthECDSASignature(BigInteger r, BigInteger s, byte[] v)
        {
            ECDSASignature = new ECDSASignature(r, s);
            ECDSASignature.V = v;
        }

        internal EthECDSASignature(ECDSASignature signature)
        {
            ECDSASignature = signature;
        }

        internal EthECDSASignature(BigInteger[] rs)
        {
            ECDSASignature = new ECDSASignature(rs);
        }

        public EthECDSASignature(byte[] derSig)
        {
            ECDSASignature = new ECDSASignature(derSig);
        }

        internal ECDSASignature ECDSASignature { get; }

        public byte[] R => ECDSASignature.R.ToByteArrayUnsigned();

        public byte[] S => ECDSASignature.S.ToByteArrayUnsigned();

        public byte[] V
        {
            get => ECDSASignature.V;
            set => ECDSASignature.V = value;
        }

        public bool IsLowS => ECDSASignature.IsLowS;

        public static EthECDSASignature FromDER(byte[] sig)
        {
            return new EthECDSASignature(sig);
        }

      

        public byte[] ToDER()
        {
            return ECDSASignature.ToDER();
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

        public static string CreateStringSignature(EthECDSASignature signature)
        {
            return signature.CreateStringSignature();
        }
    }
}