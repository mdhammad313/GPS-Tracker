package md52d5b319ab200da73649ed4b77bcd1381;


public class GelLocation_MyLocationReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("GPS.GelLocation+MyLocationReceiver, GPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GelLocation_MyLocationReceiver.class, __md_methods);
	}


	public GelLocation_MyLocationReceiver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == GelLocation_MyLocationReceiver.class)
			mono.android.TypeManager.Activate ("GPS.GelLocation+MyLocationReceiver, GPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public GelLocation_MyLocationReceiver (md52d5b319ab200da73649ed4b77bcd1381.GelLocation p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == GelLocation_MyLocationReceiver.class)
			mono.android.TypeManager.Activate ("GPS.GelLocation+MyLocationReceiver, GPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "GPS.GelLocation, GPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
