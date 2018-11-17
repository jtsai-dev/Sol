using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JT.Infrastructure.AppCommon
{
    public class Security
    {
        #region MD5 Encrypt
        /// <summary>
        /// MD5 Encrypt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string str)
        {
            byte[] strBytes = Encoding.ASCII.GetBytes(str);

            byte[] strEncryptResult = (new MD5CryptoServiceProvider()).ComputeHash(strBytes);

            return Convert.ToBase64String(strEncryptResult);
        }
        #endregion


        #region DES
        private readonly static string keyFix = "tzZ_Y2!j";

        /// <summary>
        /// DES Encrypt
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">the key's length must be equal or less than 8</param>
        /// <returns></returns>
        public static string DESEncrypt(string str, string key)
        {
            if (key.Length > 8)
            {
                throw new Exception("the key's length must be equal or less than 8");
            }

            if (key.Length < 8)
            {
                key += keyFix.Substring(0, 8 - key.Length);
            }

            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(key);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV),
CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(str);
            cst.FlushFinalBlock();
            string result = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

            cryptoProvider.Clear();
            cst.Close();
            ms.Close();

            return result;
        }

        /// <summary>
        /// DES Decrypt
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">the key's length must be equal 8</param>
        /// <returns></returns>
        public static string DESDecrypt(string str, string key)
        {
            if (key.Length < 8)
            {
                key += keyFix.Substring(0, 8 - key.Length);
            }

            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(key);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(str));
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);

            StreamReader sr = new StreamReader(cst);
            string result = sr.ReadToEnd();

            cryptoProvider.Clear();
            cst.Close();
            ms.Close();

            return result;
        }

        #endregion


        #region RSA
        public static void RSACreateKey(ref string publicKey, ref string privateKey)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            publicKey = RSA.ToXmlString(false);
            privateKey = RSA.ToXmlString(true);
        }

        /// <summary>
        /// RSA Encrypt
        /// </summary>
        /// <param name="str"></param>
        /// <param name="publicKey">public Key</param>
        /// <returns></returns>
        public static string RSAEncrypt(string str, string publicKey)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(publicKey);

            byte[] encryptedData = RSA.Encrypt(data, true);
            RSA.Clear();

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// RSA Decrypt
        /// </summary>
        /// <param name="str"></param>
        /// <param name="privateKey">private Key</param>
        /// <returns></returns>
        public static string RSADecrypt(string str, string privateKey)
        {
            byte[] bydata = Convert.FromBase64String(str);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);

            byte[] decryptedData = RSA.Decrypt(bydata, true);
            RSA.Clear();

            return Encoding.UTF8.GetString(decryptedData);
        }
        #endregion
    }
}
