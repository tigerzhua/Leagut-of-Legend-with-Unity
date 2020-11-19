using UnityEngine;
using System.Collections;

public class CatchUPUnit : MonoBehaviour {

	public Unit host;

	// Update is called once per frame
	void Update () {
		if (host == null)
			host = Utility.get_unit_script(gameObject);

		transform.position = host.transform.position;
	}
}
