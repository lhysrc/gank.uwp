using GankIO.Common;
using GankIO.Services;
using System;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace GankIO.BackgroundTask
{
    public sealed class LiveTileTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            await TileAndToast.Show();

            deferral.Complete();
        }       

    }
}
