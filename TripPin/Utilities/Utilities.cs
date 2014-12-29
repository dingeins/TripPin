using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Microsoft.OData.Client;
using TripPin.Common;

namespace TripPin.Utilities
{
    class Utilities
    {
        public static async Task AssignCountToPageDataAsync(string path, string key, ObservableDictionary defaultViewModel, string term)
        {
            Exception exception = null;
                int count = 0;
            if (!App.localSettings.Values.ContainsKey(key))
            {
                
                try
                {
                    EventHandler<SendingRequest2EventArgs> changePlainTextEventHandler =
                        (s, args) => args.RequestMessage.SetHeader("Accept", "text/plain;charset=utf-8");
                    App.tripPinContext.SendingRequest2 += changePlainTextEventHandler;
                    count =
                        (await App.tripPinContext.ExecuteAsync<int>(new Uri(App.tripPinContext.BaseUri + path))).First();
                    App.tripPinContext.SendingRequest2 -= changePlainTextEventHandler;
                }
                catch (Exception localException)
                {
                    exception = localException;
                }
                if (exception == null)
                {
                    App.localSettings.Values[key] = count;
                }
                else
                {
                    await new MessageDialog(exception.Message, "Error loading " + key).ShowAsync();
                }
            }
            else
            {
                count = (int)App.localSettings.Values[key];
            }

            defaultViewModel[key] = count.ToString() + term;
        }
    }
}
