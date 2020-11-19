using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Champion : Unit {
	//defines
	//those things are unique to champions, more general ones are in Unit.cs
	public struct champion_flags{
		public void init(){
		}
	}

	public struct champion_stats{
		public int level;
		public float exp;
		public float level_up_exp;

		public void init(){
			level = 1;
			exp = 0f;
			level_up_exp = 100f;
		}
	}

	public enum champion_name{
		START,//dummy
		Garen,
		Ashe,
		Ryze,
		END,//dummy
	}

	public enum Skill{
		Q,
		W,
		E,
		R,
		P,
		D,
		F,
		B,
	}
		
	public Champion.champion_flags c_flags;
	public Champion.champion_stats base_c_stats;
	public Champion.champion_stats runtime_c_stats;
	public skill Q_skill, W_skill, E_skill, R_skill, Passive_skill;
	public Utility.Timer Q_timer, W_timer, E_timer, R_timer, Passive_timer;
	public Utility.Timer death_timer;
	public Utility.Timer ai_takeover_timer;
	public bool rotation_override;
	public bool ai_takeover;
	public UI_Champion ui_champ;

	//Virtuals
	public override void init(){
        ui = Utility.get_UI(transform.parent.gameObject);
		ui_champ = Utility.get_UI(transform.parent.gameObject) as UI_Champion;
		fac = Types.Faction.OTHER;
		autoAtkTimer = new Utility.Timer (1f);
		Q_timer = new Utility.Timer (0f);
		W_timer = new Utility.Timer (0f);
		E_timer = new Utility.Timer (0f);
		R_timer = new Utility.Timer (0f);
		Passive_timer = new Utility.Timer (0f);
		death_timer = new Utility.Timer (0f);
		//not using this for now
		ai_takeover_timer = new Utility.Timer (GameSceneConsts.ai_takeover_time);
		status = new List<Status> ();
		self_col = GetComponent<Collider> ();
		init_AtkTarget ();
		init_resources ();
		//navagent = transform.parent.FindChild("NavUnit").gameObject.GetComponent<NavMeshAgent>();
		//navagent.transform.SetParent (null);
		//navagent.gameObject.AddComponent (Network);
		//navagent.updateRotation = false;
		//navagent.updatePosition = false;
		rotation_override = false;
	}
	//skills related
	public virtual bool act_Q(){return false;}
	public virtual bool act_W(){return false;}
	public virtual bool act_E(){return false;}
	public virtual bool act_R(){return false;}
	public virtual bool act_Passive(){return false;}

	public virtual void upgrade_skill(string str){}
	public virtual void upgrade_skill_bonus(string str){}//like Garen's W, or Nasus's Q

	public override void process_death(){
		base.process_death ();
		death_timer.start_timer ();
		navagent.Stop ();
		navagent.enabled = false;
	}

	public override void OnKillingTarget(Unit target){
		//get exp
		change_exp(target.runtime_u_stats.exp);
		//get gold
		host.change_gold(target.runtime_u_stats.bounty);
	}

	public bool change_exp(float _exp){
		if (runtime_c_stats.level >= GameSceneConsts.max_level)
			return false;
		
		runtime_c_stats.exp += _exp;
		if (runtime_c_stats.exp >= runtime_c_stats.level_up_exp) {
			level_up ();
			runtime_c_stats.exp -= runtime_c_stats.level_up_exp;
		}
		return true;
	}

	public virtual void level_up(){
		if (runtime_c_stats.level + 1 <= GameSceneConsts.max_level)
			runtime_c_stats.level += 1;
		else {
			runtime_c_stats.level = GameSceneConsts.max_level;
			return;
		}

		ui_champ.update_level ();
		//the attack speed bonus is a little bit trickier
		bonus_u_stats_mult.attack_speed += 0.0029f;
		base_u_stats += (float)(runtime_c_stats.level-1)*u_stats_growth;
		update_all_stats ();
		update_attack_speed ();
		update_movement_speed ();
	}

	public virtual bool check_champion_special_move_condition (){return true;}

	public override void update_timers(){
		autoAtkTimer.update_timer();
		Q_timer.update_timer();
		W_timer.update_timer();
		E_timer.update_timer();
		R_timer.update_timer();
		Passive_timer.update_timer();
		death_timer.update_timer ();
		if(flags.in_combat){
			inCombat_timer.update_timer ();
			if (inCombat_timer.is_over ())
				change_incombat_state (false);
		}

		if (!flags.is_dead) {
			regen_timer.update_timer ();
			if (regen_timer.is_over ()) {
				regen_timer.reset_timer ();
				change_health (runtime_u_stats.hp_reg);
				//TODO mana regen here
			}
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

	public virtual void refresh_player_panel(){
		UI_player_panel.instance.refresh_champ_cd ();
	}

	//low-level stuff
	/*
	public GameObject cur_collision_obj;

	void OnTriggerEnter(Collider col){
		if(col.transform.parent != transform.parent)
			cur_collision_obj = col.gameObject;
	}

	void OnTriggerExit(Collider col){
		if(col.transform.parent != transform.parent)
			cur_collision_obj = null;
	}
	*/

	public override void update_unique(){
		if (!netID.isLocalPlayer)
			return;
		//this one seems to work fine for a champion, but we'll see
		if (!flags.is_dead) {
			//ai takeover
			if (reached (navagent.destination)) {
				ai_takeover = true;
				rotation_override = false;
			} else {
				ai_takeover = false;
				rotation_override = true;
			}

			//if (reached (navagent.destination))
			//	rotation_override = false;

			Vector3 lookatdir = navagent.steeringTarget - transform.position;
			Vector3 cur_forward = transform.forward;
			lookatdir.y = 0f;
			cur_forward.y = 0f;

			transform.position = navagent.transform.position;//Vector3.Lerp(transform.position, navagent.transform.position, Time.deltaTime);
			if (navagent.desiredVelocity.magnitude >= 0.1f * runtime_u_stats.move_speed * GameSceneConsts.dist_multiplier)
				navagent.velocity = navagent.desiredVelocity.normalized * runtime_u_stats.move_speed * GameSceneConsts.dist_multiplier;
			else
				navagent.velocity = Vector3.zero;
			//print (navagent.velocity.magnitude);

			if (!stunned ()) {
				//if (Mathf.Abs (Vector3.Angle (lookatdir, cur_forward)) >= 10f) {//10 is a magic number TODO rewrite this
				if (cur_target_obj != null) {
					if (within_range (cur_target_obj)) {
						//no override means it should autoattack, so stop moving
						stop_moving ();
						auto_attack (cur_target_obj);
					}

					if(ai_takeover && (cur_target_obj != null)){
						transform.LookAt (new Vector3 (cur_target_obj.transform.position.x, transform.position.y, cur_target_obj.transform.position.z));
					} else {
						transform.LookAt (new Vector3 (navagent.steeringTarget.x, transform.position.y, navagent.steeringTarget.z));
						resume_moving ();
					}
				} else {
					transform.LookAt (new Vector3 (navagent.steeringTarget.x, transform.position.y, navagent.steeringTarget.z));
					resume_moving ();
				}
			}
		} else {//special update during death
			if (death_timer.is_over ()) {//time to respawn
				death_timer.reset_timer();
				respawn();
			}
		}
	}

	public bool reached(Vector3 tgt){
		if (Utility.get_dist_position (transform.position, tgt) <= GameSceneConsts.champion_wp_error)
			return true;

		return false;
	}
}
