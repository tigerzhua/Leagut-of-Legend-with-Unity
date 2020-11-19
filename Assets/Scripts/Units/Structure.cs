using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Structure : Unit {

	public enum Structure_type{
		INHIB,
		NEXUS,
	}

	public Structure_type type;
	Utility.Timer struct_timer;//for respawn
	public List<GameObject> guardians;//struct can only be targeted when its guardians are all dead

	public void Structinit(Types.Faction _fac, Structure.Structure_type _type){
		init ();
		fac = _fac;
		type = _type;
	}

	public override void init (){
		init_pre ();
		ui = Utility.get_UI(transform.parent.gameObject);
		struct_timer = new Utility.Timer (1f);
		load_structure_stats ();
		runtime_u_stats = base_u_stats;
		init_AtkTarget ();
		init_resources ();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (!netID.isServer)
			return;

		update_timers ();
		bool all_dead = true;
		for (int i = 0; i < guardians.Count; ++i) {
			if (guardians [i].layer != GameSceneConsts.LAYER_DEAD)
				all_dead = false;
		}

		if ( (all_dead)&&(!flags.is_dead) )
			activate ();
	}

	public override void update_timers (){
		struct_timer.update_timer ();
	}

	public void activate(){
		gameObject.layer = GameSceneConsts.LAYER_DEFAULT;
	}

	public void load_structure_stats(){
		if (type == Structure_type.INHIB) {
			base_u_stats.max_hp = 3300f;
			base_u_stats.hp = base_u_stats.max_hp;
			base_u_stats.armor = 40f;
			base_u_stats.magic_resist = 40f;
		} else if (type == Structure_type.NEXUS) {
			base_u_stats.max_hp = 3300f;
			base_u_stats.hp = base_u_stats.max_hp;
			base_u_stats.armor = 40f;
			base_u_stats.magic_resist = 40f;
		}
	}
}
