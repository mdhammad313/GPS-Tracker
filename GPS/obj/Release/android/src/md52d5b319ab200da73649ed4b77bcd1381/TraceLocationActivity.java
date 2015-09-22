package md52d5b319ab200da73649ed4b77bcd1381;


public class TraceLocationActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("GPS.TraceLocationActivity, GPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TraceLocationActivity.class, __md_methods);
	}


	public TraceLocationActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == TraceLocationActivity.class)
			mono.android.TypeManager.Activate ("GPS.TraceLocationActivity, GPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
