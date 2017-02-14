using GankIO.Common;
using GankIO.Models;
using GankIO.Services;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.UI;

namespace GankIO.ViewModels
{
    public class PhotosPageViewModel : ViewModelBase
    {
        #region event

        //public 福利 DraggingModel { get; set; }
        public DelegateCommand<Windows.UI.Xaml.Controls.DragItemsStartingEventArgs> DrapStartingCommand
        { get; set; }
        #endregion

        public PhotosPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                //Value = "Designtime value";
            }


            //DrapStartingCommand = new DelegateCommand<Windows.UI.Xaml.Controls.DragItemsStartingEventArgs>(e =>
            //{
            //    if (e.Cancel) return;
            //    //记录拖拽项的元数据
            //    DraggingModel = e.Items.First() as 福利;
            //    //对数据集合做数据持久化 即可实现每次打开应用后数据顺序是上次的排序后的顺序
            //});

            init();
        }

        private void init()
        {
            Action<Exception> onError = e =>
            {
                if (e is NoMoreItemsException) ErrorMsg = "已无更多图片。";
                else ErrorMsg = "加载图片出错。";
                IsLoading = false;
            };
            Action onStartLoading = () =>
            {
                ErrorMsg = null;
                IsLoading = true;
            };
            Action onEndLoading = () =>
            {
                IsLoading = false;
            };
            _norandomFulis = new IncrementalLoadingCollection<FuliSource, 福利>(20, onStartLoading, onEndLoading, onError);
            _randomFulis = new IncrementalLoadingCollection<RandomFuliSource, 福利>(20, onStartLoading, onEndLoading, onError);
        }
        public void Refresh()
        {
            init();
            setData(IsRandom.Value);
        }
        public async Task LoadMoreData()
        {
            ISupportIncrementalLoading srcs;
            if (IsRandom.Value)
            {
                srcs = _randomFulis;
            }
            else
            {
                srcs = _norandomFulis;
            }
            await srcs.LoadMoreItemsAsync(20);
            srcs.GetType().GetTypeInfo()
                .GetDeclaredProperty(nameof(_randomFulis.HasMoreItems))
                .SetValue(_randomFulis, true);
        }

        private string _ErrorMsg;
        public string ErrorMsg { get { return _ErrorMsg; } set { Set(ref _ErrorMsg, value); } }

        public bool? _isRandom = false;
        public bool? IsRandom
        {
            get
            {
                return _isRandom;
            }
            set
            {
                if (value == null || _isRandom == value) return;
                Set(ref _isRandom, value);
                setData(value.Value);
            }
        }


        bool _IsLoading = default(bool);
        public bool IsLoading { get { return _IsLoading; } set { Set(ref _IsLoading, value); } }       


        private void setData(bool random)
        {
            ErrorMsg = null;
            if (random)
            {
                Fulis = _randomFulis;
            }
            else
            {
                Fulis = _norandomFulis;
            }
        }

        ObservableCollection<福利> _fulis;
        internal ObservableCollection<福利> Fulis
        {
            get
            {
                return _fulis;
            }
            set
            {
                Set(ref _fulis, value);
            }
        }
        private IncrementalLoadingCollection<FuliSource, 福利> _norandomFulis;// = new IncrementalLoadingCollection<FuliSource, 福利>(20,onError:onError);
        private IncrementalLoadingCollection<RandomFuliSource, 福利> _randomFulis;// = new IncrementalLoadingCollection<RandomFuliSource, 福利>(20);



        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();

            //if (Fulis.Any() || mode == NavigationMode.Back) return;

            //var res = await GankService.GetFuliResult(10, 1);
            //res.results.ForEach(Fulis.Add);
            
            if(parameter is Boolean)
            {
                IsRandom = (bool)parameter;
            }
            else
            {
                setData(false);
            }
            

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }




    }
    public class FuliSource : IIncrementalSource<福利>
    {
        public async Task<IEnumerable<福利>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Views.Busy.SetBusy(true,$"加载第{pageIndex + 1}页数据。");
            var res = await GankService.GetFuliResult(pageSize, pageIndex + 1);
            //res.ForEach(async f => await ImageCache.Instance.PreCacheAsync(new Uri(f.url)));
            if (res == null || !res.Any()) throw new NoMoreItemsException();
            return res;
        }

    }
    public class RandomFuliSource : IIncrementalSource<福利>
    {
        public async Task<IEnumerable<福利>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GankService.GetRandomCategoryItemsAsync<福利>(pageSize);
            if (res == null || !res.Any()) throw new NoMoreItemsException();
            return res;
        }

    }
}
