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

        public static string GetAppVersion()
        {
            var v = Windows.ApplicationModel.Package.Current.Id.Version;
            return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
        }
        public static string GetOSVersion()
        {
            string deviceFamilyVersion = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong major = (version & 0xFFFF000000000000L) >> 48;
            ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            ulong revision = (version & 0x000000000000FFFFL);
            var osVersion = $"{major}.{minor}.{build}.{revision}";
            return osVersion;
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

        public static string ReadableDate(this DateTime date,string format="yyyy/MM/dd")
        {
            var today = DateTime.Today;
            switch ((date.Date - today).Days)
            {
                case -2: return "前天";
                case -1: return "昨天";
                case 0: return "今天";
                case 1: return "明天";
                case 2: return "后天";
                default:return date.ToString(format);
            }
        }
    }
}
