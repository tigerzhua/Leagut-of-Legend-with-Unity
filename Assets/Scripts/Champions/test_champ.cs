using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class test_champ : Champion {
	// Update is called once per frame
	//void Update () {
	//	update_timers ();
	//}

	void Start() {
		//Unity stuff
		fac = Types.Faction.RED;//TODO test only
		rb = GetComponent<Rigidbody> ();

		//init stats
		flags.init ();
		c_flags.init ();
		base_u_stats.init ();
		base_c_stats.init ();
		runtime_c_stats.init ();
		runtime_c_stats.level = 10;

		load_garen_base_stats ();
		//init runtime stats
		runtime_u_stats = base_u_stats;

		//set timers
		autoAtkTimer = new Utility.Timer(runtime_u_stats.attack_speed);
		Q_timer = new Utility.Timer (0f);
		W_timer = new Utility.Timer (0f);
		E_timer = new Utility.Timer (0f);
		R_timer = new Utility.Timer (0f);
		Passive_timer = new Utility.Timer (0f);
		status = new List<Status> ();
		//rb.detectCollisions = true;
		//init appearances
		//load special FX
	}

	void load_garen_base_stats(){
		//base_c_stats
		base_u_stats.max_hp = 1616.0f;
		base_u_stats.hp = base_u_stats.max_hp;
		base_u_stats.hp_reg = 7.8f;
		base_u_stats.mana = 0.0f;
		base_u_stats.mana_reg = 0.0f;
		base_u_stats.damage = 58.0f;
		base_u_stats.range = 175f;
		base_u_stats.move_speed = 340.0f;
		base_u_stats.attack_speed = 0.625f;
		base_u_stats.armor = 27.5f;
		base_u_stats.magic_resist = 32.1f;
		base_u_stats.crd = 0.0f;
		base_u_stats.life_steal = 0.0f;
	}

	public override bool act_Q(){return false;}
	public override bool act_W(){return false;}
	public override bool act_E(){return false;}
	public override bool act_R(){return false;}
	public override bool act_Passive(){return false;}
}
