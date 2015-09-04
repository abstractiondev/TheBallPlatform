using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Widget;
using Java.Interop;
using Xamarin;
using IRunnable = Java.Lang.IRunnable;
using Object = Java.Lang.Object;

namespace TheBallMobileApp
{
    public delegate string WebUIOperation(string url, string data);

    class TBJS2OP : Object, IRunnable
    {
        public const string TRUE_RESULT = "true";

        internal static TBJS2OP Current;

        public List<WebUIOperation> ActiveOperations = new List<WebUIOperation>();

        private Context context;

        public TBJS2OP(Context context)
        {
            this.context = context;
            Current = this;
        }

        public bool IsInitializing { get; set; }

        public void Run()
        {
            
        }

        [Export("ShowText")]
        public void ShowToast(Java.Lang.String text)
        {
            Toast.MakeText(context, text, ToastLength.Short).Show();
        }

        [Export("ExecuteAjaxOperation")]
        public string ExecuteAjaxOperation(Java.Lang.String operationUrl, Java.Lang.String operationData)
        {
            //Toast.MakeText(context, "Op call: " + operationUrl + " Data: " + operationData, ToastLength.Short).Show();
            //string locPart = getLocationString();
            //return true;
            var executionResult = ActiveOperations.Select(operation => operation(operationUrl.ToString(), operationData.ToString())).FirstOrDefault(result => result != null);
            Toast.MakeText(context, "Ding ding", ToastLength.Long).Show();
            return "{ \"OperationResult\": " + (executionResult ?? "null") + " }";
        }

        [Export("GetIsInitializing")]
        public bool GetIsInitializing()
        {
            return IsInitializing;
        }

        private string getLocationString()
        {
            var _locationManager = (LocationManager) context.GetSystemService(Activity.LocationService);
            Criteria criteria = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            var acceptableProviders = _locationManager.GetProviders(criteria, true);
            if (acceptableProviders.Any())
            {
                var provider = acceptableProviders.First();
                var location = _locationManager.GetLastKnownLocation(provider);
                return String.Format("Lat: {0}, Lon: {1}", location.Latitude, location.Longitude);
            }
            return "No location";
        }

        public void RegisterOperation(WebUIOperation operation)
        {
            ActiveOperations.Add(operation);
        }

        public void UnregisterOperation(WebUIOperation operation)
        {
            ActiveOperations.Remove(operation);
        }

        public static void ReportException(Exception exception)
        {
            if(Insights.IsInitialized)
                Insights.Report(exception);
        }

        public static void ReportSuccess(string successMessage)
        {
            Toast.MakeText(Current.context, successMessage, ToastLength.Long).Show();
        }
    }
}