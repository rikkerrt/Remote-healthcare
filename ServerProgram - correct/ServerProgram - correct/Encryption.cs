using System.Security.Cryptography;
using System.Text;

class encryption
{
    private static int lengthKey = 2048;
    public static (string, string) GenerateKeys()
    {
        using (RSA rsa = RSA.Create(lengthKey))
        {
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);

            return (publicKey, privateKey);
        }
    }

    public static byte[] Encrypt(string publicKey, string text)
    {
        using (RSA rsa = RSA.Create(lengthKey))
        {
            rsa.FromXmlString(publicKey);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] encryptedBytes = rsa.Encrypt(textBytes, RSAEncryptionPadding.Pkcs1);

            return encryptedBytes;
        }
    }

    public static string Decrypt(string privateKey, byte[] data)
    {
        using (RSA rsa = RSA.Create(lengthKey))
        {
            rsa.FromXmlString(privateKey);
            byte[] decryptedBytes = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

            return decryptedText;
        }
    }
}