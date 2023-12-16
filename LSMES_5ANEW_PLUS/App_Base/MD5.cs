using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class Encryption
    {
        /// <summary>
        /// MD5加密函数
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        /// 
        public static string MD5_Encode(string str)
        {
            MD5 m = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(str);
            byte[] result = m.ComputeHash(data);
            string ret1 = "";
            try
            {
                for (int j = 0; j < result.Length; j++)
                {
                    ret1 += result[j].ToString("x").PadLeft(2, '0');
                }
                return ret1;
            }
            catch
            {
                return str;
            }
        }

    }
}