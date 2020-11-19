using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundAoe : MonoBehaviour {
	public List<GameObject> targets;
	Collider self;
	bool affect_champions, affect_minions, affect_neturals;
	bool is_continuous, can_crit;
	Types.damage_combo damage, start_damage, end_damage;
	float duration;
	float single_target_bonus;
	int max_damage_times, done_damage_times;
	Utility.Timer dur_timer, dmg_timer;
	Status enter_status, end_status;

	//run time
	Types.damage_combo damage_final;
	bool sharing_dur, sharing_dmg;//if this aoe circle shares timer with its owner
	CapsuleCollider clder;

	public void init(Collider slf, float radius, bool continuous,
		Types.damage_combo dmg, Types.damage_combo start_dmg, Types.damage_combo end_dmg,
		float dur = 0, int times_of_dmg = 0,//paras for continuous damage
		bool crit = false,
		float single_tgt_bonus = 0,//if only one target in there, any additional damage?
		bool champ = true, bool minion = true, bool neturals = true,
		Utility.Timer share_dur = null, Utility.Timer share_dmg = null,
		Status enter_sts = default(Status), Status end_sts = default(Status)){

		self = slf;
		duration = dur;
		damage = dmg;
		start_damage = start_dmg;
		end_damage = end_dmg;
		max_damage_times = times_of_dmg;
		done_damage_times = 0;
		can_crit = crit;
		single_target_bonus = single_tgt_bonus;
		enter_status = enter_sts;
		end_status = end_sts;

		affect_champions = champ;
		affect_minions = minion;
		affect_neturals = neturals;
		is_continuous = continuous;
		if (is_continuous) {
			if (share_dur == null) {
				sharing_dur = false;
				dur_timer = new Utility.Timer (dur);
				dur_timer.start_timer ();
			} else {
				sharing_dur = true;
				dur_timer = share_dur;
			}

			if (share_dmg == null) {
				sharing_dmg = false;
				dmg_timer = new Utility.Timer ((float)(duration / max_damage_times));
				dmg_timer.start_timer ();
			} else {
				sharing_dmg = true;
				dmg_timer = share_dmg;
			}
		}

		clder = GetComponent<CapsuleCollider> ();
		clder.radius = radius*GameSceneConsts.dist_multiplier;
	}

	// Update is called once per frame
	void Update () {
		damage_final = damage;
		if(targets.Count == 1)
			damage_final = (1f+single_target_bonus)*damage;

		if (is_continuous) {
			if(!sharing_dmg)
				dmg_timer.update_timer ();

			if (dmg_timer.is_over()) {
				foreach (GameObject obj in targets) {
					Unit unit = obj.GetComponent<Unit> ();
					if (unit != null) {
						//unit.receive_damage (damage_final);
						if (!unit.has_status (enter_status.name))
							unit.receive_status (enter_status);
					}
				}
				dmg_timer.restart ();
				done_damage_times += 1;
			}

			if(!sharing_dur)
				dur_timer.update_timer ();

			if (dur_timer.is_over ()) {
				if (done_damage_times < max_damage_times) {
					foreach (GameObject obj in targets) {
						Unit unit = obj.GetComponent<Champion> ();
						if (unit != null) {
							//unit.receive_damage ((float)(max_damage_times - done_damage_times) * damage_final);
							if (!unit.has_status (enter_status.name))
								unit.receive_status (end_status);
						}
					}
				}
				skill_prefab_destroy_sequence ();
			}
		}
	}

	void skill_prefab_destroy_sequence (){
		targets.Clear ();
		Destroy (gameObject);
	}

	//maintain the list
	void OnTriggerEnter(Collider col){
		if (col == self)
			return;

		if ( (col.gameObject.tag == GameSceneConsts.CHAMPION_TAG)&&affect_champions )
			targets.Add(col.gameObject);
		else if ( (col.gameObject.tag == GameSceneConsts.MINION_TAG)&&affect_minions )
			targets.Add(col.gameObject);
		else if ( (col.gameObject.tag == GameSceneConsts.NETURAL_TAG)&&affect_neturals )
			targets.Add(col.gameObject);
	}
	void OnTriggerStay(Collider col){
		if (col == self)
			return;

		if (!targets.Contains (col.gameObject)) {
			if ((col.gameObject.tag == GameSceneConsts.CHAMPION_TAG) && affect_champions)
				targets.Add (col.gameObject);
			else if ((col.gameObject.tag == GameSceneConsts.MINION_TAG) && affect_minions)
				targets.Add (col.gameObject);
			else if ((col.gameObject.tag == GameSceneConsts.NETURAL_TAG) && affect_neturals)
				targets.Add (col.gameObject);
		}
	}
	void OnTriggerExit(Collider col){
		//if (col == self)
		//	return;

		if (targets.Contains (col.gameObject)) {
			if ((col.gameObject.tag == GameSceneConsts.CHAMPION_TAG) && affect_champions)
				targets.Add (col.gameObject);
			else if ((col.gameObject.tag == GameSceneConsts.MINION_TAG) && affect_minions)
				targets.Add (col.gameObject);
			else if ((col.gameObject.tag == GameSceneConsts.NETURAL_TAG) && affect_neturals)
				targets.Add (col.gameObject);
		} else {
			string s = "WARNING: Round_AOE failed to detect an object: " + col.gameObject.name;
			print (s);
		}
	}
}
