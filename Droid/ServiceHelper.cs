using System;
using Android.App;
using Android.Content;

namespace Clockwise.Droid
{
	public class ServiceHelper
	{
		public static bool IsMyServiceRunning(Type serviceClass, Context context)
		{
			ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
			foreach(ActivityManager.RunningServiceInfo service in manager.GetRunningServices(int.MaxValue))
			{
				if (serviceClass.Name.Equals(service.Service.ClassName))
				{
					return true;
				}
			}
			return false;
		}
	}
}
