using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Garen_E : MonoBehaviour {

	public List<Unit> targets;
	CapsuleCollider self_col;
	Types.damage_combo dmg, actual_dmg;
	Utility.Timer dur_timer;//the REFERENCE of the timer, which is 3s
	float timeStep;
	Garen host;

	int dmgTimeRemaining;
	int maxDmgTime;
	// Use this for initialization
	public void init (Garen _host, Utility.Timer E_timer, int damageTime, Types.damage_combo _dmg) {
		targets = new List<Unit> ();
		host = _host;
		self_col = GetComponent<CapsuleCollider> ();
		self_col.radius = 325f * GameSceneConsts.dist_multiplier;
		dur_timer = E_timer;
		dmg = _dmg;
		//how many times E deals damage
		dmgTimeRemaining = damageTime;
		maxDmgTime = damageTime;
		//it become damage per time
		dmg.physical_dmg /= (float)damageTime;
		timeStep = 3f*100f / (float)damageTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (dur_timer.get_time_remaining() <= 3f*100f - (maxDmgTime - dmgTimeRemaining)*timeStep) {
			dmgTimeRemaining -= 1;
			float dmg_modifier = 1f;
			if (targets.Count == 1) {
				if (targets [0] != null) {
					if (targets [0].gameObject.tag == GameSceneConsts.MINION_TAG)
						dmg_modifier *= (1.33f * 0.75f);
					else
						dmg_modifier *= 1.33f;

					if (Utility.is_crit (host.runtime_u_stats.crit_chance))
						dmg_modifier *= 2f;
					
					actual_dmg = dmg_modifier * dmg;
					if (targets [0] != null)
						targets [0].receive_damage (host, false, actual_dmg);
				}
			} else {
				foreach(Unit tgt in targets){
					if (tgt != null) {
						dmg_modifier = 1f;
						if (Utility.is_crit (host.runtime_u_stats.crit_chance))
							dmg_modifier *= 2f;
					
						if (tgt.gameObject.tag == GameSceneConsts.MINION_TAG) {
							dmg_modifier *= 0.75f;
							actual_dmg = dmg_modifier * dmg;
							if (tgt != null)
								tgt.receive_damage (host, false, actual_dmg);
						} else {
							if (tgt != null)
								tgt.receive_damage (host, false, dmg);
						}
					}
				}
			}
		}

		if (dur_timer.is_over () || (dmgTimeRemaining <= 0))
			self_destroy ();
	}

	public void self_destroy(){
		Destroy (gameObject);
	}

	//maintain the list
	void OnTriggerEnter(Collider col){
		if ( (!Utility.TagUnit(col.gameObject))||(col == host.self_col)||(col == self_col) )
			return;
		
		Unit obj = Utility.get_unit_script_peer (col.gameObject);
		if (obj == null)
			return;
		else if ((obj.fac != host.fac)&&(!targets.Contains(obj)))
			targets.Add (obj);
	}

	void OnTriggerStay(Collider col){
		OnTriggerEnter (col);
	}

	void OnTriggerExit(Collider col){
		if ( (!Utility.TagUnit(col.gameObject))||(col == host.self_col)||(col == self_col) )
			return;

		Unit obj = Utility.get_unit_script_peer (col.gameObject);
		if (obj == null)
			return;
		else if (targets.Contains (obj))
			targets.Remove (obj);
	}
}
