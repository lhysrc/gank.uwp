using GankIO.Common;
using GankIO.Models;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Storage;
using Windows.Web.Http;

namespace GankIO.Services
{
    public static class GankService
    {
        private static string contentUrl(int y, int m, int d) =>
            $"http://gank.io/api/history/content/day/{y}/{m}/{d}";

        //static StorageFileHelper _storageHelper = new StorageFileHelper();
        public static async Task<DayResult> GetDayResult(DateTime date, bool useCache)
        {
            var url = $"http://gank.io/api/day/{ date.Year }/{ date.Month }/{ date.Day }";// dayUrl(date.Year, date.Month, date.Day);

            string key = $"DayResult{date.ToString("yyyyMMdd")}";
            string json = string.Empty;

            if (useCache && await ApplicationData.Current.LocalCacheFolder.FileExistsAsync(key))
            {
                json = await StorageFileHelper.ReadTextFromLocalCacheFileAsync(key);
            }
            else
            {
                json = await Utils.GetJsonAsync(url);
                await StorageFileHelper.WriteTextToLocalCacheFileAsync(json, key);
            }
            var res = Utils.Deserialize<DayResultRoot>(json);

            return res.error ? null : res.results;

        }

        public static async Task<ContentResultRoot> GetContentResult(DateTime date)
        {
            var url = contentUrl(date.Year, date.Month, date.Day);

            var json = await Common.Utils.GetJsonAsync(url);


            var res = Utils.Deserialize<ContentResultRoot>(json);
            return res;
        }

        public static async Task<福利[]> GetFuliResult(int pageSize, int pageNum)
        {
            var url = $"http://gank.io/api/data/%E7%A6%8F%E5%88%A9/{pageSize}/{pageNum}";

            var json = await Common.Utils.GetJsonAsync(url);


            var res = Utils.Deserialize<FuliResult>(json);
            return res.error ? null : res.results;
        }


        public static async Task<T[]> GetCategoryItems<T>(int pageSize, int pageNum) where T : all
        {
            var url = $"http://gank.io/api/data/{typeof(T).Name}/{pageSize}/{pageNum}";

            var json = await Utils.GetJsonAsync(url);

            var res = Utils.Deserialize<CategoryRetuls<T>>(json);

            //if(typeof(T) == typeof(all))
            //{
            //    return res.results.Select(r => (T)r.MappingToDerived()).ToArray();
            //}
            return res.error ? null : res.results;

        }


        public static async Task<T[]> GetRandomCategoryItemsAsync<T>(int cnt) where T : all
        {
            var url = $"http://gank.io/api/random/data/{typeof(T).Name}/{cnt}";

            var json = await Utils.GetJsonAsync(url);

            var res = Utils.Deserialize<CategoryRetuls<T>>(json);

            return res.error ? null : res.results;

        }

        public static async Task<DateTime[]> GetHistoryDatesAsync()
        {
            var url = "http://gank.io/api/day/history";
            //try
            //{
            var json = await Utils.GetJsonAsync(url);


            var res = Utils.Deserialize<HistoryDates>(json);

            return res.error ? new DateTime[0] : res.results;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine(e.Message);
            //    return new DateTime[0];
            //}


        }

        public static async Task<PostResult> PostGank(string url, string desc, string who, string type, bool debug = false)
        {
            var postUrl = @"https://gank.io/api/add2gank";
            var postData = new Dictionary<string, string>
            {
                { nameof(url) , url },
                { nameof(desc) , desc },
                { nameof(who) , who },
                { nameof(type) , type },
                { nameof(debug) , debug.ToString().ToLower() },
            };
            var json = await Utils.PostFormData(postUrl, postData);
            return Utils.Deserialize<PostResult>(json);
        }

    }
}
