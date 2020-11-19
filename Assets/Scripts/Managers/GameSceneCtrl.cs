using UnityEngine;
using System.Collections;

public enum Mouse_status{
	NORMAL,
	SELECT,//select target for skill/autoattack, etc.
	SPECIAL,//champion dependent, say Jhin's ult
}

public enum Onhold_action{//what action is on hold?
	NONE,//normally
	Q_skill,
	W_skill,
	E_skill,
	R_skill,
	Passive_skill,
}

public class GameSceneCtrl : MonoBehaviour {

	//Singleton
	public static GameSceneCtrl instance;

	//player
	GameObject player;
	Player playerScript;

	//champion
	GameObject player_champion;
	Champion champion_script;

	//physics
	Collider self_col;
	GameObject main_camera;
	float y_ref;

	//mouse part
	Vector3 target_pt;
	public Vector3 lookat_pt;
	public Mouse_status mouse_status;

	//display coeff flag
	Types.display_coeff dis_coff;

	//other
	Onhold_action onhold;

	//init function
	public void init(GameObject _player, Champion.champion_name _champ){
		player = _player;
		playerScript = player.GetComponent<Player> ();

		player_champion = Utility.get_champion_object (player);
		champion_script = player_champion.GetComponent<Champion> ();
		self_col = champion_script.self_col;

		main_camera = GameObject.Find ("Main Camera");
		y_ref = player.transform.position.y;
		target_pt = player.transform.position; 
		lookat_pt = player.transform.position;

		//init flag
		dis_coff.init();
		set_follow_camera (dis_coff.follow_camera);

		//mouse
		mouse_status = Mouse_status.NORMAL;
		onhold = Onhold_action.NONE;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (player == null)
			return;

		if (!champion_script.stunned ()) {
			//Utility.get_champion_transform (player).LookAt(lookat_pt);
		}

		//TODO transfer this to Unit
		if ( (champion_script.can_move()&&(champion_script.check_champion_special_move_condition())) ) {
			//player.transform.position = champion_script.get_navagent ().transform.position;
		}
	}
 
	//main loop
	void Update(){
		if (player == null)
			return;
		
		process_mouse_input ();
		process_keyboard_input ();
	}

	//main loop helpers
	void process_mouse_input(){
		//construct the ray
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (mouse_status == Mouse_status.NORMAL) {
			if (Input.GetButtonDown ("Fire2")) {//right click
				if (Physics.Raycast (ray, out hit)) {
					//
					champion_script.ai_takeover = false;
					target_pt = hit.point;
					lookat_pt = new Vector3 (hit.point.x, player_champion.transform.position.y, hit.point.z);
					if (!champion_script.flags.is_dead) {
						if (Utility.TagTargetable (hit.collider.gameObject) && hit.collider.gameObject.GetComponent<Unit> ().fac != champion_script.fac) {
							print ("click");
							champion_script.cur_target_obj = hit.collider.gameObject;
							//not moving when champing clicks to attack someone and it's in range
							if (!champion_script.within_range (champion_script.cur_target_obj)) {
								champion_script.moveto (target_pt);
							}
							champion_script.rotation_override = false;
						} else{//hit something untargetable
							champion_script.rotation_override = true;
							champion_script.moveto (target_pt);
						}
					}
				}
			} else if (Input.GetButtonDown ("Fire1")) {//left click
				if (Physics.Raycast (ray, out hit)) {
					if (Utility.TagTargetable (hit.collider.gameObject)) {
						champion_script.cur_target_obj = hit.collider.gameObject;
					} else
						champion_script.cur_target_obj = null;
				}
			} else {
				lookat_pt = player_champion.transform.position + new Vector3(player_champion.transform.forward.x, 0f, player_champion.transform.forward.z);
			}
		} else if(mouse_status == Mouse_status.SELECT){
			if (Input.GetButtonDown ("Fire2")) {//right click, calcels the selection
				mouse_status = Mouse_status.NORMAL;
			} else if (Input.GetButtonDown ("Fire1")) {//left click
				//
				champion_script.ai_takeover = false;
				mouse_status = Mouse_status.NORMAL;
				if (Physics.Raycast (ray, out hit)) {
					if ( (hit.collider.gameObject.tag == GameSceneConsts.CHAMPION_TAG)||
						 (hit.collider.gameObject.tag == GameSceneConsts.MINION_TAG)||
						 (hit.collider.gameObject.tag == GameSceneConsts.NETURAL_TAG) ) {
						champion_script.cur_target_obj = hit.collider.gameObject;
						switch (onhold) {
						case Onhold_action.Q_skill:
							playerScript.activate_Q_skill ();
							break;
						case Onhold_action.W_skill:
							playerScript.activate_W_skill ();
							break;
						case Onhold_action.E_skill:
							playerScript.activate_E_skill ();
							break;
						case Onhold_action.R_skill:
							playerScript.activate_R_skill ();
							break;
						case Onhold_action.Passive_skill:
							playerScript.activate_Passive_skill ();
							break;
						default:
							break;
						}
						onhold = Onhold_action.NONE;
					}
				}
			}
		}

		//pan camera over map
		if (!dis_coff.follow_camera) {
			float move_threshold = dis_coff.camera_move_threshold;
			Vector3 camera_dir = Vector3.zero;
			Vector3 mpos = Input.mousePosition;
			if (mpos.x <= move_threshold)
				camera_dir += Vector3.left;
			if (mpos.x >= (float)(Screen.width - move_threshold))
				camera_dir += Vector3.right;
			if (mpos.y <= move_threshold)
				camera_dir += Vector3.back;
			if (mpos.y >= (float)(Screen.height - move_threshold))
				camera_dir += Vector3.forward;
			
			camera_dir.Normalize ();
			main_camera.transform.position += camera_dir * dis_coff.camera_speed * Time.deltaTime;
		}
	}

	void process_keyboard_input(){
		if (Input.GetKeyDown (KeyCode.Y)) {//switch camera mod
			if (!dis_coff.follow_camera) {
				set_follow_camera (true);
			} else {
				set_follow_camera (false);
			}
		} else if (Input.GetKeyDown (KeyCode.Q)) {
			if (!playerScript.activate_Q_skill ()) {
				mouse_status = Mouse_status.SELECT;
				onhold = Onhold_action.R_skill;
			}
		} else if (Input.GetKeyDown (KeyCode.W)) {
			playerScript.activate_W_skill ();
		} else if (Input.GetKeyDown (KeyCode.E)) {
			playerScript.activate_E_skill ();
		} else if (Input.GetKeyDown (KeyCode.R)) {
			if (!playerScript.activate_R_skill ()) {
				mouse_status = Mouse_status.SELECT;
				onhold = Onhold_action.R_skill;
			}
		} else if (Input.GetKeyDown (KeyCode.D)) {
			playerScript.activate_summoner_spell_D ();
		} else if (Input.GetKeyDown (KeyCode.F)) {
			playerScript.activate_summoner_spell_F ();
		} else if (Input.GetKeyDown (KeyCode.B)) {
			playerScript.activate_recall ();
		}
	}

	//display helpers
	void set_follow_camera(bool flag){
		if (flag){
			dis_coff.follow_camera = true;
			main_camera.transform.position = player.transform.FindChild ("camera_pos").transform.position;
			main_camera.transform.SetParent (player.transform);
		} else{
			dis_coff.follow_camera = false;
			main_camera.transform.SetParent (null);
		}
	}
}
