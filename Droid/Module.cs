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
		protected TextView title;
		public Module(Context c, int index, View v, TextView tv)
		{
			this.context = c;
			this.index = index;
			this.view = v;
			this.title = tv;
			saveBtn = v.FindViewById<LinearLayout>(Resource.Id.save_button);
		}
	}
}
