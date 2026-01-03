using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncryptionService : IEncryptionService {
    public string Encrypt(string plainText, string key) {
        using (Aes aes = Aes.Create()) {
            aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            aes.GenerateIV();
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream()) {
                ms.Write(aes.IV, 0, aes.IV.Length); // IV'yi baþa yaz
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
    public string Decrypt(string cipherText, string key) {
        byte[] fullBytes = Convert.FromBase64String(cipherText);
        using (Aes aes = Aes.Create()) {
            aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            byte[] iv = new byte[aes.IV.Length];
            Array.Copy(fullBytes, 0, iv, 0, iv.Length); // IV'yi baþtan al
            aes.IV = iv;
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(fullBytes, iv.Length, fullBytes.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs)) return sr.ReadToEnd();
        }
    }
}