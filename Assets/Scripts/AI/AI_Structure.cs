using UnityEngine;
using System.Collections;

public class AI_Structure : AI {

	Structure structure;
	//for player, target is HOSTILE, decision is HOLD

	public override void init_special(){
		structure = (Structure)unit;
	}

	// Update is called once per frame
	public override void Update () {

	}

	public override void think(){			

	}
}
