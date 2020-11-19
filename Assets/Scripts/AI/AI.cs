using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

//the sutomatic movement for game objects
public class AI : MonoBehaviour {

	public enum PotentialTarget{
		MINION,
		STRUCT,//towers, etc
		CHAMPION,
		ENEMY_TEAM,
		HOSTILE,//so that sleeping monsters are excluded
		ANY,
		NONE,
	}

	public enum Decision{
		IDEL,//doing nothing
		HOLD,//don't move but attack anyone within range
		OBJECTIVE,//continue the objective of the game
		CHASE,//chase down some enemy
		ESCAPE,
	}

	protected const float thinkPeriod = 0.5f;
	public Unit unit;//whose brain this is
	protected Utility.Timer ThinkTimer;//how regular to think
	public Vector3 holdedPos;

	//main result
	protected PotentialTarget target;
	protected Decision decision;

	//low level
	protected CapsuleCollider vision;//anything within the collider affects the decision
	protected Collider self;
	protected List<GameObject> objWithin_Ally_champ;
	protected List<GameObject> objWithin_Ally_minion;
	protected List<GameObject> objWithin_Ally_struct;
	protected List<GameObject> objWithin_Ally_item;//wards, etc
	protected List<GameObject> objWithin_Hostile_champ;
	protected List<GameObject> objWithin_Hostile_minion;
	protected List<GameObject> objWithin_Hostile_struct;
	protected List<GameObject> objWithin_Hostile_item;
	protected List<GameObject> objWithin_neturals;
	protected List<GameObject> listedby;//who targeted me?
	protected List<GameObject> temp_untargetable;//someone still in range but still untargetable

	// Use this for initialization
	void Start () {
	
	}

	public void init (Unit _host, Collider _self, PotentialTarget _tgt = PotentialTarget.ANY, Decision _dec = Decision.IDEL){
		ThinkTimer = new Utility.Timer (thinkPeriod);
		ThinkTimer.start_timer ();
		self = _self;
		unit = _host;
		_host.AI_unit = this;
		target = _tgt;
		decision = _dec;
		vision = GetComponent<CapsuleCollider> ();

		objWithin_Ally_champ = new List<GameObject>();
		objWithin_Ally_minion = new List<GameObject>();
		objWithin_Ally_struct = new List<GameObject>();
		objWithin_Ally_item = new List<GameObject>();
		objWithin_Hostile_champ = new List<GameObject>();
		objWithin_Hostile_minion = new List<GameObject>();
		objWithin_Hostile_struct = new List<GameObject>();
		objWithin_Hostile_item = new List<GameObject>();
		objWithin_neturals = new List<GameObject> ();
		listedby = new List<GameObject> ();
		temp_untargetable = new List<GameObject> ();

		init_special ();
	}

	//places for child class start
	public virtual void init_special(){}
	
	// Update is called once per frame
	public virtual void Update () {
		//..and not a server-controlled object
		if (!unit.netID.isServer)
			return;

		if (!unit.flags.is_dead) {
			ThinkTimer.update_timer ();
			if (ThinkTimer.is_over ()) {
				clean_up_all_lists ();
				think ();
				ThinkTimer.restart ();
			}

			//if we're aiming at a target, move until within range, and attack
			if (unit.cur_target_obj != null) {
				if (Utility.LayerTargetable(unit.cur_target_obj)) {
					if (unit.within_range (unit.cur_target_obj)) {
						unit.stop_moving ();
						unit.auto_attack (unit.cur_target_obj);
					} else {//we have to move close to the target
						unit.resume_moving ();
						unit.attackto (unit.cur_target_obj);
					}
				} else {
					unit.cur_target_obj = null;
				}
			} else {//continue with the objective
				unit.resume_moving ();
				unit.return_to_objective ();
			}
		}
	}

	protected void clean_up_all_lists(){//get rid of null elements in all lists
		clean_up_list (objWithin_Ally_champ);
		clean_up_list (objWithin_Ally_minion);
		clean_up_list (objWithin_Ally_struct);
		clean_up_list (objWithin_Ally_item);
		clean_up_list (objWithin_Hostile_champ);
		clean_up_list (objWithin_Hostile_minion);
		clean_up_list (objWithin_Hostile_struct);
		clean_up_list (objWithin_Hostile_item);
		clean_up_list (objWithin_neturals);
		clean_up_list (listedby);
		clean_up_null (temp_untargetable);
	}

	protected void clean_up_lists_informed(){//get rid of null elements in all lists
		move_list_elements_to_temp (objWithin_Ally_champ);
		move_list_elements_to_temp (objWithin_Ally_minion);
		move_list_elements_to_temp (objWithin_Ally_struct);
		move_list_elements_to_temp (objWithin_Ally_item);
		move_list_elements_to_temp (objWithin_Hostile_champ);
		move_list_elements_to_temp (objWithin_Hostile_minion);
		move_list_elements_to_temp (objWithin_Hostile_struct);
		move_list_elements_to_temp (objWithin_Hostile_item);
		move_list_elements_to_temp (objWithin_neturals);
		move_list_elements_to_temp (listedby);
	}

	public void inform_target_source_death(){//src is Unit
		foreach (GameObject source in listedby) {
			if(source != null){
				AI sourceAI = source.GetComponent<AI>();//not at best performance
				sourceAI.clean_up_lists_informed ();
				//sourceAI.think ();
			}
		}
		listedby.Clear ();
	}

	protected void clean_up_list(List<GameObject> toclean){//get rid of null elements in a single lists
		List<GameObject> trashbin = new List<GameObject>();
		for (int i = 0; i < toclean.Count; ++i) {
			if (toclean [i] == null)
				trashbin.Add (toclean [i]);
			else {
				if (!Utility.LayerTargetable(toclean [i]))
					trashbin.Add (toclean[i]);
			}
		}
		foreach (GameObject obj in trashbin)
			toclean.Remove (obj);
		
		trashbin.Clear ();
	}

	protected void clean_up_null(List<GameObject> toclean){//get rid of null elements in a single lists
		List<GameObject> trashbin = new List<GameObject>();
		for (int i = 0; i < toclean.Count; ++i) {
			if (toclean [i] == null)
				trashbin.Add (toclean [i]);
		}
		foreach (GameObject obj in trashbin)
			toclean.Remove (obj);

		trashbin.Clear ();
	}

	protected void move_list_elements_to_temp(List<GameObject> toclean){//move stuff to temp_untargetable
		List<GameObject> trashbin = new List<GameObject>();
		for (int i = 0; i < toclean.Count; ++i) {
			if (toclean [i] == null)
				trashbin.Add (toclean [i]);
			else {
				if (!Utility.LayerTargetable(toclean [i]))
					trashbin.Add (toclean[i]);
			}
		}
		foreach (GameObject obj in trashbin) {
			toclean.Remove (obj);
			if (!temp_untargetable.Contains (obj))
				temp_untargetable.Add (obj);
		}

		trashbin.Clear ();
	}

	//usually temp_untargetable should be the argument
	protected void move_back_list_element(List<GameObject> toclean){//move stuff back from temp_untargetable
		List<GameObject> trashbin = new List<GameObject>();
		for (int i = 0; i < toclean.Count; ++i) {
			if (toclean [i] == null)
				trashbin.Add (toclean [i]);
			else {
				if (Utility.LayerTargetable (toclean [i])) {
					OnTriggerEnter_obj (toclean[i]);
					trashbin.Add (toclean [i]);
				}
			}
		}
		foreach (GameObject obj in trashbin) {
			toclean.Remove (obj);
		}

		trashbin.Clear ();
	}

	//"Think"
	public virtual void think(){
		move_back_list_element (temp_untargetable);
		unit.AtkTarget.GetComponent<AttackTarget> ().move_back_list_element ();
	}

	public void set_decision(Decision _dec){
		decision = _dec;
	}

	public void set_target(PotentialTarget _tgt){
		target = _tgt;
	}

	public GameObject pick_an_enemy(){
		//treat 3 lists as one
		//order is as written in the line below
		int idx = Utility.get_random_int(0, objWithin_Hostile_champ.Count + objWithin_Hostile_minion.Count + objWithin_Hostile_struct.Count);

		if (idx < objWithin_Hostile_champ.Count)
			return objWithin_Hostile_champ [idx];
		else if ((idx >= objWithin_Hostile_champ.Count) && (idx < objWithin_Hostile_champ.Count + objWithin_Hostile_minion.Count))
			return objWithin_Hostile_minion [idx - objWithin_Hostile_champ.Count];
		else if ((idx >= objWithin_Hostile_minion.Count) && (idx < objWithin_Hostile_champ.Count + objWithin_Hostile_minion.Count + objWithin_Hostile_struct.Count))
			return objWithin_Hostile_struct [idx - objWithin_Hostile_champ.Count - objWithin_Hostile_minion.Count];

		return null;
	}

	public GameObject pick_a_minion(){
		//treat 3 lists as one
		//order is as written in the line below
		int idx = Utility.get_random_int(0, objWithin_Hostile_minion.Count);
		return objWithin_Hostile_minion [idx];
	}

	public GameObject pick_a_champion(){
		//treat 3 lists as one
		//order is as written in the line below
		int idx = Utility.get_random_int(0, objWithin_Hostile_champ.Count);
		return objWithin_Hostile_champ [idx];
	}

	//maintain vision list
	void OnTriggerEnter(Collider col){
		//dont do anything if it's not local, or if it's a human
		if ( (!unit.netID.isLocalPlayer)&&(unit.isPlayer) )
			return;
		//..and not a server-controlled object
		if (!unit.netID.isServer)
			return;

		if (col != self){
			if (col.gameObject.GetComponent<Unit> () != null) {
				if (col.gameObject.GetComponent<Unit> ().AI_unit != null) {
					Unit tgt = col.gameObject.GetComponent<Unit> ();
					if ( Utility.LayerTargetable(tgt.gameObject)&&(!tgt.AI_unit.listedby.Contains (gameObject)))//inform the other side we targeted them
						tgt.AI_unit.listedby.Add (gameObject);
				}
			}

			if (!Utility.LayerTargetable(col.gameObject)){
				if(!temp_untargetable.Contains(col.gameObject))
					temp_untargetable.Add(col.gameObject);
			}

			if (col.gameObject.GetComponent<Unit> ().fac == unit.fac) {
				if (col.gameObject.tag == GameSceneConsts.CHAMPION_TAG)
					objWithin_Ally_champ.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.MINION_TAG)
					objWithin_Ally_minion.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.NETURAL_TAG)
					objWithin_neturals.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.TOWER_TAG)
					objWithin_Ally_struct.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.OBJECTIVE_TAG)
					objWithin_Ally_struct.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.ITEM_TAG)
					objWithin_Ally_item.Add (col.gameObject);
			} else {//not in same faction
				if (col.gameObject.tag == GameSceneConsts.CHAMPION_TAG)
					objWithin_Hostile_champ.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.MINION_TAG)
					objWithin_Hostile_minion.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.NETURAL_TAG)
					objWithin_neturals.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.TOWER_TAG)
					objWithin_Hostile_struct.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.OBJECTIVE_TAG)
					objWithin_Hostile_struct.Add (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.ITEM_TAG)
					objWithin_Hostile_item.Add (col.gameObject);
			}
		}
	}

	void OnTriggerExit(Collider col){
		//dont do anything if it's not local, or if it's a human
		if ( (!unit.netID.isLocalPlayer)&&(unit.isPlayer) )
			return;
		//..and not a server-controlled object
		if (!unit.netID.isServer)
			return;
		
		if (col != self){
			if (col.gameObject.GetComponent<Unit> () != null) {
				if (col.gameObject.GetComponent<Unit> ().AI_unit != null) {
					Unit tgt = col.gameObject.GetComponent<Unit> ();
					if (tgt.AI_unit.listedby.Contains (gameObject))//inform the other side we targeted them
						tgt.AI_unit.listedby.Remove (gameObject);
				}
			}

			if (temp_untargetable.Contains (col.gameObject))
				temp_untargetable.Remove (col.gameObject);

			if (col.gameObject.GetComponent<Unit> ().fac == unit.fac) {
				if (col.gameObject.tag == GameSceneConsts.CHAMPION_TAG)
					objWithin_Ally_champ.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.MINION_TAG)
					objWithin_Ally_minion.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.NETURAL_TAG)
					objWithin_neturals.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.TOWER_TAG)
					objWithin_Ally_struct.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.OBJECTIVE_TAG)
					objWithin_Ally_struct.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.ITEM_TAG)
					objWithin_Ally_item.Remove (col.gameObject);
			} else {//not in same faction
				if (col.gameObject.tag == GameSceneConsts.CHAMPION_TAG)
					objWithin_Hostile_champ.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.MINION_TAG)
					objWithin_Hostile_minion.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.NETURAL_TAG)
					objWithin_neturals.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.TOWER_TAG)
					objWithin_Hostile_struct.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.OBJECTIVE_TAG)
					objWithin_Hostile_struct.Remove (col.gameObject);
				else if (col.gameObject.tag == GameSceneConsts.ITEM_TAG)
					objWithin_Hostile_item.Remove (col.gameObject);
			}
		}
	}

	void OnTriggerEnter_obj(GameObject obj){
		if ( (!unit.netID.isLocalPlayer)&&(unit.isPlayer) )
			return;

		if (obj != gameObject){
			if (obj.GetComponent<Unit> ().fac == unit.fac) {
				if (obj.tag == GameSceneConsts.CHAMPION_TAG)
					objWithin_Ally_champ.Add (obj);
				else if (obj.tag == GameSceneConsts.MINION_TAG)
					objWithin_Ally_minion.Add (obj);
				else if (obj.tag == GameSceneConsts.NETURAL_TAG)
					objWithin_neturals.Add (obj);
				else if (obj.tag == GameSceneConsts.TOWER_TAG)
					objWithin_Ally_struct.Add (obj);
				else if (obj.tag == GameSceneConsts.OBJECTIVE_TAG)
					objWithin_Ally_struct.Add (obj);
				else if (obj.tag == GameSceneConsts.ITEM_TAG)
					objWithin_Ally_item.Add (obj);
			} else {//not in same faction
				if (obj.tag == GameSceneConsts.CHAMPION_TAG)
					objWithin_Hostile_champ.Add (obj);
				else if (obj.tag == GameSceneConsts.MINION_TAG)
					objWithin_Hostile_minion.Add (obj);
				else if (obj.tag == GameSceneConsts.NETURAL_TAG)
					objWithin_neturals.Add (obj);
				else if (obj.tag == GameSceneConsts.TOWER_TAG)
					objWithin_Hostile_struct.Add (obj);
				else if (obj.tag == GameSceneConsts.OBJECTIVE_TAG)
					objWithin_Hostile_struct.Add (obj);
				else if (obj.tag == GameSceneConsts.ITEM_TAG)
					objWithin_Hostile_item.Add (obj);
			}
		}
	}
}
