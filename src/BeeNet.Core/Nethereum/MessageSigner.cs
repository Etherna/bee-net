using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using Nethereum.Model;
using System.Diagnostics.CodeAnalysis;

namespace Nethereum.Signer
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public class MessageSigner
    {
        public virtual string EcRecover(byte[] hashMessage, string signature)
        {
            var ecdaSignature = ExtractEcdsaSignature(signature);
            return EthECKey.RecoverFromSignature(ecdaSignature, hashMessage).GetPublicAddress();
        }

        public virtual string EcRecover(byte[] hashMessage, EthECDSASignature signature)
        {
            return EthECKey.RecoverFromSignature(signature, hashMessage).GetPublicAddress();
        }

        public byte[] Hash(byte[] plainMessage)
        {
           return new DefaultMessageHasher().Hash(plainMessage);
        }

        public virtual string HashAndEcRecover(string plainMessage, string signature)
        {
            return EcRecover(Hash(Encoding.UTF8.GetBytes(plainMessage)), signature);
        }

        public string HashAndSign(string plainMessage, string privateKey)
        {
            return HashAndSign(Encoding.UTF8.GetBytes(plainMessage), new EthECKey(privateKey.HexToByteArray(), true));
        }

        public string HashAndSign(byte[] plainMessage, string privateKey)
        {
            return HashAndSign(plainMessage, new EthECKey(privateKey.HexToByteArray(), true));
        }

        public virtual string HashAndSign(byte[] plainMessage, EthECKey key)
        {
            var hash = Hash(plainMessage);
            var signature = key.SignAndCalculateV(hash);
            return CreateStringSignature(signature);
        }

        public string Sign(byte[] message, string privateKey)
        {
            return Sign(message, new EthECKey(privateKey.HexToByteArray(), true));
        }

        public virtual string Sign(byte[] message, EthECKey key)
        {
            var signature = key.SignAndCalculateV(message);
            return CreateStringSignature(signature);
        }

        public virtual EthECDSASignature SignAndCalculateV(byte[] message, byte[] privateKey)
        {
            return new EthECKey(privateKey, true).SignAndCalculateV(message);
        }

        public virtual EthECDSASignature SignAndCalculateV(byte[] message, string privateKey)
        {
            return new EthECKey(privateKey.HexToByteArray(), true).SignAndCalculateV(message);
        }

        public virtual EthECDSASignature SignAndCalculateV(byte[] message, EthECKey key)
        {
            return key.SignAndCalculateV(message);
        }

        private static string CreateStringSignature(EthECDSASignature signature)
        {
            return signature.CreateStringSignature();
        }

        public static EthECDSASignature ExtractEcdsaSignature(string signature)
        {
            return EthECDSASignatureFactory.ExtractECDSASignature(signature);
        }
    }
}