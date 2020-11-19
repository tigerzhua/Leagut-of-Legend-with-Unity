using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SetupMap : NetworkBehaviour {
	public enum Game_type{
		SUMMONERS_NORMAL,
	}

	// Use this for initialization
	public override void OnStartServer(){
		print ("OnStartServer");
		if (!isServer)
			return;

		print ("IsStartServer");
		setup_map ();
		//map specific contents:
	}
	/*
	void Start(){
		if (!isServer)
			return;

		setup_map ();
	}
	*/

	void setup_map(){
		//first, towers
		Transform towerMaster = transform.FindChild("Towers");
		for (int i = 0; i < towerMaster.childCount; ++i) {
			//the "root" object for a tower
			GameObject towerPos = towerMaster.GetChild(i).gameObject;
			Tower_info towerInfo = towerPos.GetComponent<Tower_info> ();
			GameObject new_tower = Instantiate (GameSceneConsts.tower_prefab, towerPos.transform.position, towerPos.transform.rotation) as GameObject;
			towerInfo.tower_unit = Utility.get_unit_object (new_tower);
			Tower towerScript = Utility.get_unit_script (new_tower) as Tower;
			towerScript.Towerinit (towerInfo.fac, towerInfo.type);
			Utility.get_AI_script (new_tower).init (towerScript, towerScript.self_col, AI.PotentialTarget.ENEMY_TEAM, AI.Decision.HOLD);
			Utility.get_UI_obj (new_tower).GetComponent<UI_structure> ().init ();
            towerScript.init_netUnit();

			NetworkServer.Spawn (new_tower);
		}

		Transform structureMaster = transform.FindChild ("Structures");
		for (int i = 0; i < structureMaster.childCount; ++i) {
			//the "root" object for a tower
			GameObject structPos = structureMaster.GetChild(i).gameObject;
			struct_info structInfo = structPos.GetComponent<struct_info> ();
			GameObject new_struct = Instantiate (GameSceneConsts.inhib_prefab, structPos.transform.position, structPos.transform.rotation) as GameObject;
			Structure struct_script = Utility.get_unit_script (new_struct) as Structure;
			struct_script.Structinit (structInfo.fac, structInfo.type);
			Utility.get_AI_script (new_struct).init (struct_script, struct_script.self_col, AI.PotentialTarget.NONE, AI.Decision.IDEL);
			Utility.get_UI_obj (new_struct).GetComponent<UI_structure> ().init ();
			foreach (GameObject tower in structInfo.guardians) {
				struct_script.guardians.Add (tower);
			}
            struct_script.init_netUnit();
			NetworkServer.Spawn (new_struct);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
