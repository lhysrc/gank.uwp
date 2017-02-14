using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GankIO.Models
{

    public class DayResultRoot
    {
        public string[] category { get; set; }
        public bool error { get; set; }
        public DayResult results { get; set; }
    }

    public class DayResult
    {
        public Android[] Android { get; set; }
        public iOS[] iOS { get; set; }
        public App[] App { get; set; }
        public 休息视频[] 休息视频 { get; set; }
        public 福利[] 福利 { get; set; }
        public 拓展资源[] 拓展资源 { get; set; }
        public 瞎推荐[] 瞎推荐 { get; set; }
        public 前端[] 前端 { get; set; }

        private List<all> _all;
        public all[] all
        {
            get
            {
                if (_all == null)
                {
                    _all = new List<all>();
                    Android?.ForEach(_all.Add);
                    iOS?.ForEach(_all.Add);
                    App?.ForEach(_all.Add);
                    休息视频?.ForEach(_all.Add);
                    福利?.ForEach(_all.Add);
                    拓展资源?.ForEach(_all.Add);
                    瞎推荐?.ForEach(_all.Add);
                    前端?.ForEach(_all.Add);
                }
                return _all.ToArray();
            }
        }
    }


    public class all
    {
        public string _id { get; set; }
        public DateTime createdAt { get; set; }
        public string desc { get; set; }
        public string[] images { get; set; }
        public DateTime publishedAt { get; set; }
        public string source { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public bool used { get; set; }
        public string who { get; set; }

        public static bool operator ==(all a, all b)
        {
            return Object.Equals(a, b);
        }
        public static bool operator !=(all a, all b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj is all)
            {
                var a = (all)obj;
                return a._id == _id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static string[] GetAllTypeNames()
        {
            return typeof(DayResult)
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.Name != nameof(all))
                .Select(p => p.Name).ToArray();
        }
        /// <summary>
        /// 根据type字段映射到子类
        /// </summary>
        /// <returns></returns>
        public all MappingToDerived()
        {
            var srcType = GetType();
            var desType = Type.GetType($"{srcType.Namespace}.{type}");

            if (desType == null) return this;

            var srcProps = srcType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p => p.Name, p => p.GetValue(this));


            var o = Activator.CreateInstance(desType);


            desType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ForEach(p =>
            {
                if (srcProps.ContainsKey(p.Name))
                    p.SetValue(o, srcProps[p.Name]);
            });

            return (all)o;

            //T t = new T
            //{
            //    _id = _id,
            //    createdAt = createdAt,
            //    desc = desc,
            //    images = images,
            //    publishedAt = publishedAt,
            //    source = source,
            //    type = type,
            //    url = url,
            //    used = used,
            //    who = who
            //};
            //return t;
        }
    }

    //福利 | Android | iOS | 休息视频 | 拓展资源 | 前端 | all
    public class 瞎推荐 : all { }
    public class Android : all
    {
        //public string _id { get; set; }
        //public DateTime createdAt { get; set; }
        //public string desc { get; set; }
        //public string[] images { get; set; }
        //public DateTime publishedAt { get; set; }
        //public string source { get; set; }
        //public string type { get; set; }
        //public string url { get; set; }
        //public bool used { get; set; }
        //public string who { get; set; }
    }
    public class 前端 : all { }
    public class 拓展资源 : all { }
    public class App : all
    {
        //public string _id { get; set; }
        //public DateTime createdAt { get; set; }
        //public string desc { get; set; }
        //public DateTime publishedAt { get; set; }
        //public string source { get; set; }
        //public string type { get; set; }
        //public string url { get; set; }
        //public bool used { get; set; }
        //public string who { get; set; }
    }

    public class iOS : all
    {
        //public string _id { get; set; }
        //public DateTime createdAt { get; set; }
        //public string desc { get; set; }
        //public string[] images { get; set; }
        //public DateTime publishedAt { get; set; }
        //public string source { get; set; }
        //public string type { get; set; }
        //public string url { get; set; }
        //public bool used { get; set; }
        //public string who { get; set; }
    }

    public class 休息视频 : all
    {
        //public string _id { get; set; }
        //public DateTime createdAt { get; set; }
        //public string desc { get; set; }
        //public DateTime publishedAt { get; set; }
        //public string source { get; set; }
        //public string type { get; set; }
        //public string url { get; set; }
        //public bool used { get; set; }
        //public string who { get; set; }
    }

    public class 福利 : all
    {
        //public string _id { get; set; }
        //public DateTime createdAt { get; set; }
        //public string desc { get; set; }
        //public DateTime publishedAt { get; set; }
        //public string source { get; set; }
        //public string type { get; set; }
        //public string url { get; set; }
        //public bool used { get; set; }
        //public string who { get; set; }
    }
}
