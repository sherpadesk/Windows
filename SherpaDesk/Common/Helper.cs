using SherpaDesk.Models.Request;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI;

namespace SherpaDesk.Common
{
    public static class Helper
    {
        public static Color HexStringToColor(string hexColor)
        {
            Color color = new Color();
            string hc = ExtractHexDigits(hexColor);
            if (hc.Length != 8)
            {
                return color;
            }
            string a = hc.Substring(0, 2);
            string r = hc.Substring(2, 2);
            string g = hc.Substring(4, 2);
            string b = hc.Substring(6, 2);
            try
            {
                byte ai = Byte.Parse(a, NumberStyles.HexNumber);
                byte ri = Byte.Parse(r, NumberStyles.HexNumber);
                byte gi = Byte.Parse(g, NumberStyles.HexNumber);
                byte bi = Byte.Parse(b, NumberStyles.HexNumber);
                color = Color.FromArgb(ai, ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return color;
            }
            return color;
        }

        public static string ExtractHexDigits(string input)
        {
            // remove any characters that are not digits (like #)
            var isHexDigit
                = new Regex("[abcdefABCDEF\\d]+");
            string newnum = "";
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString()))
                {
                    newnum += c.ToString();
                }
            }
            return newnum;
        }

        public static string GetMD5(string str)
        {
            string dataHash = string.Empty;
            string hashType = "MD5";
            try
            {
                HashAlgorithmProvider Algorithm = HashAlgorithmProvider.OpenAlgorithm(hashType);
                IBuffer vector = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
                IBuffer digest = Algorithm.HashData(vector);
                if (digest.Length != Algorithm.HashLength)
                {
                    throw new System.InvalidOperationException(
                      "HashAlgorithmProvider failed to generate a hash of proper length!");
                }
                else
                {
                    dataHash = CryptographicBuffer.EncodeToHexString(digest);//Encoding it to a Hex String 
                    return dataHash;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string FullName(string firstName, string lastName, string email)
        {
            if (string.IsNullOrWhiteSpace(firstName + lastName))
                return email;
            else
                return string.Format("{1}, {0}", firstName, lastName).Trim(',');
        }

        public static string GetUrlParams<TRequest>(TRequest request) where TRequest : IRequestType
        {
            string result = string.Empty;

            if (!request.IsEmpty)
            {
                var type = typeof(TRequest);
                foreach (var method in type.GetRuntimeMethods())
                {
                    var onSerializing = method.GetCustomAttribute<OnSerializingAttribute>();
                    if (onSerializing != null)
                    {
                        method.Invoke(request, new object[1] { null });
                    }
                }
                foreach (var prop in type.GetRuntimeProperties())
                {
                    var dataMember = prop.GetCustomAttribute<DataMemberAttribute>();
                    if (dataMember != null)
                    {
                        object val = prop.GetValue(request);
                        if (!IsDefault(val, prop.PropertyType))
                        {
                            result += string.Format("{0}={1}&", dataMember.Name, val);
                        }
                    }
                }
                foreach (var field in type.GetRuntimeFields())
                {
                    var dataMember = field.GetCustomAttribute<DataMemberAttribute>();
                    if (dataMember != null)
                    {
                        object val = field.GetValue(request);
                        if (!IsDefault(val, field.FieldType))
                        {
                            result += string.Format("{0}={1}&", dataMember.Name, val);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(result))
                    result = "?" + result.TrimEnd('&');
            }
            return result;
        }

        public static bool IsDefault(object objectValue, Type objectType)
        {
            if (objectValue == null) return true;

            var type = objectType.GetTypeInfo();

            if (!type.IsValueType) return false;

            if (type.ContainsGenericParameters) return false;

            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(objectType).Equals(objectValue);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else return false;
        }

        public static string HtmlToString(string html)
        {
            html = html.Replace("<br>", Environment.NewLine);
            return Windows.Data.Html.HtmlUtilities.ConvertToText(html);
        }
    }
}
