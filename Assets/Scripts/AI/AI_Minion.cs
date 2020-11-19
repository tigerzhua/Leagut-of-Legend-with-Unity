using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Minion : AI {

	Minion minion;

	public override void init_special(){
		minion = (Minion)unit;
	}
	
	// Update is called once per frame
	//void Update () {
	//
	//}

	public override void think(){
		move_back_list_element (temp_untargetable);
		minion.AtkTarget.GetComponent<AttackTarget> ().move_back_list_element ();
		clean_up_all_lists ();
		switch (target) {
		case PotentialTarget.ENEMY_TEAM:
			//if it's not yet engaged, randomly choose a target
			if (minion.cur_target_obj == null) {
				minion.attackto (pick_an_enemy ());
			} else {//in combat
				if (minion.cur_target_obj != null) {
					if ((minion.cur_target_obj.tag == GameSceneConsts.CHAMPION_TAG) && (!minion.within_range (minion.cur_target_obj)))
						minion.attackto (pick_an_enemy ());
				} else {//cur target is null
					minion.attackto (pick_an_enemy ());
				}
			}
			break;
		default:
			break;
		}
	}
}
