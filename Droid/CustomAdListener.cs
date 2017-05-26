class CustomAdlistener : Android.Gms.Ads.AdListener
{
	// Declare the delegate (if using non-generic pattern).
	public delegate void AdLoadedEvent();
	public delegate void AdClosedEvent();
	public delegate void AdOpenedEvent();
	// Declare the event.
	public event AdLoadedEvent AdLoaded;
	public event AdClosedEvent AdClosed;
	public event AdOpenedEvent AdOpened;
	public override void OnAdLoaded()
	{
		if (AdLoaded != null) this.AdLoaded();
		base.OnAdLoaded();
	}
	public override void OnAdClosed()
	{

	}
	public override void OnAdOpened()
	{
		if (AdOpened != null) this.AdOpened();
		base.OnAdOpened();
	}
}