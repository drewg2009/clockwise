using System;
using Android.Views;
using Android.Animation;
using Android.Views.Animations;
using Clockwise.Droid;
using Android.Widget;

namespace Clockwise.Droid
{
	public class AnimationHelper
	{
		AnimationManager manager;
		View v;

		private void OnHeightAnimationEnd(object sender, EventArgs e)
		{
			manager.Animating = false;
			//if(v.LayoutParameters.Height != 0)
			//	v.LayoutParameters.Height = RelativeLayout.LayoutParams.WrapContent;
		}

		private void OnAlphaAnimationEnd(object sender, EventArgs e)
		{
			//manager.Animating = false;
			v.Visibility = (v.Alpha < 0.5f) ?
					ViewStates.Invisible : ViewStates.Visible;
		}

		private class HeightUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
		{
			View v;
			public HeightUpdateListener(View v)
			{
				this.v = v;
			}
			public void OnAnimationUpdate(ValueAnimator animation)
			{
				v.LayoutParameters.Height = (int)animation.AnimatedValue;
				v.RequestLayout();
			}
		}

		private class AlphaUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
		{
			View v;
			public AlphaUpdateListener(View v)
			{
				this.v = v;
			}
			public void OnAnimationUpdate(ValueAnimator animation)
			{
				v.Alpha = (float)animation.AnimatedValue;
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
			valueAnimator.AddUpdateListener(new HeightUpdateListener(v));
					valueAnimator.AnimationEnd += OnHeightAnimationEnd;
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
			valueAnimator.AddUpdateListener(new HeightUpdateListener(v));
			valueAnimator.AnimationEnd += OnHeightAnimationEnd;
    		valueAnimator.SetInterpolator(new DecelerateInterpolator());
    		valueAnimator.SetDuration(duration);
    		valueAnimator.Start();
		}

		public void Fade(int duration, float targetAlpha)
		{
			//manager.Animating = true;
			float prevAlpha = v.Alpha;
			ValueAnimator valueAnimator = ValueAnimator.OfFloat(prevAlpha, targetAlpha);
			valueAnimator.SetInterpolator(new DecelerateInterpolator());
			valueAnimator.AddUpdateListener(new AlphaUpdateListener(v));
			valueAnimator.AnimationEnd += OnAlphaAnimationEnd;
			valueAnimator.SetInterpolator(new DecelerateInterpolator());
			valueAnimator.SetDuration(duration);
			valueAnimator.Start();
		}
	}
}
