using System.Security.Cryptography;
using System.Text;

namespace PPE.Core;
/// <summary>
/// AES 加密解密
/// </summary>
public static class AesHelper
{

    private static string? _key;

    /// <summary>
    /// 密钥
    /// </summary>
    public static string Key
    {
        get
        {
            if (_key == null)
            {
                _key = ConfigManager.Instance().GetConfigString("CryptoKey");
            }
            if (_key == null || _key.Length < 32)
            {
                _key = HashHelper.MD5_32(Guid.NewGuid().ToString());
                ConfigManager.Instance().Update("CryptoKey", _key);
            }
            return _key!;
        }
        set { _key = value; }
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="cipherText">明文</param>
    /// <returns></returns>
    public static string Encrypt(this string cipherText)
    {
        return Encrypt(cipherText, Key);
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="cipherText">明文</param>
    /// <param name="key">密码</param>
    /// <returns></returns>
    public static string Encrypt(this string cipherText, string argKey)
    {
        return Encrypt(cipherText, argKey, argKey);
    }

    // <summary>
    /// 加密
    /// </summary>
    /// <param name="cipherText">明文</param>
    /// <param name="argKey">密码</param>
    /// <param name="argIv">向量</param>
    /// <returns></returns>
    public static string Encrypt(this string cipherText, string argKey, string argIV)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText);
        ArgumentException.ThrowIfNullOrWhiteSpace(argKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(argIV);
        byte[] encrypted;
        using (Aes enAes = Aes.Create())
        {
            var keyBytes = Encoding.UTF8.GetBytes(HashHelper.MD5_32(argKey)!);
            enAes.Key = keyBytes;
            enAes.IV = Encoding.UTF8.GetBytes(HashHelper.MD5_16(argIV));
            ICryptoTransform encryptor = enAes.CreateEncryptor(enAes.Key, enAes.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(cipherText);
                    }
                    encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="cipherText">密文</param>
    /// <returns></returns>
    public static string Decrypt(this string cipherText) => Decrypt(cipherText, Key);

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="cipherText">密文</param>
    /// <param name="argKey">密钥</param>
    /// <returns></returns>
    public static string Decrypt(this string cipherText, string argKey) => Decrypt(cipherText, argKey, argKey);

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="cipherText">密文</param>
    /// <param name="argKey">密钥</param>
    /// <param name="argIV">向量</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string Decrypt(this string cipherText, string argKey, string argIV)
    {
        if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException(nameof(cipherText));
        if (argKey == null || argKey.Length <= 0) throw new ArgumentNullException(nameof(argKey));
        if (argIV == null || argIV.Length <= 0) throw new ArgumentNullException(nameof(argIV));
        byte[] decrypted = Convert.FromBase64String(cipherText);
        string originalText = string.Empty;
        using (Aes aesAlg = Aes.Create())
        {
            var keyBytes = Encoding.UTF8.GetBytes(HashHelper.MD5_32(argKey));
            aesAlg.Key = keyBytes;
            aesAlg.IV = Encoding.UTF8.GetBytes(HashHelper.MD5_16(argIV));

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(decrypted))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        originalText = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return originalText;
    }
}