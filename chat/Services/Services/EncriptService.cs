using Chat.Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Chat.Services.Services
{
    public class EncriptService
    {
        private static readonly string Key = "0123456789abcdef0123456789abcdef"; // 32 ký tự
        // Mã hóa tin nhắn
        public static string EncryptMessage(string message)
        {
            using (var aesAlg = new AesGcm(Encoding.UTF8.GetBytes(Key)))
            {
                byte[] nonce = new byte[12];  // Nonce là một số ngẫu nhiên (12 byte là chuẩn cho AES-GCM)
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(nonce);
                }

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] ciphertext = new byte[messageBytes.Length];
                byte[] tag = new byte[16];  // Mã xác thực (tag 16 byte)

                aesAlg.Encrypt(nonce, messageBytes, ciphertext, tag, null);

                // Ghép nối nonce, ciphertext và tag lại với nhau để lưu vào cơ sở dữ liệu
                byte[] result = new byte[nonce.Length + ciphertext.Length + tag.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
                Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
                Buffer.BlockCopy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);

                return Convert.ToBase64String(result);
            }
        }

        // Giải mã tin nhắn
        public static string DecryptMessage(string encryptedMessage)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedMessage);

            byte[] nonce = new byte[12];
            byte[] ciphertext = new byte[cipherBytes.Length - 28];  // Kích thước tag là 16 bytes, nonce là 12 bytes
            byte[] tag = new byte[16];

            Buffer.BlockCopy(cipherBytes, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(cipherBytes, nonce.Length, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(cipherBytes, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

            using (var aesAlg = new AesGcm(Encoding.UTF8.GetBytes(Key)))
            {
                byte[] decryptedMessage = new byte[ciphertext.Length];
                aesAlg.Decrypt(nonce, ciphertext, tag, decryptedMessage, null);
                return Encoding.UTF8.GetString(decryptedMessage);
            }
        }
    }
}
