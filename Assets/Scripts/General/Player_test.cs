using UnityEngine;
using System.Collections;

public class Player_test : MonoBehaviour {

	//which champion this player is
	Champion.champion_name champ_type;
	Champion champ;
	float gold;

	public void Start(){
		//champ_type = Champion.champion_name.Garen;//TODO test only

		//load the champion
		//first of all, clear the champion object
		//foreach (var comp in Utility.get_champion_object (gameObject).GetComponents<Component>()){
		//	if (!(comp is Transform))
		//		Destroy(comp);
		//}
		//then assign the right champion
		//switch (champ_type) {
		//case Champion.champion_name.Garen:
		//	champ = Utility.get_champion_object (gameObject).AddComponent <Garen>() as Champion;
		//	champ.init ();
		//	break;
		//default:
		//	break;
		//}

		//TODO test only
		champ = Utility.get_champ_script(gameObject);
		UI_Champion champ_UI = Utility.get_UI_obj(gameObject).GetComponent<UI_Champion> ();
		champ_UI.init ();
		champ.ui = champ_UI;
		gold = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void activate_Q_skill() {
		champ.act_Q ();
	}

	public void activate_W_skill() {
		champ.act_W ();
	}

	public void activate_E_skill() {
		champ.act_E ();
	}

	public void activate_R_skill() {
		champ.act_R ();
	}

	public void activate_passive_skill(){
		champ.act_Passive ();
	}
}
