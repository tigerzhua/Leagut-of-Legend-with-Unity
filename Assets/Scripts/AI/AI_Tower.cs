using UnityEngine;
using System.Collections;

public class AI_Tower : AI {

	Tower tower;

	public override void init_special(){
		tower = (Tower)unit;
		decision = Decision.HOLD;
		target = PotentialTarget.ENEMY_TEAM;
		vision.radius = tower.runtime_u_stats.range*GameSceneConsts.dist_multiplier;
	}

	// Update is called once per frame
	public override void Update () {
		//..and not a server-controlled object
		if (!unit.netID.isServer)
			return;

		if (!tower.flags.is_dead) {
			ThinkTimer.update_timer ();
			if (ThinkTimer.is_over ()) {
				clean_up_all_lists ();
				think ();
				ThinkTimer.restart ();
			}

			//if we're aiming at a target, move until within range, and attack
			if (unit.cur_target_obj != null) {
				if (unit.within_range (unit.cur_target_obj)) {
					unit.auto_attack (unit.cur_target_obj);
				}
			} else {//continue with the objective
				unit.flags.in_combat = false;
			}
		}
	}

	public override void think(){	
		move_back_list_element (temp_untargetable);
		tower.AtkTarget.GetComponent<AttackTarget> ().move_back_list_element ();
		switch (target) {
		case PotentialTarget.ENEMY_TEAM:
			//if it's not yet engaged, randomly choose a target
			bool need_refresh = false;
			//either the target is dead, or current target get untargetable
			if (tower.cur_target_obj == null)
				need_refresh = true;
			else if ((!tower.flags.in_combat) || (!Utility.LayerTargetable (tower.cur_target_obj)) || tower.within_range(tower.cur_target_obj))
				need_refresh = true;
			
			if (need_refresh){
				if (objWithin_Hostile_minion.Count > 0) {//normal: attack minion if can
					GameObject tgt = pick_a_minion ();
					if(Utility.LayerTargetable(tgt))
						tower.cur_target_obj = tgt;
				} else if (objWithin_Hostile_champ.Count > 0){
					GameObject tgt = pick_a_champion ();
					if (Utility.LayerTargetable (tgt)) {
						tower.cur_target_obj = tgt;
					}
				}
			}
			break;
		default:
			break;
		}
	}
}
