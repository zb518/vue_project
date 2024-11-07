using System.Security.Cryptography;
using System.Text;

namespace PPE.Core;
/// <summary>
/// 生成hash值
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// 生成16位哈希值
    /// </summary>
    /// <param name="plainString"></param>
    /// <returns></returns>
    public static string MD5_16(this string plainString)
    {
        var data = Encoding.UTF8.GetBytes(plainString);
        var hash = MD5.HashData(data);
        return BitConverter.ToString(hash, 4, 8).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 生成SHA256
    /// </summary>
    /// <param name="plainString"></param>
    /// <returns></returns>
    public static string GetSHA256(this string plainString)
    {
        if (string.IsNullOrEmpty(plainString))
        {
            return string.Empty;
        }
        using (SHA256 hash = SHA256.Create())
        {
            return GetHash(hash, plainString);
        }
    }

    /// <summary>
    /// 生成SHA384
    /// </summary>
    /// <param name="plainString"></param>
    /// <returns></returns>
    public static string GetSHA384(this string plainString)
    {
        if (string.IsNullOrEmpty(plainString))
        {
            return string.Empty;
        }
        using (SHA384 hash = SHA384.Create())
        {
            return GetHash(hash, plainString);
        }
    }

    /// <summary>
    /// 生成SHA512
    /// </summary>
    /// <param name="plainString"></param>
    /// <returns></returns>
    public static string GetSHA512(this string plainString)
    {
        if (string.IsNullOrEmpty(plainString))
        {
            return string.Empty;
        }
        using (SHA512 hash = SHA512.Create())
        {
            return GetHash(hash, plainString);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hashAlgorithm"></param>
    /// <param name="plainString"></param>
    /// <returns></returns>
    private static string GetHash(HashAlgorithm hashAlgorithm, string plainString)
    {
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(plainString));
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            builder.Append(data[i].ToString("x2"));
        }
        return builder.ToString();
    }

    /// <summary>
    /// Verify a hash against a string
    /// </summary>
    /// <param name="hashAlgorithm"></param>
    /// <param name="input"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    public static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
    {
        var hashOfInput = GetHash(hashAlgorithm, input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        return comparer.Compare(hashOfInput, hash) == 0;
    }

    /// <summary>
    /// MD5 32位哈希值
    /// </summary>
    /// <param name="plainString"></param>
    /// <returns></returns>
    public static string MD5_32(this string plainString)
    {
        if (string.IsNullOrEmpty(plainString))
        {
            return string.Empty;
        }
        using (MD5 hash = MD5.Create())
        {
            return GetHash(hash, plainString);
        }
    }
}