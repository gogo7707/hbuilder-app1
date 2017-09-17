using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Randao.Web.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(this string str, int len)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            else if (str.Length > len)
            {
                return str.Substring(0, len);
            }
            else
            {
                return str;
            }
        }

        #region 字符串进行编码
        /// <summary>
        /// 把字符串转换成BASE64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeBase64(this string str)
        {
            return Convert.ToBase64String(str.ToByteArray<System.Text.UTF8Encoding>());
        }

        /// <summary>
        /// 把BASE64转换成字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeBase64(this string str)
        {
            return System.Text.UTF8Encoding.ASCII.GetString(Convert.FromBase64String(str));
        }
        /// <summary>
        /// 对URL字符串进行编码(UTF-8)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            return System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 对URL字符串进行解码(UTF-8)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            return System.Web.HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8);
        }
        #endregion

        #region 字符串转换为字节数
        /// <summary>
        /// 字符串转换为字节数组
        /// </summary>
        /// <typeparam name="encoding">编码</typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByteArray<encoding>(this string str) where encoding : Encoding
        {
            Encoding enc = Activator.CreateInstance<encoding>();
            return enc.GetBytes(str);
        }

        /// <summary>
        /// 字符串转换为字节数组（UTF8）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string str)
        {
            return str.ToByteArray<System.Text.UTF8Encoding>();
        }

        /// <summary>
        /// 字符串转换为字节序列（UTF8）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Stream ToStream(this string str)
        {
            return str.ToStream<System.Text.UTF8Encoding>();
        }

        /// <summary>
        /// 字符串转换为字节序列
        /// </summary>
        /// <typeparam name="encoding">编码</typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Stream ToStream<encoding>(this string str) where encoding : Encoding
        {
            byte[] bytes = str.ToByteArray<encoding>();
            return new System.IO.MemoryStream(bytes);
        }
        #endregion

        #region 加密/解密
        /// <summary>
        /// 不可逆加密
        /// </summary>
        /// <typeparam name="Algorithm">加密HASH算法</typeparam>
        /// <typeparam name="StringEncoding">字符编码</typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptOneWay<Algorithm, StringEncoding>(this string str)
            where Algorithm : HashAlgorithm
            where StringEncoding : Encoding
        {
            Encoding enco = Activator.CreateInstance<StringEncoding>();
            byte[] inputBye = enco.GetBytes(str);
            byte[] bytes = Activator.CreateInstance<Algorithm>().ComputeHash(inputBye);
            return System.BitConverter.ToString(bytes).Replace("-", ""); ;
        }

        /// <summary>
        /// 不可逆加密
        /// </summary>
        /// <typeparam name="Algorithm">加密HASH算法</typeparam>
        /// <param name="str">字符编码</param>
        /// <returns></returns>
        public static string EncryptOneWay<Algorithm>(this string str)
            where Algorithm : HashAlgorithm
        {
            return str.EncryptOneWay<Algorithm, System.Text.UTF8Encoding>();
        }

        /// <summary>
        /// MD5编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5(this string text)
        {
            return text.EncryptOneWay<MD5CryptoServiceProvider, UTF8Encoding>().ToLower();
        }

        public static string SHA512(this string text)
        {
            return text.EncryptOneWay<SHA512Cng, UTF8Encoding>();
        }
        public static string SHA256(this string text)
        {
            return text.EncryptOneWay<SHA256Cng, UTF8Encoding>();
        }
        #endregion

        #region 验证

        /// <summary>
        /// 验证整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidNumber(this String input)
        {
            return ValidateString(input, @"^[1-9]{1}[0-9]{0,9}$");
        }

        /// <summary>
        /// 验证是否日期
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidDate(this String input)
        {
            bool bValid = ValidateString(input, @"^[12]{1}(\d){3}[-][01]?(\d){1}[-][0123]?(\d){1}$");
            return (bValid && input.CompareTo("1753-01-01") >= 0);
        }

        /// <summary>
        /// 验证EMAIL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this String input)
        {
            return ValidateString(input, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 验证EMAIL
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this String input, String expression)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (String.IsNullOrEmpty(expression))
            {
                expression = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            }
            return ValidateString(input, expression);
        }

        /// <summary>
        /// 验证是否手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidMobile(this String input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return ValidateString(input, "^0{0,1}(1[3-8])[0-9]{9}$");
        }

        internal static Boolean ValidateString(String input, String expression)
        {
            Regex validator = new Regex(expression, RegexOptions.None);
            return validator.IsMatch(input);
        }

        public static bool IsValidIP(this String input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            return ValidateString(input, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
        }
        #endregion

    }
}
