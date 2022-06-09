using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace MiouIME.Core
{
    public class comm
    {
        static string iv = "miounet8";
        static string key = "miounet8";

        /// <summary>
        /// DES加密偏移量，必须是>=8位长的字符串
        /// </summary>
        public static string IV
        {
            get { return iv; }
            set { iv = value; }
        }

        /// <summary>
        /// DES加密的私钥，必须是8位长的字符串
        /// </summary>
        public static string Key
        {
            get { return key; }
            set { key = value; }
        }

 

      
        public static string Decrypt(string sourceFile)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);
            Encoding txtcode =InputHelp.GetFileType(sourceFile);
            string pToDecrypt = string.Empty;
            pToDecrypt = File.ReadAllText(sourceFile, txtcode);

            if (pToDecrypt == "")
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < pToDecrypt.Length; i++)
            {

                    sb.Append((char)(pToDecrypt[i] + 1));
                
            }

            pToDecrypt = sb.ToString();

            return pToDecrypt;
        }

 
        public static string Encrypt(string pToEncrypt)
        {
            string strReturn = "";
            if (pToEncrypt == "")
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < pToEncrypt.Length; i++)
            {
                    sb.Append((char)(pToEncrypt[i] - 1));
            }

            strReturn = sb.ToString();

            return strReturn;

        }

       
        public static void EncryptMB(string sourceFile)
        {
            Core.InputHelp.openJM = Core.InputHelp.mbiniobj.IniReadValue("词库设置", "加密") == "1" ? true : false;
            if (Core.InputHelp.openJM)
            {
                Encoding txtcode = InputHelp.GetFileType(sourceFile);
                string pToDecrypt = string.Empty;
                pToDecrypt = File.ReadAllText(sourceFile, txtcode);
                File.WriteAllText(sourceFile, Encrypt(pToDecrypt), txtcode);
            }
        }
    }
}
