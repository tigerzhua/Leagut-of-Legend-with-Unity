using UnityEngine;
using System.Collections;

public class AI_Player : AI {

	Champion champ;
	//for player, target is HOSTILE, decision is HOLD

	public override void init_special(){
		champ = (Champion)unit;
		holdedPos = transform.position;
		vision.radius = (GameSceneConsts.ai_max_chase_range + champ.runtime_u_stats.range) * GameSceneConsts.dist_multiplier;
	}

	// Update is called once per frame
	public override void Update () {
		if (!champ.netID.isLocalPlayer)
			return;

		if ((!unit.flags.is_dead) && champ.ai_takeover) {
			ThinkTimer.update_timer ();
			if (ThinkTimer.is_over ()) {
				clean_up_all_lists ();
				think ();
				ThinkTimer.restart ();
			}

			//this is "HOLD"
			//if we're aiming at a target, move until within range, and attack
			if (unit.cur_target_obj != null) {
				if ((Utility.LayerTargetable (unit.cur_target_obj)) &&
				    Utility.get_dist_position (holdedPos, unit.cur_target_obj.transform.position) <= GameSceneConsts.ai_max_chase_range) {
					if (unit.within_range (unit.cur_target_obj)) {
						unit.stop_moving ();
						//unit.auto_attack (unit.cur_target_obj);
					} else {//we have to move close to the target
						unit.resume_moving ();
						unit.attackto (unit.cur_target_obj);
					}
				} else {
					unit.resume_moving ();
					unit.cur_target_obj = null;
				}
			} else {//continue with the objective
				unit.resume_moving ();
				if (Utility.get_dist_position (holdedPos, champ.transform.position) >= 0.25f) {
					champ.moveto (holdedPos);
				}
			}
		} else if (!unit.flags.is_dead) {
			unit.resume_moving ();
			holdedPos = champ.transform.position;
		} else if (unit.flags.is_dead) {
			if (unit.fac == Types.Faction.BLUE) {
				holdedPos = GameSceneConsts.blue_team_respawn_pt.transform.position;
			} else if (unit.fac == Types.Faction.RED) {
				holdedPos = GameSceneConsts.blue_team_respawn_pt.transform.position;
			}
		}
	}

	public override void think(){			
		move_back_list_element (temp_untargetable);
		champ.AtkTarget.GetComponent<AttackTarget> ().move_back_list_element ();
		switch (target) {
		case PotentialTarget.HOSTILE:
		//if it's not yet engaged, randomly choose a target
			if (champ.cur_target_obj == null) {
				GameObject tgt = pick_an_enemy ();
				champ.cur_target_obj = tgt;
			}
			break;
		default:
			break;
		}
	}
}
