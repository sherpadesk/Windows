using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.UI;
using SherpaDesk.Interfaces;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.IO;

namespace SherpaDesk.Common
{
    public static class Helper
    {
        public static Color HexStringToColor(string hexColor)
        {
            var color = new Color();
            var hc = ExtractHexDigits(hexColor);
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
            return input.Where(c => isHexDigit.IsMatch(c.ToString())).Aggregate("", (current, c) => current + c.ToString());
        }

        public static string GetMD5(string str)
        {
            const string hashType = "MD5";
            try
            {
                var Algorithm = HashAlgorithmProvider.OpenAlgorithm(hashType);
                var vector = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
                var digest = Algorithm.HashData(vector);
                if (digest.Length != Algorithm.HashLength)
                {
                    throw new InvalidOperationException(
                      "HashAlgorithmProvider failed to generate a hash of proper length!");
                }
                string dataHash = CryptographicBuffer.EncodeToHexString(digest);
                return dataHash;

            }
            catch
            {
                return string.Empty;
            }
        }

        public static string FullName(string firstName, string lastName, string email, bool withEmail = false)
        {
            if (string.IsNullOrWhiteSpace(firstName + lastName))
                return email;
            return !withEmail ? string.Format("{1}, {0}", firstName, lastName).Trim(',') : string.Format("{1}, {0} ({2})", firstName, lastName, email);
        }

        public static string GetUrlParams<TRequest>(TRequest request) where TRequest : IRequestType
        {
            var result = string.Empty;

            if (request.IsEmpty) return result;
            var type = typeof(TRequest);

            foreach (var method in from method in type.GetRuntimeMethods() 
                                   let onSerializing = method.GetCustomAttribute<OnSerializingAttribute>() 
                                   where onSerializing != null 
                                   select method)
            {
                method.Invoke(request, new object[] { null });
            }
            foreach (var prop in type.GetRuntimeProperties())
            {
                var dataMember = prop.GetCustomAttribute<DataMemberAttribute>();
                if (dataMember == null) continue;
                var val = prop.GetValue(request);
                if (!IsDefault(val, prop.PropertyType))
                {
                    result += string.Format("{0}={1}&", dataMember.Name, val);
                }
            }
            foreach (var field in type.GetRuntimeFields())
            {
                var dataMember = field.GetCustomAttribute<DataMemberAttribute>();
                if (dataMember == null) continue;
                var val = field.GetValue(request);
                if (!IsDefault(val, field.FieldType))
                {
                    result += string.Format("{0}={1}&", dataMember.Name, val);
                }
            }
            if (!string.IsNullOrEmpty(result))
                result = "?" + result.TrimEnd('&');
            return result;
        }

        public static bool IsDefault(object objectValue, Type objectType)
        {
            if (objectValue == null) return true;

            var type = objectType.GetTypeInfo();

            if (!type.IsValueType) return false;

            if (type.ContainsGenericParameters) return false;

            if (!type.IsPrimitive && type.IsNotPublic) return false;
            try
            {
                return Activator.CreateInstance(objectType).Equals(objectValue);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string HtmlToString(string html)
        {
            html = html.Replace("<br>", "<br/>");
            return Windows.Data.Html.HtmlUtilities.ConvertToText(html);
        }

        public static string ToXML<T>(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                serializer.Serialize(xmlWriter, value);
            }
            return stringBuilder.ToString(); 
        }

        public static T FromXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T value;
            using (StringReader stringReader = new StringReader(xml))
            {
                object deserialized = serializer.Deserialize(stringReader);
                value = (T)deserialized;
            }

            return value;
        } 
    }
}
