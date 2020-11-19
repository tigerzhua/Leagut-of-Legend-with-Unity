using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncDB : NetworkBehaviour {

	public SyncListInt testlist;

	public override void OnStartServer ()
	{
		print ("gulu");
		init ();
	}

	public void init(){
		print ("init");
		testlist = new SyncListInt();
		testlist.Callback = testlistCB;
	}

	public void testlistCB(SyncListInt.Operation op, int idx){
		testlist.Add(-1);
		print (testlist.Count);
	}
}
