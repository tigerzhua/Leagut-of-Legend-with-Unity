using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameData : MonoBehaviour {

	public static string[] champ_names;
	public static Image[] champ_portrait;
	public void init(){
		champ_names = new string[Utility.get_num_all_champions()+2];
		loadNames ();
	}

	void loadNames(){
		champ_names[(int)Champion.champion_name.Garen] = "Garen";
		champ_names[(int)Champion.champion_name.Ryze] = "Ryze";
		champ_names[(int)Champion.champion_name.Ashe] = "Ashe";
	}
}
