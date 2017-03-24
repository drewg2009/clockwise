using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Clockwise.Droid
{
	public abstract class Module
	{
		protected Context context;
		protected int index;
		protected View view;
		protected LinearLayout saveBtn;

		public Module(Context c, int index, View v)
		{
			this.context = c;
			this.index = index;
			this.view = v;

			saveBtn = v.FindViewById<LinearLayout>(Resource.Id.save_button);
		}
	}
}
