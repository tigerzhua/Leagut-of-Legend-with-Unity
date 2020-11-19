using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MinionSpawnController : NetworkBehaviour {

	public enum WaveType{
		NORMAL,
		ENHANCED,//with cannon
		SUPER,
	}

	public Types.Faction fac;
	public Types.Lanes lane;
	//wave timer controls interval between waves, micro is used within one wave to separate minions
	Utility.Timer timer_wave, timer_micro;
	bool wave_active;
	int minion_sent;
	GameObject minion_prefab;
	public List<GameObject> wps;

	//take down what to spawn
	public List<Minion.Minion_Type> NormalWave, CannonWave, SuperWave;
	WaveType type;

	// Use this for initialization
	void Start () {
		init (Types.Lanes.MID, Types.Faction.BLUE);
	}

	void init(Types.Lanes _lane = Types.Lanes.OTHER, Types.Faction _fac = Types.Faction.OTHER){
		//fac = _fac;
		//lane = _lane;
		type = WaveType.NORMAL;

		wave_active = false;
		timer_wave = new Utility.Timer (GameSceneConsts.minion_wave_interval);
		timer_wave.finish ();
		timer_micro = new Utility.Timer (GameSceneConsts.minion_spawn_interval);
		timer_wave.start_timer ();
		timer_micro.start_timer ();
		//load Resources
		minion_prefab = Resources.Load("Prefabs/Minions/minion_test") as GameObject;
	}

	// Update is called once per frame
	void Update () {
		timer_wave.update_timer ();

		if (timer_wave.is_over ()) {
			wave_active = true;
			minion_sent = 0;
			timer_wave.restart ();
		}
			
		if (wave_active) {
			timer_micro.update_timer ();
			if (timer_micro.is_over ()) {
				int minion_num = NormalWave.Count;
				List<Minion.Minion_Type> list = NormalWave;
				if (type == WaveType.ENHANCED) {
					minion_num = CannonWave.Count;
					list = CannonWave;
				} else if (type == WaveType.SUPER) {
					minion_num = SuperWave.Count;
					list = SuperWave;
				}
					
				spawn_minion_check_flag (list, minion_num);
			}
		}
	}

	void spawn_minion_check_flag(List<Minion.Minion_Type> tosend, int army_size){
		if (minion_sent < army_size) {
			timer_micro.restart ();
			//spawn and assemble the minion here
			GameObject new_minion = Instantiate(minion_prefab, transform.position, Quaternion.identity) as GameObject;
			GameObject navunit = Instantiate (GameSceneConsts.NavUnit_minion, transform.position, Quaternion.identity) as GameObject;
			UnityEngine.AI.NavMeshAgent navagent = navunit.GetComponent<UnityEngine.AI.NavMeshAgent> ();
			navagent.updateRotation = false;
			//navagent.updatePosition = false;
			NetworkServer.Spawn (navunit);
			Minion minion_script = Utility.get_minion_script (new_minion);
			minion_script.navagent = navunit.GetComponent<UnityEngine.AI.NavMeshAgent> ();
			AI_Minion minion_AI = Utility.get_AI_script (new_minion) as AI_Minion;
			UI_Minion minion_UI = Utility.get_UI (new_minion) as UI_Minion;
			//assemble the minion
			minion_script.init (wps, _type:tosend[minion_sent], _fac: fac);
			minion_script.update_movement_speed ();
			minion_script.init_set_AI (minion_AI, AI.PotentialTarget.ENEMY_TEAM, AI.Decision.OBJECTIVE);
			minion_UI.init ();
            minion_script.init_netUnit();
            NetworkServer.Spawn (new_minion);

			//TODO temp solution
			if(fac == Types.Faction.RED)
				new_minion.transform.FindChild("Unit").FindChild("Mesh").FindChild("Cube").gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

			minion_sent += 1;
		} else {
			//this wave is finished
			wave_active = false;
		}
	}

	//can only be called before a wave spawn
	public bool setWaveType(WaveType _type){
		if (wave_active)
			return false;
		
		type = _type;
		return true;
	}
}
