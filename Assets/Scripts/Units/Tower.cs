using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : Unit {

	public enum Tower_type{
		OUTER,
		INNER,
		INHIB,
		NEXUS,
	}

	public Tower_type type;

	public void Towerinit(Types.Faction _fac, Tower.Tower_type _type){
		init ();
		fac = _fac;
		type = _type;
	}

	public override void init (){
		init_pre ();
		load_tower_stats ();
		runtime_u_stats = base_u_stats;
		ui = Utility.get_UI(transform.parent.gameObject);
		init_post_tower ();
	}

	public void init_post_tower(){

		runtime_u_stats = base_u_stats;
		update_attack_speed ();
		autoAtkTimer = new Utility.Timer(runtime_u_stats.attack_speed);
		autoAtkTimer.start_timer ();
		init_AtkTarget ();
		init_resources ();
	}

	void load_tower_stats(){
		flags.ranged = true;
		switch (type) {
		case Tower_type.OUTER:
			base_u_stats.max_hp = 3500f;
			base_u_stats.hp = base_u_stats.max_hp;
			base_u_stats.damage = 152f;
			base_u_stats.range = 700f;
			base_u_stats.projectile_spd = 600f;
			base_u_stats.attack_speed = 0.83f;
			base_u_stats.armor = 40f;
			base_u_stats.magic_resist = 40f;
			break;
		case Tower_type.INNER:
			base_u_stats.max_hp = 3300f;
			base_u_stats.hp = base_u_stats.max_hp;
			base_u_stats.damage = 170f;
			base_u_stats.range = 700f;
			base_u_stats.projectile_spd = 600f;
			base_u_stats.attack_speed = 0.83f;
			base_u_stats.armor = 40f;
			base_u_stats.magic_resist = 40f;
			break;
		case Tower_type.INHIB:
			base_u_stats.max_hp = 3300f;
			base_u_stats.hp = base_u_stats.max_hp;
			base_u_stats.damage = 170f;
			base_u_stats.range = 700f;
			base_u_stats.projectile_spd = 600f;
			base_u_stats.attack_speed = 4f;
			base_u_stats.armor = 40f;
			base_u_stats.magic_resist = 40f;
			break;
		case Tower_type.NEXUS://TODO it's same as inhib now
			base_u_stats.max_hp = 3300f;
			base_u_stats.hp = base_u_stats.max_hp;
			base_u_stats.damage = 170f;
			base_u_stats.range = 700f;
			base_u_stats.projectile_spd = 600f;
			base_u_stats.attack_speed = 4f;
			base_u_stats.armor = 40f;
			base_u_stats.magic_resist = 40f;
			break;
		default:
			break;
		}
	}
	
	// Update is called once per frame
	public override void Update () {
		if (!netID.isServer)
			return;
		
		update_timers ();
	}
}
