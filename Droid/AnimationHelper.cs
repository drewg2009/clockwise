using System;
using Android.Views;
using Android.Animation;
using Android.Views.Animations;
using Clockwise.Droid;
namespace Clockwise.Droid
{
	public class AnimationHelper
	{
		AnimationManager manager;
		View v;

		private void OnAnimationEnd(object sender, EventArgs e)
		{
			manager.Animating = false;
		}

		private class UpdateLister : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
		{
			View v;
			public UpdateLister(View v)
			{
				this.v = v;
			}
			public void OnAnimationUpdate(ValueAnimator animation)
			{
				v.LayoutParameters.Height = (int)animation.AnimatedValue;
				v.RequestLayout();
			}
		}

		public AnimationHelper(View v, AnimationManager am)
		{
			this.v = v;
			this.manager = am;
		}

		public void expand(int duration, int targetHeight)
		{
			manager.Animating = true;
			int prevHeight = v.Height;
			v.Visibility = ViewStates.Visible;
			ValueAnimator valueAnimator = ValueAnimator.OfInt(prevHeight, targetHeight);
			valueAnimator.AddUpdateListener(new UpdateLister(v));
					valueAnimator.AnimationEnd += OnAnimationEnd;
			valueAnimator.SetInterpolator(new DecelerateInterpolator());
			valueAnimator.SetDuration(duration);
		    valueAnimator.Start();
		}



		public void collapse(int duration, int targetHeight)
		{
			manager.Animating = true;
			int prevHeight = v.Height;
			ValueAnimator valueAnimator = ValueAnimator.OfInt(prevHeight, targetHeight);
			valueAnimator.SetInterpolator(new DecelerateInterpolator());
			valueAnimator.AddUpdateListener(new UpdateLister(v));
			valueAnimator.AnimationEnd += OnAnimationEnd;
    		valueAnimator.SetInterpolator(new DecelerateInterpolator());
    		valueAnimator.SetDuration(duration);
    		valueAnimator.Start();
		}
	}
}
