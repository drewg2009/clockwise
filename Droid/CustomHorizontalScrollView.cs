using System;
using Android.Animation;
using Android.Content;
using Android.Widget;
using Java.Lang;

namespace Clockwise.Droid
{
	public class CustomHorizontalScrollView : HorizontalScrollView
	{
		public interface IOnScrollChangedListener
		{
			void onScrollStart();
			void onScrollEnd();
		}

		private bool IsAnimating = false;
		private long lastScrollUpdate = -1;
		private int scrollTaskInterval = 100;
		private Runnable mScrollingRunnable;
		public IOnScrollChangedListener mOnScrollListener;

		public CustomHorizontalScrollView(Context context) : base(context, null, 0)
		{
			init(context);
		}

		public CustomHorizontalScrollView(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs, 0)
		{
			init(context);
		}

		public CustomHorizontalScrollView(Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			init(context);
		}

		private void init(Context context)
		{
			mScrollingRunnable = new Runnable(delegate
			{
				if ((Java.Lang.JavaSystem.CurrentTimeMillis() - lastScrollUpdate) > scrollTaskInterval)
				{
					// Scrolling has stopped.
					lastScrollUpdate = -1;
					if(!IsAnimating) mOnScrollListener.onScrollEnd();
				}
				else
				{
					// Still scrolling - Check again in scrollTaskInterval milliseconds...
					PostDelayed(mScrollingRunnable, scrollTaskInterval);
				}
			});
		}

		public void setOnScrollChangedListener(IOnScrollChangedListener onScrollChangedListener)
		{
			this.mOnScrollListener = onScrollChangedListener;
		}

		public void setScrollTaskInterval(int scrollTaskInterval)
		{
			this.scrollTaskInterval = scrollTaskInterval;
		}

		protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
		{
			base.OnScrollChanged(l, t, oldl, oldt);
			if (mOnScrollListener != null)
			{
				if (lastScrollUpdate == -1)
				{
					mOnScrollListener.onScrollStart();
					PostDelayed(mScrollingRunnable, scrollTaskInterval);
				}

				lastScrollUpdate = Java.Lang.JavaSystem.CurrentTimeMillis();
			}
		}

		public void AnimateScrollTo(int x)
		{
			int y = 0;
			ObjectAnimator xTranslate = ObjectAnimator.OfInt(this, "scrollX", x);
			ObjectAnimator yTranslate = ObjectAnimator.OfInt(this, "scrollY", y);

			AnimatorSet animators = new AnimatorSet();
			animators.SetDuration(200);
			animators.PlayTogether(xTranslate, yTranslate);
			animators.AnimationEnd += delegate {
				IsAnimating = false;
			};
			animators.AnimationStart += delegate
			{
				IsAnimating = true;
			};
			animators.Start();
		}
	}
}
