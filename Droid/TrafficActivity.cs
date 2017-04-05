
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clockwise.Droid
{
	[Activity(Label = "TrafficActivity")]
	public class TrafficActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.traffic);
			initUI();

		}

		private void initUI()
		{
			Spinner transType = FindViewById<Spinner>(Resource.Id.transType);
			transType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			//var adapter = ArrayAdapter.CreateFromResource(
			//		this, Resource.Array.transportation_methods, Android.Resource.Layout.SimpleSpinnerItem);

			//adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			//transType.Adapter = adapter;
		}

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			//string toast = string.Format("The planet is {0}", spinner.GetItemAtPosition(e.Position));
			//Toast.MakeText(this, toast, ToastLength.Long).Show();
		}
	}
}
