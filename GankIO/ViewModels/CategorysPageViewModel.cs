using GankIO.Models;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using GankIO.Services;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml.Data;

namespace GankIO.ViewModels
{
    public interface IRefresh
    {
        void Refresh();
    }
    
    public class CategoryPartViewModel<T>: ViewModelBase, IRefresh where T : all
    {
        private IncrementalLoadingCollection<CategorySource<T>, T> _CategoryItems;
        public IncrementalLoadingCollection<CategorySource<T>, T> CategoryItems
        {
            get { return _CategoryItems; }
            private set { Set(ref _CategoryItems, value); }
        }
        public CategoryPartViewModel()
        {
            init();
        }
        void init()
        {
            Action<Exception> onError = e =>
            {
                if (e is NoMoreItemsException) ErrorMsg = "已无更多条目。";
                else ErrorMsg = "加载出错。";
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
            CategoryItems = new IncrementalLoadingCollection<CategorySource<T>, T>(20, onStartLoading, onEndLoading, onError);
        }
        public void Refresh()
        {
            ErrorMsg = null;
            init();
        }
        DelegateCommand _ReloadCommand;
        public DelegateCommand ReloadCommand
            => _ReloadCommand ?? (_ReloadCommand = new DelegateCommand(async () =>
            {
                await CategoryItems.LoadMoreItemsAsync(20);
                CategoryItems.GetType().GetTypeInfo()
                .GetDeclaredProperty(nameof(CategoryItems.HasMoreItems))
                .SetValue(CategoryItems, true);
            }));

        bool _IsLoading = default(bool);
        public bool IsLoading { get { return _IsLoading; } set { Set(ref _IsLoading, value); } }
        private string _ErrorMsg;
        public string ErrorMsg { get { return _ErrorMsg; } set { Set(ref _ErrorMsg, value); } }
    }
    public class CategorysPageViewModel : ViewModelBase
    {
        public CategoryPartViewModel<all> allPartViewModel { get; } 
            = new CategoryPartViewModel<all>();
        public CategoryPartViewModel<Android> AndroidPartViewModel { get; } 
            = new CategoryPartViewModel<Android>();
        public CategoryPartViewModel<iOS> iOSPartViewModel { get; } 
            = new CategoryPartViewModel<iOS>();
        public CategoryPartViewModel<前端> QdPartViewModel { get; }
            = new CategoryPartViewModel<前端>();
        public CategoryPartViewModel<拓展资源> TzzyPartViewModel { get; }
            = new CategoryPartViewModel<拓展资源>();
        public CategoryPartViewModel<瞎推荐> XtzPartViewModel { get; }
                    = new CategoryPartViewModel<瞎推荐>();
        public CategoryPartViewModel<休息视频> VideoPartViewModel { get; }
                    = new CategoryPartViewModel<休息视频>();
        
        public CategorysPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
    }


    public class CategorySource<T> : IIncrementalSource<T> where T: all
    {
        public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {            
            //Random r = new Random();
            //if (r.Next(100) % 3 == 0)
            //    throw new Exception();

            var res = await GankService.GetCategoryItems<T>(pageSize, pageIndex + 1);
            if (res == null || !res.Any()) throw new NoMoreItemsException();
            return res;
        }

    }
    public class RandomCategorySource<T> : IIncrementalSource<T> where T : all
    {
        public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {

            //Views.Busy.SetBusy(true,$"加载第{pageIndex + 1}页数据。");
            //try
            //{
            var res = await GankService.GetRandomCategoryItemsAsync<T>(pageSize);
            if (res == null || !res.Any()) throw new NoMoreItemsException();
            return res;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine($"获取{typeof(T)}数据失败：" + e.Message);
            //    return new T[0];
            //}
            //finally
            //{
            //    //Views.Busy.SetBusy(false);
            //}
        }

    }

    class NoMoreItemsException : Exception { }
}
