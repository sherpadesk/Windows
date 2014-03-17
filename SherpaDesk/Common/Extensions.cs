using System;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SherpaDesk.Common
{
    public static class Extensions
    {
        public static TResponse Invalid<TResponse>(this TResponse response, params string[] messages)
            where TResponse : SherpaDesk.Models.Response.Response
        {
            response.Status = eResponseStatus.Invalid;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }

        public static TResponse Fail<TResponse>(this TResponse response, params string[] messages)
            where TResponse : SherpaDesk.Models.Response.Response
        {
            response.Status = eResponseStatus.Fail;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }


        public static TResponse Error<TResponse>(this TResponse response, params string[] messages)
            where TResponse : SherpaDesk.Models.Response.Response
        {
            response.Status = eResponseStatus.Error;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
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

        public static async void HandleError(this Page page, Response response)
        {
            MessageDialog dialog = new MessageDialog(response.Message, "Error");
            if (response.Status == eResponseStatus.Invalid)
            {
                //TODO: show the validation messages on page
                // apply page
            }
            else if (response.Status == eResponseStatus.Fail)
            {
                //TODO: show the simple error message from response.Message
                await dialog.ShowAsync();
            }
            else if (response.Status == eResponseStatus.Error)
            {
                //TODO: show the complex dialog with internal error message and descriptions from response.Messagess
                // It can has a possibility to send response object by email
                await dialog.ShowAsync();
            }
        }
    }
}
