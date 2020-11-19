using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Garen : Champion {

	public bool Q_flag, W_flag, E_flag, Passive_flag;
	Utility.Timer Q_dur_timer, Q_move_dur_timer, W_dur_timer,E_dur_timer;
	//skill stats
	float Passive_cd;
	float Q_cd = 8f, Q_silence_time = 1.5f, Q_move_time = 1.5f, Q_move_bonus = 0.3f;
	float W_bonus_armor = 0f, W_bonus_mr = 0f, W_damage_reduction = 0.3f, W_dmg_rdc_runtime = 0f, W_dur, W_defence_bonus = 0.25f;
	float E_cd = 9f, E_dur = 3f, E_single_bonus = 0.3333f, E_range = 325f; 
	float R_missing_health_ratio, R_range;
	Types.damage_combo E_damage; 
	GameObject villian, E_aoe_prefab;
	GameObject E_obj;//runtime e object

	public override void update_timers(){
		if (!netID.isLocalPlayer)
			return;

		autoAtkTimer.update_timer();
		Q_timer.update_timer();
		W_timer.update_timer();
		E_timer.update_timer();
		R_timer.update_timer();
		Passive_timer.update_timer();
		death_timer.update_timer ();

		if (!flags.is_dead) {
			regen_timer.update_timer ();
			if (regen_timer.is_over ()) {
				regen_timer.reset_timer ();
				change_health (runtime_u_stats.hp_reg);
				//TODO mana regen here
			}
		}

		//Q skills
		Q_dur_timer.update_timer ();
		if (Q_dur_timer.is_over ()) {
			Q_dur_timer.reset_timer ();
			Q_flag = false;
		}
		Q_move_dur_timer.update_timer ();
		if (Q_move_dur_timer.is_over ()) {
			Q_move_dur_timer.reset_timer ();
			bonus_u_stats_mult.move_speed -= Q_move_bonus;
			update_movement_speed ();
		}
		W_dur_timer.update_timer ();
		if (W_dur_timer.is_over ()) {
			W_dur_timer.reset_timer ();
			W_flag = false;
			W_dmg_rdc_runtime = 0f;
			update_dmg_reduction ();
		}

		E_dur_timer.update_timer ();
		if (E_dur_timer.is_over ()) {
			if(E_obj == null)
				E_dur_timer.reset_timer ();
			
			E_flag = false;
		}

		//update and possibily, clear status
		List<Status> trashbin = new List<Status>();
		foreach (Status sts in status) {
			sts.update_status();
			if (sts.is_over ())
				trashbin.Add (sts);
		}
		foreach (Status trash in trashbin) {
			flags.set_flag_from_string (trash.flags, false);
			status.Remove (trash);
		}
		trashbin.Clear ();

		if (isPlayer)
			refresh_player_panel ();
	}

	public override void refresh_player_panel(){
		if (!netID.isLocalPlayer)
			return;
		
		base.refresh_player_panel ();
	}

	public override void init(){
		init_pre ();
        ui = Utility.get_UI(transform.parent.gameObject);
		ui_champ = Utility.get_UI(transform.parent.gameObject) as UI_Champion;
		rb = GetComponent<Rigidbody> ();

		if (!netID.isLocalPlayer && isPlayer)
			return;

		death_timer = new Utility.Timer (0f);
		death_timer.change_max_timer (2f);//TODO should not be a fixed value;
		death_timer.reset_timer();

		//if (transform.parent.FindChild ("NavUnit") != null) {
		//	navagent = transform.parent.FindChild ("NavUnit").gameObject.GetComponent<NavMeshAgent> ();
		//	if (navagent != null)
		//		navagent.transform.SetParent (null);
		//}
		//navagent.updateRotation = false;
		//navagent.updatePosition = false;

		//init stats
		flags.init ();
		c_flags.init ();
		base_u_stats.init ();
		bonus_u_stats.init ();
		u_stats_growth.init ();
		base_c_stats.init ();
		runtime_c_stats.init ();
		status = new List<Status> ();

		load_garen_base_stats ();
		init_garen_skills ();
        init_post();
        /*
		//init runtime stats
		runtime_u_stats = base_u_stats;
		update_attack_speed ();
		update_movement_speed ();

		//set suto attack timers
		autoAtkTimer = new Utility.Timer(runtime_u_stats.attack_speed);
		autoAtkTimer.start_timer ();
		//rb.detectCollisions = true;
		//init appearances
		//load special FX

		//Unity stuff
		self_col = GetComponent<Collider> ();
		init_AtkTarget ();

		init_resources ();
		*/
        //navagent.updatePosition = false;
    }

	void load_garen_base_stats(){
		//base_c_stats
		base_u_stats.max_hp = 616.28f;
		base_u_stats.hp = base_u_stats.max_hp;
		base_u_stats.hp_reg = 7.84f*0.1f;
		base_u_stats.max_mana = 0.0f;
		base_u_stats.mana = 0.0f;
		base_u_stats.mana_reg = 0.0f;
		base_u_stats.damage = 57.88f;//TODO
		base_u_stats.range = 175f;
		base_u_stats.move_speed = 340.0f;//TODO
		base_u_stats.attack_speed = 0.625f;
		base_u_stats.armor = 27.536f;
		base_u_stats.magic_resist = 32.1f;
		base_u_stats.crd = 0.0f;
		base_u_stats.life_steal = 0f;

		u_stats_growth.hp = 84.25f;
		u_stats_growth.max_hp = 84.25f;
		u_stats_growth.damage = 4.5f;
		//u_stats_growth.as_bonus = 0.0029f;//as growth is considered "bonus"
		u_stats_growth.hp_reg = 0.5f;
		u_stats_growth.armor = 3.0f;
		u_stats_growth.magic_resist = 1.25f;
	}

	void init_garen_skills(){
		Q_skill = new skill ();
		Q_skill.need_indicate_target = false;
		W_skill = new skill ();
		W_skill.need_indicate_target = false;
		W_skill.cd = 24f;
		E_skill = new skill ();
		E_skill.need_indicate_target = false;
		R_skill = new skill ();
		R_skill.need_indicate_target = true;
		Passive_skill = new skill ();

		Q_timer = new Utility.Timer (Q_cd);
		Q_timer.finish ();
		W_timer = new Utility.Timer (W_skill.cd);
		W_timer.finish ();
		E_timer = new Utility.Timer (E_cd);
		E_timer.finish ();
		R_timer = new Utility.Timer (R_skill.cd);
		R_timer.finish ();
		Passive_timer = new Utility.Timer (Passive_skill.cd);

		Q_flag = false;
		W_flag = false; 
		E_flag = false;
		Passive_flag = false;

		//Q : Decisive Strike
		Q_skill.cd = 8f;
		Q_skill.dmg = 30f;
		Q_skill.ad_ratio = 1.4f;
		Q_skill.timer.change_max_timer (Q_skill.cd);
		Q_dur_timer = new Utility.Timer (4.5f);//its fixed
		Q_move_dur_timer = new Utility.Timer (Q_move_time);
		Q_silence_time = 1.5f;
		//TODO test only
		Q_skill.level = 4;
		upgrade_skill ("Q");

		//W : 
		W_dur_timer = new Utility.Timer (W_dur);

		//E : Judgement
		E_dur_timer = new Utility.Timer (E_dur);
		E_aoe_prefab = Resources.Load ("Prefabs/Champions/Garen/Garen_E_prefab") as GameObject;
		E_damage = new Types.damage_combo ();
		//TODO test only
		E_skill.level = 0;
		upgrade_skill ("E");
		W_skill.level = 0;
		upgrade_skill ("W");

		//TODO test only
		R_skill.level = 2;
		upgrade_skill ("R");
	}

	public override GameObject auto_attack(GameObject target){
		if ( (!within_range (target))||flags.is_stun )
			return null;

		if ( (!Q_flag) && (!autoAtkTimer.is_over ()) )
			return null;

		float dmg = 0f;
		Unit tgt = target.GetComponent<Unit> ();
		if (!Q_flag) {
			if (autoAtkTimer.is_over ())
				dmg = runtime_u_stats.damage;
		} else {
			if (!Q_dur_timer.is_over ()) {
				//setup the status
				Status Garen_Q_sts = new Status();
				Garen_Q_sts.init (Q_silence_time);
				Garen_Q_sts.name = "Garen_Q";
				Garen_Q_sts.flags = "s";
				Garen_Q_sts.from = gameObject;
				Garen_Q_sts.to = target;

				autoAtkTimer.finish ();
				dmg = Q_skill.dmg + Q_skill.ad_ratio * (runtime_u_stats.damage + bonus_u_stats.damage);
				tgt.receive_status (Garen_Q_sts);
				//champ.set_silent (true, Q_silence_time);
				Q_flag = false;
			}
		}

		if (tgt.receive_damage (this, true, prepare_dmg())) {
			cur_target_obj = null;
		}

		autoAtkTimer.restart ();
		print (dmg);
		return null;
	}

	public override void OnKillingTarget(Unit target){
		if (!netID.isLocalPlayer)
			return;
		//get exp
		change_exp(target.runtime_u_stats.exp);
		//get gold
		host.change_gold(target.runtime_u_stats.bounty);
		//update W skill state
		if (W_skill.level > 0) {
			if (W_bonus_armor >= 30f) {
				return;
			} else if (W_bonus_armor + W_defence_bonus < 30f) {
				W_bonus_armor += W_defence_bonus;
				W_bonus_mr += W_defence_bonus;
				update_armor ();
				update_magic_resist ();
			} else {
				W_bonus_armor = 30f;
				W_bonus_mr = 30f;
				update_armor ();
				update_magic_resist ();
			}
		}
	}

	public override void update_armor(){
		runtime_u_stats.armor = base_u_stats.armor + bonus_u_stats.armor + W_bonus_armor;
	}

	public override void update_magic_resist(){
		runtime_u_stats.magic_resist = base_u_stats.magic_resist + bonus_u_stats.magic_resist + W_bonus_mr;
	}

	public override void update_dmg_reduction(){
		runtime_u_stats.dmg_reduction = base_u_stats.dmg_reduction + bonus_u_stats.dmg_reduction + W_dmg_rdc_runtime;
	}

	//skills
	public override bool act_Q(){
		if ( (Q_skill.level>0)&&(Q_timer.is_over())&&(!Q_flag)&&can_cast() ) {
			Q_flag = true;
			Q_dur_timer.restart ();
			Q_timer.restart ();
			Q_move_dur_timer.restart ();
			bonus_u_stats_mult.move_speed += Q_move_bonus;
			update_movement_speed ();
			return true;
		}
		return false;
	}
	public override bool act_W(){
		if ((W_skill.level > 0) && (W_timer.is_over ()) && (!W_flag) && can_cast()) {
			W_flag = true;
			W_timer.restart ();
			W_dur_timer.restart ();
			W_dmg_rdc_runtime = W_damage_reduction;
			update_dmg_reduction ();
		}
		return false;
	}

	public override bool act_E(){
		if ((E_skill.level > 0) && (E_timer.is_over ()) && (!E_flag)&&can_cast()) {
			E_flag = true;
			//how many times E deals damage
			int dmg_times = 5 + (int)((runtime_c_stats.level - 1 - ((runtime_c_stats.level - 1) % 3)) / 3);
			//the total damage of E
			update_E_damage ();
			E_dur_timer.restart ();
			E_timer.restart ();
			E_obj = Instantiate (E_aoe_prefab, transform.position, transform.rotation) as GameObject;
			Types.damage_combo zero = new Types.damage_combo ();
			zero.init ();
			E_obj.GetComponent<Garen_E> ().init (this, E_dur_timer, dmg_times, E_damage);
			E_obj.name = "Garen_E_prefab";
			E_obj.transform.SetParent (transform.parent);
			return true;
		}
		return false;
	}

	public void update_E_damage(){
		E_skill.dmg = (10f + (float)(E_skill.level*4) + runtime_u_stats.damage*(0.33f+0.01f*E_skill.level)) * E_dur;
		E_damage.physical_dmg = E_skill.dmg;
		E_damage.armor_pen_percent = runtime_u_stats.armor_pen_percent;
		E_damage.armor_pen = runtime_u_stats.armor_pen;
	}

	public override bool act_R(){
		if ((R_skill.level > 0) && (R_timer.is_over ()) && can_cast() && cur_target_obj != null) {
			if (cur_target_obj.tag != GameSceneConsts.CHAMPION_TAG)
				return false;

			R_timer.restart ();
			Champion tgt = cur_target_obj.GetComponent<Champion> ();
			float missing_health = tgt.runtime_u_stats.max_hp - 
								   tgt.runtime_u_stats.hp;
			float dmg = missing_health * 0.4f + R_skill.dmg;
			print ("DEMACIA!!!!");
			if (cur_target_obj == villian)
				tgt.receive_damage (this, false, td: dmg);
			else
				tgt.receive_damage (this, false, md:dmg);
			print (dmg);
			return true;
		}
		return false;
	}
	public override bool act_Passive(){return false;}

	//upgrades
	public override void level_up(){
		base.level_up ();
	}

	public override void upgrade_skill(string str){
		if (str == "Q") {
			if (Q_skill.level >= GameSceneConsts.max_skill_level)
				return;
			else {
				Q_skill.level += 1;
				switch (Q_skill.level) {
				case 1:
					Q_move_dur_timer.change_max_timer (1.5f);
					Q_skill.dmg = 30f;
					break;
				case 2:
					Q_move_dur_timer.change_max_timer (2.0f);
					Q_skill.dmg = 55f;
					break;
				case 3:
					Q_move_dur_timer.change_max_timer (2.5f);
					Q_skill.dmg = 80f;
					break;
				case 4:
					Q_move_dur_timer.change_max_timer (3.0f);
					Q_skill.dmg = 105f;
					break;
				case 5:
					Q_move_dur_timer.change_max_timer (3.5f);
					Q_skill.dmg = 130f;
					break;
				default:
					break;
				}
			}
		} else if (str == "W") {
			if (W_skill.level >= GameSceneConsts.max_skill_level)
				return;
			else {
				W_skill.level += 1;
				if (W_skill.level <= 0) {
					W_skill.cd = 24f;
					W_dur = 2f;
				} else {
					W_skill.cd = 25f - (float)W_skill.level;
					W_dur = 1 + (float)W_skill.level;
				}
				W_dur_timer.change_max_timer (W_dur);
				W_skill.timer.change_max_timer (W_skill.cd);
			}
		} else if (str == "E") {
			if (E_skill.level >= GameSceneConsts.max_skill_level)
				return;
			else {
				E_skill.level += 1;
				//update damage upon activate the skill
			}
		} else if (str == "R") {
			if (R_skill.level > GameSceneConsts.max_ult_level)
				return;
			else {
				R_skill.level += 1;
				R_skill.dmg = (float)R_skill.level * 175f;
				R_skill.cd = 140f - (float)R_skill.level * 20f;
				R_timer.change_max_timer (R_skill.cd);
			}
		}
	}
}
