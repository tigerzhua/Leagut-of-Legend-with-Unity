using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minion : Unit {

	public enum Minion_Type{
		MELEE,
		CASTER,
		CANNON,
		SUPER,
		OTHER,
	}
		
	public Minion_Type type;

	//used in game
	List<GameObject> wps;//wap points of moving
	int cur_tgt_idx;
	public GameObject WpTarget;//the target to move to

	// Use this for initialization
	void Start () {
	}

	//init functions, it's not the same one in Unit, so cannot be called directly from Unit
	public void init(List<GameObject> _wps, Minion_Type _type = Minion_Type.OTHER, Types.Faction _fac = Types.Faction.OTHER){
		init_pre ();

		type = _type;
		fac = _fac;
		cur_tgt_idx = 0;
		rb = GetComponent<Rigidbody> ();
		ui = Utility.get_UI(transform.parent.gameObject);
		//navagent = transform.parent.FindChild ("NavUnit").gameObject.GetComponent<NavMeshAgent> ();
		//navagent.updateRotation = false;
		//navagent.transform.SetParent (null);
		//init mesh and stats
		switch (_type) {
		case Minion_Type.MELEE:
			load_stats_MELEE ();
			break;
		case Minion_Type.CASTER:
			load_stats_CASTER ();
			break;
		case Minion_Type.CANNON:
			load_stats_CANNON ();
			break;
		case Minion_Type.SUPER:
			load_stats_SUPER ();
			break;
		default:
			load_stats_DEFAULT ();//this one should not be called
			break;
		}

		//TODO: temp set texture color
		switch (_fac) {
		case Types.Faction.BLUE:
			break;
		case Types.Faction.RED:
			break;
		}

		wps = _wps;
		if (wps.Count > 0)
			moveto (wps [0]);

		init_post ();
	}

	void load_stats_MELEE(){
		base_u_stats.max_hp = 455f;
		base_u_stats.hp = base_u_stats.max_hp;
		base_u_stats.hp_reg = 0f;
		base_u_stats.mana = 0f;
		base_u_stats.mana_reg = 0f;
		base_u_stats.damage = 12f;
		base_u_stats.range = 110f;
		base_u_stats.move_speed = 340.0f;
		base_u_stats.attack_speed = 1.25f;
		base_u_stats.armor = 0f;
		base_u_stats.magic_resist = 0f;
		base_u_stats.crd = 0f;
		base_u_stats.life_steal = 0.0f;
	}

	void load_stats_CASTER(){//TODO place holder
		flags.ranged = true;
		base_u_stats.max_hp = 455f;
		base_u_stats.hp = base_u_stats.max_hp;
		base_u_stats.hp_reg = 0f;
		base_u_stats.mana = 0f;
		base_u_stats.mana_reg = 0f;
		base_u_stats.damage = 12f;
		base_u_stats.range = 500f;
		base_u_stats.move_speed = 340.0f;
		base_u_stats.attack_speed = 1.25f;
		base_u_stats.armor = 0f;
		base_u_stats.magic_resist = 0f;
		base_u_stats.crd = 0f;
		base_u_stats.life_steal = 0.0f;
	}

	void load_stats_CANNON(){//TODO place holder
		base_u_stats.max_hp = 455f;
		base_u_stats.hp = base_u_stats.max_hp;
		base_u_stats.hp_reg = 0f;
		base_u_stats.mana = 0f;
		base_u_stats.mana_reg = 0f;
		base_u_stats.damage = 12f;
		base_u_stats.range = 110f;
		base_u_stats.move_speed = 340.0f;
		base_u_stats.attack_speed = 1.25f;
		base_u_stats.armor = 0f;
		base_u_stats.magic_resist = 0f;
		base_u_stats.crd = 0f;
		base_u_stats.life_steal = 0.0f;
	}

	void load_stats_SUPER(){
	}

	void load_stats_DEFAULT(){
	}
	
	// Update is called once per frame
	public override void Update () {
		if (!netID.isServer)
			return;
		
		update_timers ();

		if (!flags.in_combat || cur_target_obj == null) {
			transform.LookAt (new Vector3 (navagent.steeringTarget.x, transform.position.y, navagent.steeringTarget.z));
		} else {
			transform.LookAt (new Vector3 (cur_target_obj.transform.position.x, transform.position.y, cur_target_obj.transform.position.z));
		}
		transform.position = navagent.transform.position;

		if (cur_tgt_idx < wps.Count-1) {
			if (reached (wps[cur_tgt_idx])) {
				moveto (wps [cur_tgt_idx + 1]);
				cur_tgt_idx += 1;
			}
		}
	}

	//runtime actions
	public override bool moveto(GameObject tgt){
		WpTarget = tgt;
		navagent.SetDestination (tgt.transform.position);
		return true;
	}

	public override void process_death (){
		base.process_death ();
		//TODO test only
		Destroy (navagent.gameObject);
		Destroy (transform.parent.gameObject);
	}

	public override void return_to_objective(){
		if (cur_tgt_idx < wps.Count-1) {
			if (reached (wps [cur_tgt_idx])) {
				moveto (wps [cur_tgt_idx + 1]);
				cur_tgt_idx += 1;
			} else {
				moveto (wps [cur_tgt_idx]);
			}
		}
	}

	//move to a target until within range
	public override bool attackto(GameObject tgt){
		if (tgt == null)
			return false;
		
		moveto (tgt);
		cur_target_obj = tgt;
		return true;
	}

	public bool reached(GameObject tgt){
		if (Utility.get_dist_gameobject(gameObject, tgt) <= GameSceneConsts.minion_wp_error)
			return true;

		return false;
	}
}
