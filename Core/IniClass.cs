using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MiouIME.Core
{
    /// <summary>
    /// 读写INI文件
    /// </summary>
    public class IniClass
    {
        public string path;

        //[DllImport("kernel32")]
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        //[DllImport("kernel32")]
        //private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <param name="INIPath"></param>
        public IniClass(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section">节名</param>
        /// <param name="Key">键值名</param>
        /// <param name="Value">值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            bool isdeal = false;
            bool find = false;
            string[] strary = File.ReadAllLines(this.path, Encoding.Unicode);
            for (int i = 0; i < strary.Length; i++)
            {
                if (strary[i] == "[" + Section + "]")
                    isdeal = true;
                if (isdeal)
                {
                    if (strary[i].StartsWith(Key + "="))
                    {
                        strary[i] = strary[i].Split('=')[0] + "=" + Value;
                        find = true;
                        break;
                    }
                }
            }
            if (!find)
            {
                string[] strary1 = new string[strary.Length + 1];
                strary.CopyTo(strary1, 0);
                strary1[strary1.Length-1] = string.Format("{0}={1}", Key, Value);
                strary = strary1;
            }
            File.WriteAllLines(this.path, strary, Encoding.Unicode);
            //WritePrivateProfileString(Section, Key, Value, this.path); 
        }

        /// <summary>
        /// 读INI文件
        /// </summary>
        /// <param name="Section">节名</param>
        /// <param name="Key">键值名</param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            string temp = "";
            bool isdeal = false;
            string[] strary = File.ReadAllLines(this.path, Encoding.Unicode);
            for (int i = 0; i < strary.Length; i++)
            {
                if (strary[i] == "[" + Section + "]")
                    isdeal = true;
                if (isdeal)
                {
                    if (strary[i].StartsWith(Key + "="))
                    {
                        temp = strary[i].Split('=')[1];
                        break;
                    }
                }
            }
            //StringBuilder temp = new StringBuilder(255); 
            //int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path); 
            return temp.ToString();
        }
    }
}
