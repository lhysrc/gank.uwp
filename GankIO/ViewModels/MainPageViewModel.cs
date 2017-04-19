using Template10.Mvvm;
using Template10.Utils;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using GankIO.Services;
using System.Collections.ObjectModel;
using GankIO.Models;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace GankIO.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {

            }
            SelectedDatesChanged = new DelegateCommand<CalendarViewSelectedDatesChangedEventArgs>(async e =>
            {
                if (e.AddedDates.Any())
                {
                    var date = e.AddedDates[0];
                    await loadDataByDate(date.Date,true);
                }
            });
            CalendarItemChanging = new DelegateCommand<CalendarViewDayItemChangingEventArgs>(e =>
            {
                if (HistoryDates == null) return;
                e.Item.IsBlackout = !HistoryDates.Contains(e.Item.Date.Date);

                //Debug.WriteLine(e.Item.Date.ToString());
                //args.Item.IsBlackout = true;
            });
        }
        public DelegateCommand<ItemClickEventArgs> ListViewItemClick { get; set; }
        public DelegateCommand<CalendarViewSelectedDatesChangedEventArgs> SelectedDatesChanged { get; set; }
        public DelegateCommand<CalendarViewDayItemChangingEventArgs> CalendarItemChanging
        { get; set; }
        string _HeaderImage = " ";
        public string HeaderImage { get { return _HeaderImage; } set { Set(ref _HeaderImage, value); } }
        //public string HeaderImage { get { return (DayResults.FirstOrDefault(o=>o is 福利) as 福利)?.url; }}
        string _Html = "";
        public string Html { get { return _Html; } set { Set(ref _Html, value); } }

        string _ErrorMsg = "";
        internal string ErrorMsg {
            get { return _ErrorMsg; }
            set
            {
                Set(ref _ErrorMsg, value);
                IsLoading = false;
            }
        }

        DateTime? _CurrentDate;
        internal DateTime? CurrentDate { get { return _CurrentDate; } set { Set(ref _CurrentDate, value); } }

        bool _IsLoading = default(bool);
        public bool IsLoading { get { return _IsLoading; } set { Set(ref _IsLoading, value); } }

        public DateTime[] HistoryDates;

        //DateTimeOffset _CurrentDate;
        //internal DateTimeOffset CurrentDate
        //{
        //    get { return _CurrentDate; }
        //    set
        //    {
        //        Set(ref _CurrentDate, value);
        //        if (HistoryDates.Contains(value.DateTime.Date))
        //            loadData(value.Date);
        //    }
        //}
        internal DateTimeOffset MaxDate
        {
            get
            {

                if (HistoryDates == null || HistoryDates.Length <= 0) return DateTimeOffset.Now;
                return HistoryDates.First() > DateTime.Now ? HistoryDates.First() : DateTimeOffset.Now;
            }
        }
        internal DateTimeOffset MinDate
        {
            get
            {
                if (HistoryDates == null || HistoryDates.Length <= 0) return DateTimeOffset.MinValue;
                return HistoryDates.Last();
            }
        }

        internal ObservableCollection<all> DayResults { get; set; } = new ObservableCollection<all>();
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (DayResults.Any() || mode == NavigationMode.Back) return;


            await InitData();
            await Task.CompletedTask;
        }
        public async Task InitData(bool useCache = true)
        {
            IsLoading = true; ErrorMsg = "";
            try
            {
                HistoryDates = await GankService.GetHistoryDatesAsync();
                if (HistoryDates == null || HistoryDates.Length <= 0)
                {
                    ErrorMsg = "获取天数失败。";
                    return;
                }
                CurrentDate = CurrentDate ?? HistoryDates.First();
                await loadDataByDate(CurrentDate.Value, useCache);

                if (CurrentDate >= DateTime.Today) Common.TileAndToast.ClearBadge();
            }
            catch (Exception e)
            {
                ErrorMsg = $"获取数据失败：{e.Message}";
            }
            finally
            {
                //Views.Busy.SetBusy(false);
                IsLoading = false;
            }
        }
        public async Task ReloadData()
        {
           await InitData(false);
        }

        private async Task loadDataByDate(DateTime date,bool useCache)
        {            
            DayResults.Clear(); HeaderImage = " "; ErrorMsg = "";
            IsLoading = true;

            if (!HistoryDates.Contains(date.Date))
            {
                ErrorMsg = $"当日无数据：{date:yyyy-MM-dd}";
                return;
            }

            //Views.Busy.SetBusy(true, "请稍候...");
            try
            {
                var res = await GankService.GetDayResult(date, useCache);

                res.all.Where(i => !(i is 福利)).ForEach(DayResults.Add);
                HeaderImage = res.福利?.FirstOrDefault()?.url;
            }
            catch (Exception e)
            {
                ErrorMsg = $"获取{date:yyyy-MM-dd}数据失败：{Environment.NewLine}{e.Message}";
            }
            finally
            {
                //Views.Busy.SetBusy(false);
                CurrentDate = date;//.ToString("yyyy-MM-dd");
                IsLoading = false;
            }


        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            //if (suspending)
            //{
            //    suspensionState[nameof(CurrentDate)] = CurrentDate.ToString("yyyy-MM-dd");
            //}
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoDetailsPage()
        {
            //Html = Value;
            //NavigationService.Navigate(typeof(Views.DetailPage), Value);
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}
