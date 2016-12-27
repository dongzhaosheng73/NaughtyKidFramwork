using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NaughtyKid.MyTools
{
    /// <summary>
    /// 字符串加密帮助类
    /// </summary>
    public  class EncryptDecodeHelper
    {
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="pToEncrypt">加密的字符</param>
        /// <param name="sKey">加密key</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string pToEncrypt, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = Encoding.ASCII.GetBytes(sKey);
            des.IV = Encoding.ASCII.GetBytes(sKey);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();

        }
        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="pToDecrypt">解密的字符串</param>
        /// <param name="sKey">解密Key</param>
        /// <returns></returns>
        public static string Decrypt(string pToDecrypt, string sKey)
        {
            var des = new DESCryptoServiceProvider();

            var inputByteArray = new byte[pToDecrypt.Length / 2];
            for (var x = 0; x < pToDecrypt.Length / 2; x++)
            {
                var i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = Encoding.ASCII.GetBytes(sKey);
            des.IV = Encoding.ASCII.GetBytes(sKey);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Encoding.Default.GetString(ms.ToArray());
        }
        /// <summary>
        /// 生成加密Kye
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            var desCrypto = (DESCryptoServiceProvider)DES.Create();

            return desCrypto.ToString();
        }
    }
}
