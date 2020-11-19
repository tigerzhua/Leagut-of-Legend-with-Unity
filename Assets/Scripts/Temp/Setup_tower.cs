using UnityEngine;
using System.Collections;

public class Setup_tower: MonoBehaviour {

	// Use this for initialization
	void Start () {
		Unit scp = Utility.get_unit_script (gameObject);
		scp.init ();
		Utility.get_AI_script (gameObject).init (scp, scp.self_col, AI.PotentialTarget.ENEMY_TEAM, AI.Decision.HOLD);
		Utility.get_UI_obj (gameObject).GetComponent<UI_structure> ().init ();
	}
}
