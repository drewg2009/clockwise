using System;
using Android.Views.Animations;
using Android.Views;
using Android.Widget;
namespace Clockwise.Droid
{
	public class AnimationManager
	{
		private bool animating = false;
		private bool open;

		public AnimationManager(bool isOpen)
		{
			this.open = isOpen;
		}

		public bool Animating
		{
			get { return animating;}
			set { animating = value; }
		}

		public bool Open
		{
			get { return open; }
			set { open = value; }
		}
	}
}
