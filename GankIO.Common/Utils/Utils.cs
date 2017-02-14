using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Net.Http;
using System.IO;

namespace GankIO.Common
{
    static public class Utils
    {
        private static readonly HttpClient _httpClient;
        static Utils()
        {
            _httpClient = new HttpClient { Timeout = new TimeSpan(0, 0, 10) };
        }
        public static async Task<string> GetJsonAsync(string url)
        {
            //请求  
            var res = _httpClient.GetStringAsync(url);
            return await res;

        }

        public static async Task<string> PostFormData(string url, Dictionary<string, string> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            var resp = await _httpClient.PostAsync(url, content);
            return await resp.Content.ReadAsStringAsync();
        }

        public static T Deserialize<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        public static long DirectorySize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) +
                   dir.GetDirectories().Sum(di => DirectorySize(di));
        }

        public static string ReadableSize(long size)
        {
            long t = 1024L * 1024L * 1024L * 1024L;
            long g = 1024L * 1024L * 1024L;
            long m = 1024L * 1024L;
            long k = 1024L;
            double dsize = size;
            if (size >= t)
            {
                return $"{dsize / t:0.00}TB";
            }
            else if(size >= g)
            {
                return $"{dsize / g:0.00}GB";
            }
            else if (size >= m)
            {
                return $"{dsize / m:0.00}MB";
            }
            else if (size >= k)
            {
                return $"{dsize / k:0.00}KB";
            }
            else
            {
                return $"{dsize:0.00}B";
            }
        }
    }
}
