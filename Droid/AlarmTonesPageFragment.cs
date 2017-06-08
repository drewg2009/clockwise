using System;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
namespace Clockwise.Droid
{
	public class AlarmTonesPageFragment : Android.Support.V4.App.Fragment
	{
		public static String ARG_PAGE = "ARG_PAGE";
		public static String INDEX = "INDEX";
		static RadioButton defaultRadioButton = null;
		static RadioButton songRadioButton = null;
		private LinearLayout groupHolder;
		private int mPage;
		private int index;
		RecyclerView alarmTonesRecyclerView;
		SongManager sm;
		public static AlarmTonesPageFragment newInstance(int page, int alarmIndex)
		{
			Bundle args = new Bundle();
			args.PutInt(ARG_PAGE, page);
			args.PutInt(INDEX, alarmIndex);

			AlarmTonesPageFragment fragment = new AlarmTonesPageFragment();
			fragment.Arguments = args;
			return fragment;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			mPage = Arguments.GetInt(ARG_PAGE);
			index = Arguments.GetInt(INDEX);
		}

		public override void OnResume()
		{
			base.OnResume();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (sm != null) { sm.stop(); }
			base.OnStop();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LinearLayout view = (LinearLayout)inflater.Inflate(Resource.Layout.tone_list_fragment, container, false);
			alarmTonesRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.tones_recycler_view);
			alarmTonesRecyclerView.HasFixedSize = true;
			alarmTonesRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
			sm = SongManager.getInstance(Context);
			alarmTonesRecyclerView.SetAdapter(new TonesRecyclerViewAdapter(Context, mPage, sm, index));
			return view;
		}
	}
}
