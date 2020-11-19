using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkUnit {

	//which champion this player is
	Champion.champion_name champ_type;
	public Champion champ;
	public int Owner;//idx of its owner
	public Types.Faction fac;
	public float gold;
	public int idx;
	GameObject navunit;

	//when local player has authority
	public override void OnStartLocalPlayer(){
		print ("OnStartLocalPlayer");
		base.OnStartLocalPlayer ();
		//if (!isLocalPlayer)
		//	return;
		
		GameSceneManager.instance.register_local_player(this);
		navunit = Instantiate(GameSceneConsts.NavUnit_player, transform.position, transform.rotation) as GameObject;
		Cmdinstantiate_navunit(navunit);
			
		if (navunit == null)
			print ("navNULL");
		//then assign the right champion
		switch (champ_type) {
		case Champion.champion_name.Garen:
			champ = Utility.get_champion_object (gameObject).AddComponent <Garen>() as Champion;
			champ.init ();
			break;
		default:
			champ = Utility.get_champion_object (gameObject).AddComponent <Garen>() as Champion;
			champ.init ();
			break;
		}
        unit = champ;
		champ.navagent = navunit.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		champ.navagent.updateRotation = false;
		//champ.navagent.updatePosition = false;
		champ.update_movement_speed ();
		GetComponent<CatchUPUnit> ().host = champ;

		//assemble the champion
		champ.fac = fac;
		champ.host = this;
        if(isLocalPlayer)
		    champ.isPlayer = true;
		AI champ_ai = Utility.get_AI_script (gameObject);
		champ_ai.init (champ, champ.self_col, AI.PotentialTarget.HOSTILE, AI.Decision.HOLD);
		champ.AI_unit = champ_ai; 
		//TODO test only
		UI_Champion champ_UI = Utility.get_UI_obj(gameObject).GetComponent<UI_Champion> ();
		champ_UI.init ();
		champ.ui = champ_UI;
		gold = 100000;

		//init its own controller
		GameSceneManager.instance.ctrlScript.init(gameObject, champ_type);

		//init its own LOCAL UI
		GameSceneManager.instance.LocalUI.init(this);

        champ.host = this;
        champ.netUnit = this as NetworkUnit;
        base.init(champ, champ.runtime_u_stats.max_hp, champ.runtime_u_stats.max_mana);
    }

	//called by lobby manager
	public void takedown_paras(Champion.champion_name _name, Types.Faction _fac, int _idx){
		idx = _idx;
		fac = _fac;
		champ_type = _name;
	}
		
	//called everywhere, so only necessary part will be initialized
	public void Start(){//(Champion.champion_name _name, Types.Faction _fac, int _idx){
		//then assign the right champion
		if (isLocalPlayer)
			return;

		switch (champ_type) {
		case Champion.champion_name.Garen:
			champ = Utility.get_champion_object (gameObject).AddComponent <Garen>() as Champion;
			champ.init ();
			break;
		case Champion.champion_name.Ashe:
			champ = Utility.get_champion_object (gameObject).AddComponent <Garen>() as Champion;
			champ.init ();
			break;
		default:
			champ = Utility.get_champion_object (gameObject).AddComponent <Garen>() as Champion;
			champ.init ();
			break;
		}
        unit = champ;
        //assemble the champion
        champ.fac = fac;
		champ.host = this;
        if(isLocalPlayer)
		    champ.isPlayer = true;
		AI champ_ai = Utility.get_AI_script (gameObject);
		champ_ai.init (champ, champ.self_col, AI.PotentialTarget.HOSTILE, AI.Decision.HOLD);
		champ.AI_unit = champ_ai; 
		//TODO test only
		UI_Champion champ_UI = Utility.get_UI_obj(gameObject).GetComponent<UI_Champion> ();
		champ_UI.init ();
		champ.ui = champ_UI;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void change_gold(float _gold){
		gold += _gold;
		UI_player_panel.instance.change_gold ((int)gold);
	}

	public bool check_gold(int _gold){
		if (_gold <= gold)
			return true;

		return false;
	}

	public bool activate_Q_skill() {
		if (!champ.Q_skill.need_indicate_target)
			champ.act_Q ();
		else {			
			if (champ.cur_target_obj != null)
				champ.act_Q ();
			else
				return false;
		}
		return true;
	}

	public bool activate_W_skill() {
		if (!champ.W_skill.need_indicate_target)
			champ.act_W ();
		else {			
			if (champ.cur_target_obj != null)
				champ.act_W ();
			else
				return false;
		}
		return true;
	}

	public bool activate_E_skill() {
		if (!champ.E_skill.need_indicate_target)
			champ.act_E ();
		else {			
			if (champ.cur_target_obj != null)
				champ.act_E ();
			else
				return false;
		}
		return true;
	}

	public bool activate_R_skill() {//true if no need to select target, false if need a target
		if (!champ.R_skill.need_indicate_target) {
			champ.act_R ();
		}
		else{
			if (champ.cur_target_obj != null)
				champ.act_R ();
			else
				return false;
		}

		return true;
	}

	public void activate_Passive_skill(){
		champ.act_Passive ();
	}

	public void activate_summoner_spell_D(){
	}

	public void activate_summoner_spell_F(){
	}

	public void activate_recall(){
	}
}
