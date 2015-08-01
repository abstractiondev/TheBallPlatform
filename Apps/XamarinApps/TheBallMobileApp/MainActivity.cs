using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin;

namespace TheBallMobileApp
{
    [Activity(Label = "TheBallMobileApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            Insights.Initialize("13ee2f7f9d2bca655467bf4b4e217fcd4658384a", ApplicationContext);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                button.Text = string.Format("{0} clicks!", count++); 
                if(count > 2)
                    throw new InvalidOperationException("Not expected operation (Xamarin Insight testing...)");
            };
        }
    }
}

