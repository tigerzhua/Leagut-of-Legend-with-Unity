using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

//skills
public class skill{
	public enum Type{
		BASE,
		PASSIVE,//for completely passive skills, like
		TOGGLE,//toggle abilities, like Teemo's E
		NORMAL
	}

	public float cost;
	public float cd;
	public float dmg;
	public float ad_ratio;
	public float ap_ratio;//if it's about bonus ap or ad, treat them as special cases in functions
	public int level;
	public Type type;
	public Utility.Timer timer;
	public bool need_indicate_target;//true for skill shot, targted skills, and targeted aoes

	public skill(){
		type = Type.NORMAL;
		cost = 0;
		cd = 0;
		dmg = 0;
		ad_ratio = 0;
		ap_ratio = 0;
		level = 0;
		timer = new Utility.Timer (0f);
		need_indicate_target = false;
	}
}

public struct Status{//buff, debuff, etc.
	public bool isnot_default;
	public bool depend_on_pos;//depends on where units stand, but not time
	public float dur;//duration
	public Utility.Timer timer;
	public string flags;
	public string name;
	public GameObject from;
	public GameObject to;

	//to know which string means which flag, see Unit

	public void init (){
		depend_on_pos = false;
		isnot_default = true;
		name = "default status name";
		flags = "";
	}
	public void init (float time){
		depend_on_pos = false;
		isnot_default = true;
		name = "default status name";
		flags = "";
		dur = time;
		timer = new Utility.Timer (dur);
		timer.start_timer ();
	}

	public void update_status(){
		timer.update_timer ();
	}

	public bool is_over(){
		return timer.is_over ();
	}
}

public class Unit : MonoBehaviour {
	//consts
	public const float dist_step = 0.01f;

	//defines
	public struct unit_flags{
		public bool ranged;
		public bool is_stun;
		public bool is_rooted;
		public bool is_silent;
		public bool red_buff;
		public bool blue_buff;
		public bool in_combat;
		public bool no_mana;//no mana unit
		public bool is_dead;

		public void init(){
			no_mana = true;
			is_dead = false;
			in_combat = false;
			is_stun = false;
			is_rooted = false;
			is_silent = false;
			red_buff = false;
			blue_buff = false;
		}

		public void set_flag_from_char(char chr, bool flg){
			switch (chr) {
			case 's':
				is_silent = flg;
				break;
			case 'r':
				is_rooted = flg;
				break;
			case 't':
				is_stun = flg;
				break;
			case 'e':
				red_buff = flg;
				break;
			case 'u':
				blue_buff = flg;
				break;
			default:
				break;
			}
		}

		public void set_flag_from_string(string str, bool flg){
			for (int i = 0; i < str.Length; ++i) {
				set_flag_from_char (str[i], flg);
			}
		}
	}

	public struct unit_stats{
		public float hp;
		public float max_hp;
		public float hp_reg;
		public float mana;//cast resources, not necessarily mana
		public float max_mana;
		public float mana_reg;
		public float move_speed;
		public float attack_speed;
		public float damage;
		public float magic_dmage;//on-hit magic damage
		public float armor_pen_percent;
		public float armor_pen;
		public float magic_pen_percent;
		public float magic_pen;
		public float range;
		public float armor;
		public float magic_resist;
		public float crd;//0.0 to 0.40(default max cdr)
		public float life_steal;
		public int crit_chance;
		public float projectile_spd;

		//bonus stats
		public float dmg_reduction;

		//what you get after killing it
		public float bounty;
		public float exp;

		public void init(){
			//TODO test only
			bounty = 50;
			exp = 30;

			hp = 0;
			max_hp = 0;
			hp_reg = 0;
			mana = 0;//cast resources, not necessarily mana
			max_mana = 0;
			mana_reg = 0;
			move_speed = 0;
			attack_speed = 0;
			damage = 0;
			magic_dmage = 0;//on-hit magic damage
			armor_pen = 0;
			armor_pen_percent = 0;
			magic_pen = 0;
			magic_pen_percent = 0;
			range = 0;
			armor = 0;
			magic_resist = 0;
			crd = 0;//0.0 to 0.45(default max cdr)
			life_steal = 0;
			dmg_reduction = 0;
			crit_chance = 0;
			projectile_spd = 400f;
		}

		public void init_to_1(){
			bounty = 1;
			exp = 1;

			hp = 1;
			max_hp = 1;
			hp_reg = 1;
			mana = 1;//cast resources, not necessarily mana
			max_mana = 1;
			mana_reg = 1;
			move_speed = 1;
			attack_speed = 1;
			damage = 1;
			magic_dmage = 1;//on-hit magic damage
			armor_pen = 1;
			armor_pen_percent = 1;
			magic_pen = 1;
			magic_pen_percent = 1;
			range = 1;
			armor = 1;
			magic_resist = 1;
			crd = 1;//0.0 to 0.45(default max cdr)
			life_steal = 1;
			dmg_reduction = 1;
			crit_chance = 1;
			projectile_spd = 1;
		}

		public static unit_stats operator + (unit_stats l, unit_stats r){
			l.bounty += r.bounty;
			l.exp += r.exp;
			l.max_hp += r.max_hp;

			l.hp += r.hp;
			l.hp_reg += r.hp_reg;
			l.mana += r.mana;//cast resources, not necessarily mana
			l.max_mana += r.max_mana;
			l.mana_reg += r.mana_reg;
			l.move_speed += r.move_speed;
			l.attack_speed += r.attack_speed;
			l.damage += r.damage;
			l.magic_dmage += r.magic_dmage;
			l.armor_pen_percent += r.armor_pen_percent;
			l.armor_pen += r.armor_pen;
			l.magic_pen_percent += r.magic_pen_percent;
			l.magic_pen += r.magic_pen;
			l.range += r.range;
			l.armor += r.armor;
			l.magic_resist += r.magic_resist;
			l.crd += r.crd;//0.0 to 0.45(default max cdr)
			l.life_steal += r.life_steal;
			l.dmg_reduction += r.dmg_reduction;
			l.crit_chance += r.crit_chance;
			l.projectile_spd += r.projectile_spd;
			return l;
		}

		public static unit_stats operator * (unit_stats l, unit_stats r){
			l.bounty *= r.bounty;
			l.exp *= r.exp;
			l.max_hp *= r.max_hp;

			l.hp *= r.hp;
			l.hp_reg *= r.hp_reg;
			l.mana *= r.mana;//cast resources, not necessarily mana
			l.max_mana *= r.max_mana;
			l.mana_reg *= r.mana_reg;
			l.move_speed *= r.move_speed;
			l.attack_speed *= r.attack_speed;
			l.damage *= r.damage;
			l.magic_dmage *= r.magic_dmage;
			l.armor_pen_percent *= r.armor_pen_percent;
			l.armor_pen *= r.armor_pen;
			l.magic_pen_percent *= r.magic_pen_percent;
			l.magic_pen *= r.magic_pen;
			l.range *= r.range;
			l.armor *= r.armor;
			l.magic_resist *= r.magic_resist;
			l.crd *= r.crd;//0.0 to 0.45(default max cdr)
			l.life_steal *= r.life_steal;
			l.dmg_reduction *= r.dmg_reduction;
			l.crit_chance *= r.crit_chance;
			l.projectile_spd *= r.projectile_spd;
			return l;
		}

		public static unit_stats operator * (float l, unit_stats r){
			r.bounty *= l;
			r.exp *= l;

			r.hp *= l;
			r.max_hp *= l;
			r.hp_reg *= l;
			r.mana *= l;//cast resources, not necessarily mana
			r.max_mana *= l;
			r.mana_reg *= l;
			r.move_speed *= l;
			r.attack_speed *= l;
			r.damage *= l;
			r.magic_dmage *= l;
			r.armor_pen_percent *= l;
			r.armor_pen *= l;
			r.magic_pen_percent *= l;
			r.magic_pen *= l;
			r.range *= l;
			r.armor *= l;
			r.magic_resist *= l;
			r.crd *= l;//0.0 to 0.45(default max cdr)
			r.life_steal *= l;
			r.dmg_reduction *= l;
			r.crit_chance = (int)((float)r.crit_chance * l);
			r.projectile_spd *= l;
			return r;
		}
	}
		
	public unit_flags flags;
	//base is what vanilla unit has, 
	//growth is what to add when level up, 
	//bonus comes from items, buffs, etc.
	public unit_stats base_u_stats, base_u_stats_mult, u_stats_growth;
	public unit_stats bonus_u_stats, bonus_u_stats_mult;
	//runtime is the one that is directly used, it's the sum of previous three, plus some unit-specific stats
	public unit_stats runtime_u_stats;

	Types.damage_combo Onhit_dmg, recent_dmg_receive;
	public Utility.Timer autoAtkTimer;
	public Types.Faction fac;
	public List<Status> status;
	GameObject autoAtk_prefab;//usually for ranged champion shooting projectiles
	public Player host;//the player object, only available for player-controlled characters
	public NetworkIdentity netID;
    public NetworkUnit netUnit;//networkUnit reference
	public bool isPlayer = false; //if this unit is a player(basically, champions)
	protected Utility.Timer inCombat_timer, regen_timer;
	public Item[] items;

	//unity stuff
	public Rigidbody rb;
	public Collider self_col;
	public GameObject AtkTarget; //the Game obj for attack radius
	public UnityEngine.AI.NavMeshAgent navagent;
	public AI AI_unit;//the brain
	public UI ui;

	//in-game actions
	public virtual GameObject auto_attack(GameObject target){
		if (  (target == null)||(!autoAtkTimer.is_over())||(!within_range (target))||!can_attack()||(!Utility.LayerTargetable(target)) )
			return null;

		//transform.LookAt (new Vector3(target.transform.position.x, transform.position.y, transform.position.z));
		GameObject projectile = new GameObject();
		if (autoAtkTimer.is_over ()) {
			change_incombat_state (true);
			Unit tgt = target.GetComponent<Unit> ();
			if (flags.ranged) {
				projectile = Instantiate (autoAtk_prefab, transform.position, transform.rotation) as GameObject;
				Projectile proj_script = Utility.get_projectile_script (projectile);
				proj_script.init(this, prepare_dmg(), runtime_u_stats.projectile_spd, true, target);
			} else {//melee
				//setup damage and on-hit stuff
				if (tgt.receive_damage (this, true, prepare_dmg())) {//TODO should have a null check here
					//OnKillingTarget (tgt); called by the target
					cur_target_obj = null;
				}
			}
			autoAtkTimer.restart ();
		}
		return projectile;
	}

	public void change_incombat_state(bool _state){
		flags.in_combat = _state;
		if (_state)
			inCombat_timer.restart ();
		else
			inCombat_timer.finish ();
	}

	public virtual bool moveto(GameObject tgt){
		navagent.SetDestination (tgt.transform.position);
		return true;
	}

	public virtual bool moveto(Vector3 tgt){
		navagent.SetDestination (tgt);
		return true;
	}

	//move to a target until within range
	public virtual bool attackto(GameObject tgt){
		if (tgt == null)
			return false;

		moveto (tgt);
		return true;
	}

	public virtual void return_to_objective(){}

	public void stop_moving(){
		navagent.Stop ();
	}

	public void resume_moving(){
		navagent.Resume ();
	}

	public virtual void OnKillingTarget(Unit target){
	}

	//Updates
	public virtual void update_timers(){
		autoAtkTimer.update_timer();
		if(flags.in_combat){
			inCombat_timer.update_timer ();
			if (inCombat_timer.is_over ())
				change_incombat_state (false);
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
	}

	//DAMAGE!
	//this funtion can be called only once during for one skill/autoattack
	public virtual bool receive_damage(Unit source, bool applyOnhit, float pd = 0f, float armor_pen = 0f, float armor_pen_percent = 0f, 
		float md = 0f, float magic_pen = 0f, float magic_pen_percent = 0f, 
		float td = 0f, float pure_damage = 0f){
		recent_dmg_receive.init ();//clear it first
		bool result = false;
		float calc_pd = Utility.get_damage (pd, runtime_u_stats.armor * (1f - armor_pen_percent) - armor_pen, runtime_u_stats.dmg_reduction);
		float calc_md = Utility.get_magic_damage (md, runtime_u_stats.magic_resist * (1f - magic_pen_percent) - magic_pen, runtime_u_stats.dmg_reduction);
		float temp;
		temp = change_health (-calc_pd);
		if (temp < 0) {
			recent_dmg_receive.physical_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.physical_dmg += temp;
		
		temp = change_health (-calc_md);
		if (temp < 0) {
			recent_dmg_receive.magic_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.magic_dmg += temp;

		temp = change_health (-td*(1f-runtime_u_stats.dmg_reduction));
		if (temp < 0) {
			recent_dmg_receive.true_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.true_dmg += temp;

		temp = change_health (-pure_damage);
		if (temp < 0) {
			recent_dmg_receive.pure_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.pure_dmg += temp;
		
		Utility.generate_dmg_txt (gameObject, (int)calc_pd, Color.red);
		Utility.generate_dmg_txt (gameObject, (int)calc_md, Color.cyan);

		//aftermath
		change_incombat_state(true);
		if(applyOnhit && Utility.LifeStealable(gameObject))
			source.Onhit_post (recent_dmg_receive);
		if ((!flags.is_dead) && result) {
			source.OnKillingTarget (this);
			process_death ();
		}

		return result;
	}

	//this funtion can be called only once during for one skill/autoattack
	public virtual bool receive_damage(Unit source, bool applyOnhit, Types.damage_combo dmg){//return true if killed someone
		recent_dmg_receive.init ();
		bool result = false;
		float calc_pd = Utility.get_damage (dmg.physical_dmg, runtime_u_stats.armor * (1f - dmg.armor_pen_percent) - dmg.armor_pen, runtime_u_stats.dmg_reduction);
		float calc_md = Utility.get_magic_damage (dmg.magic_dmg, runtime_u_stats.magic_resist*(1f-dmg.magic_pen_percent)-dmg.magic_pen, runtime_u_stats.dmg_reduction);
		float temp;
		temp = change_health (-calc_pd);
		if (temp < 0) {
			recent_dmg_receive.physical_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.physical_dmg += temp;

		temp = change_health (-calc_md);
		if (temp < 0) {
			recent_dmg_receive.magic_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.magic_dmg += temp;

		temp = change_health (-dmg.true_dmg*(1f-runtime_u_stats.dmg_reduction));
		if (temp < 0) {
			recent_dmg_receive.true_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.true_dmg += temp;

		temp = change_health (-dmg.pure_dmg);
		if (temp < 0) {
			recent_dmg_receive.pure_dmg += (-temp);
			result = true;
		} else
			recent_dmg_receive.pure_dmg += temp;
		Utility.generate_dmg_txt (gameObject, (int)calc_pd, Color.red);
		Utility.generate_dmg_txt (gameObject, (int)calc_md, Color.cyan);

		//aftermath
		change_incombat_state(true);
		if(applyOnhit && Utility.LifeStealable(gameObject))
			source.Onhit_post (recent_dmg_receive);
		if ((!flags.is_dead) && result) {
			source.OnKillingTarget (this);
			process_death ();
		}

		return result;
	}

	public virtual Types.damage_combo prepare_dmg(){
		Types.damage_combo dmg = new Types.damage_combo ();
		dmg.init ();
		dmg.physical_dmg = runtime_u_stats.damage;
		dmg = Onhit_pre (dmg);
		return dmg;
	}

	public virtual Types.damage_combo Onhit_pre(Types.damage_combo combo){//magic damage, etc
		combo = combo + Onhit_pre_item(combo);
		combo = combo + Onhit_dmg;
		if (Utility.is_crit (runtime_u_stats.crit_chance))
			combo.physical_dmg *= 2;
		return combo;
	}

	//all on-hit item effect need to be processed before hitting target
	public virtual Types.damage_combo Onhit_pre_item(Types.damage_combo combo){
		combo.init ();
		return combo;
	}

	public virtual void Onhit_post(Types.damage_combo dmg){//life steal, etc
		Onhit_post_item(dmg);
		//life steal
		change_health(dmg.physical_dmg * runtime_u_stats.life_steal);
	}

	//all on-hit item effect need to be processed after hitting target (e.g. lifesteal)
	public virtual void Onhit_post_item(Types.damage_combo dmg){
	}

	//stats update
	public virtual void Update () {
		update_timers ();
		update_unique ();
	}

	public Types.damage_combo get_damage_dealt(){return recent_dmg_receive;}

	//Items
	public bool add_item(Item.Item_name name){
		if (isPlayer) {
			if (!Utility.unit_at_home ())
				return false;

			if (!host.check_gold (GameSceneConsts.item_prices [(int)name]))
				return false;
		}

		for (int i = 0; i < GameSceneConsts.max_item; ++i) {
			if (items [i] == null) {
				host.change_gold (-GameSceneConsts.item_prices [(int)name]);
				items [i] = gameObject.AddComponent <Item>();
				items [i].init (name);
				base_u_stats += items[i].stats_add_base;
				base_u_stats_mult += items[i].stats_mult_base;
				bonus_u_stats += items[i].stats_add;
				bonus_u_stats += items[i].stats_mult;
				update_all_stats ();
				return true;
			}
		}
		return false;
	}

	//unit-specific physics calculation should also be computed here
	public virtual void update_unique (){}

	//return negative if killed, otherwise positive
	//the abs of return value is how much damage dealt
	public virtual float change_health(float num){
		float actual_dmg = 0f;
		if ((runtime_u_stats.hp + num) >= runtime_u_stats.max_hp) {
			runtime_u_stats.hp = runtime_u_stats.max_hp;
			actual_dmg = runtime_u_stats.max_hp - runtime_u_stats.hp;
		} else if ((runtime_u_stats.hp + num) <= 0) {//killed
			actual_dmg = runtime_u_stats.hp;
			runtime_u_stats.hp = 0f;
			ui.update_bar_fillamount (runtime_u_stats.hp / runtime_u_stats.max_hp);
			if (isPlayer)
				UI_player_panel.instance.change_hp (runtime_u_stats.hp / runtime_u_stats.max_hp);
            netUnit.CmdchangeCurHp(runtime_u_stats.hp);
            return -Mathf.Abs(actual_dmg);
		} else {//normal
			runtime_u_stats.hp += num;
			actual_dmg = num;
		}

		ui.update_bar_fillamount (runtime_u_stats.hp/runtime_u_stats.max_hp);
		if (isPlayer)
			UI_player_panel.instance.change_hp (runtime_u_stats.hp/runtime_u_stats.max_hp);
        netUnit.CmdchangeCurHp(runtime_u_stats.hp);
        return Mathf.Abs(actual_dmg);
	}

	//update stats
	public virtual void update_all_stats(){
		runtime_u_stats = (base_u_stats * base_u_stats_mult + bonus_u_stats) * bonus_u_stats_mult;
		//print ("hululu");
		//print (base_u_stats.max_hp);
		//print (base_u_stats_mult.max_hp);
		//print (bonus_u_stats.max_hp);
		//print (bonus_u_stats_mult.max_hp);
		//print ("galulu");
		update_attack_speed ();
		update_movement_speed ();
	}

	//TODO may need to change in the future
	//it's now different from update_all_stats
	public virtual void update_attack_speed(){
		runtime_u_stats.attack_speed = (base_u_stats.attack_speed + bonus_u_stats.attack_speed) * bonus_u_stats_mult.attack_speed;
		runtime_u_stats.attack_speed = 1.0f / runtime_u_stats.attack_speed;
	}

	public virtual void update_movement_speed(){
		runtime_u_stats.move_speed = (base_u_stats.move_speed + bonus_u_stats.move_speed) * bonus_u_stats_mult.move_speed;
		navagent.speed = runtime_u_stats.move_speed * GameSceneConsts.dist_multiplier;
	}
		
	public virtual void update_armor(){runtime_u_stats.armor = base_u_stats.armor + bonus_u_stats.armor;}
	public virtual void update_magic_resist(){runtime_u_stats.magic_resist = base_u_stats.magic_resist + bonus_u_stats.magic_resist;}
	public virtual void update_dmg_reduction(){runtime_u_stats.dmg_reduction = base_u_stats.dmg_reduction + bonus_u_stats.dmg_reduction;}

	public virtual void process_death(){
		flags.is_dead = true;
		gameObject.layer = GameSceneConsts.LAYER_DEAD;
		AI_unit.inform_target_source_death ();
		AtkTarget.GetComponent<AttackTarget> ().inform_target_source_death ();
		cur_target_obj = null;
	}

	public virtual void respawn(){
		navagent.enabled = true;
		if (fac == Types.Faction.BLUE) {
			navagent.Warp (GameSceneConsts.blue_team_respawn_pt.transform.position);
			navagent.SetDestination(GameSceneConsts.blue_team_respawn_pt.transform.position);
			AI_unit.holdedPos = GameSceneConsts.blue_team_respawn_pt.transform.position;//not sure if this is necessary
			transform.position = navagent.transform.position;
		} else if (fac == Types.Faction.RED) {
			navagent.Warp (GameSceneConsts.red_team_respawn_pt.transform.position);
			navagent.SetDestination(GameSceneConsts.red_team_respawn_pt.transform.position);
			AI_unit.holdedPos = GameSceneConsts.red_team_respawn_pt.transform.position;//not sure if this is necessary
			transform.position = navagent.transform.position;
		}
		gameObject.transform.rotation = Quaternion.identity;
		//refill hp and mana
		change_health (runtime_u_stats.max_hp);
		runtime_u_stats.mana = runtime_u_stats.max_mana;
		navagent.Resume();
		flags.is_dead = false;
		ui.refill ();
		gameObject.layer = GameSceneConsts.LAYER_DEFAULT;
	}

	//Utilities
	public bool within_range(GameObject target){
		if (AtkTarget.GetComponent<AttackTarget> ().targets.Contains (target))
			return true;

		return false;
	}

	//flag related
	public bool receive_status(Status sts){
		if (sts.isnot_default) {
			status.Add (sts);
			flags.set_flag_from_string (sts.flags, true);
			return true;
		}
		return false;
	}

	//check if the champion already has the status of this name
	public bool has_status(string name){
		foreach (Status sts in status) {
			if (sts.name == name)
				return true;
		}
		return false;
	}

	public bool can_move(){
		if ((flags.is_stun) || (flags.is_rooted))
			return false;

		return true;
	}

	public bool can_cast(){
		if (flags.is_silent||flags.is_stun)
			return false;

		return true;
	}

	public bool can_attack(){
		if (flags.is_stun)
			return false;

		return true;
	}

	//member access
	public UnityEngine.AI.NavMeshAgent get_navagent(){return navagent;}
	public bool stunned(){return flags.is_stun;}

	//basics
	public virtual void init(){}
	//call this before actual init();
	public void init_pre(){
		netID = transform.parent.gameObject.GetComponent<NetworkIdentity> ();
		self_col = GetComponent<Collider> ();
		cur_target_obj = null;
		status = new List<Status> ();
		flags = new unit_flags();
		flags.init ();

		if ( (!netID.isLocalPlayer)&&(isPlayer) )
			return;
		
		//stats
		base_u_stats = new unit_stats ();
		base_u_stats.init ();
		base_u_stats_mult = new unit_stats ();
		base_u_stats_mult.init_to_1 ();
		u_stats_growth = new unit_stats ();
		u_stats_growth.init ();
		bonus_u_stats = new unit_stats();
		bonus_u_stats.init ();
		bonus_u_stats_mult = new unit_stats ();
		bonus_u_stats_mult.init_to_1 ();

		inCombat_timer = new Utility.Timer (GameSceneConsts.in_combat_time);
		regen_timer = new Utility.Timer (GameSceneConsts.regen_interval);
		regen_timer.start_timer ();
		items = new Item[GameSceneConsts.max_item + GameSceneConsts.max_supp_item];
		for (int i = 0; i < items.Length; ++i)
			items [i] = null;
		Onhit_dmg = new Types.damage_combo ();
		Onhit_dmg.init ();
		recent_dmg_receive = new Types.damage_combo ();
		recent_dmg_receive.init ();
	}

	public void init_post(){

		runtime_u_stats = base_u_stats;
		update_attack_speed ();
		//update_movement_speed ();//called from outside
		autoAtkTimer = new Utility.Timer(runtime_u_stats.attack_speed);
		autoAtkTimer.start_timer ();
		init_AtkTarget ();
		init_resources ();
        //netUnit = transform.parent.GetComponent<NetworkUnit>();
        //netUnit.init(runtime_u_stats.max_hp, runtime_u_stats.max_mana);
	}
	//must be called after init()
	public virtual void init_set_AI(AI _ai, AI.PotentialTarget _tgt, AI.Decision _dec){
		AI_unit = _ai;
		AI_unit.init (this, self_col, _tgt, _dec);
	}

    public void init_netUnit(){
        netUnit.init(this, runtime_u_stats.max_hp,runtime_u_stats.max_mana);
    }

	//low-level stuff
	public GameObject cur_target_obj;//any clickable thing you selected

	public virtual void init_resources(){
		autoAtk_prefab = Resources.Load ("Prefabs/Common/skills/projectile_prefab") as GameObject;
	}

	public void init_AtkTarget(){
		AtkTarget = Utility.get_AtkTarget_obj (gameObject);
		AtkTarget.GetComponent<AttackTarget> ().init (self_col, fac, runtime_u_stats.range * GameSceneConsts.dist_multiplier);
	}
}
