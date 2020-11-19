using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameSceneConsts : MonoBehaviour {
	//singleton
	public static GameSceneConsts instance = null;

	public static float dist_multiplier = 0.005f;
	//public static float camera_dist_multiplier = 0.0002f;
	public static float attack_speed_cap = 2.5f;
	public static float in_combat_time = 5f;

	//tag names
	public static string GROUND_TAG = "Ground";
	public static string CHAMPION_TAG = "Champion";
	public static string MINION_TAG = "Minion";
	public static string TOWER_TAG = "Tower";
	public static string OBJECTIVE_TAG = "Objective";//inhibiters, Nexus, etc.
	public static string NETURAL_TAG = "Netural";
	public static string ITEM_TAG = "Item";//ward, etc

	public static int LAYER_DEFAULT = 0;
	public static int LAYER_DEAD = 9;
	public static int LAYER_UNTARGETABLE = 8;

	//gameplay
	public static int max_players = 10;
	public static int max_per_team = 5;

	public static int max_level = 18;
	public static int min_level = 1;
	public static int max_skill_level = 5;
	public static int max_ult_level = 3;
	public static int min_skill_level = 0;

	public static float minion_wave_interval = 90f;//1.5min
	public static float minion_spawn_interval = 0.5f;
	public static int minion_wave_num = 6;//a normal wave
	public static int minion_wave_plus = 7;//with cannon minion

	public static float minion_wp_error = 1f;
	public static float champion_wp_error = 0.2f;
	public static float ai_max_chase_range = 2f;
	public static float ai_takeover_time = 0.5f;
	public static GameObject blue_team_respawn_pt;
	public static GameObject red_team_respawn_pt; 

	public static int max_item = 6;
	public static int max_supp_item = 1;//wards, etc

	public static float regen_interval = 0.5f;

	//Common resources
	//UIs
	public static GameObject hundred_line_prefab;
	public static GameObject thousand_line_prefab;
	public static GameObject dmg_txt_prefab;
	public static GameObject shopItem_prefab;
	public static GameObject NavUnit_player;
	public static GameObject NavUnit_minion;

	//Assets
	public static GameObject player_prefab;
	public static GameObject tower_prefab;
	public static GameObject inhib_prefab;

	//Maps
	GameObject Summoner_s_rift_normal;

	//Items
	//WARNING: never change those after init
	public static string[] item_names;
	public static int[] item_prices;
	public static int[][] item_comps;

	public void init(){
		/*
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);   

		DontDestroyOnLoad (gameObject);
		*/

		loadResources ();
	}

	void loadResources(){
		//UI
		hundred_line_prefab = Resources.Load ("Prefabs/UI/HP_100_line_prefab") as GameObject;
		thousand_line_prefab = Resources.Load ("Prefabs/UI/HP_1000_line_prefab") as GameObject;
		dmg_txt_prefab = Resources.Load ("Prefabs/UI/dmg_txt_prefab") as GameObject;
		shopItem_prefab = Resources.Load ("Prefabs/UI/shopItem") as GameObject;

		//data base
		load_item_database();
		NavUnit_minion = Resources.Load ("Prefabs/Minions/NavUnit_minion") as GameObject;
		NavUnit_player = Resources.Load ("Prefabs/Common/NavUnit_player")as GameObject;

		//Assets
		player_prefab = Resources.Load ("Prefabs/Common/Player_prefab") as GameObject;
		tower_prefab = Resources.Load ("Prefabs/Common/Tower_prefab") as GameObject;
		inhib_prefab = Resources.Load ("Prefabs/Common/Inhibiter_prefab") as GameObject;
	}

	void load_item_database(){
		item_names = new string[Utility.get_num_all_items() + 3];
		item_prices = new int[Utility.get_num_all_items() + 3];
		item_comps = new int[(Utility.get_num_all_items () + 3)][];
		for (int i = 0; i < item_comps.Length; ++i) {
			item_comps [i] = new int[5] {(int)Item.Item_name.DUMMY, (int)Item.Item_name.DUMMY, (int)Item.Item_name.DUMMY, (int)Item.Item_name.DUMMY, (int)Item.Item_name.DUMMY};
		}

		//names
		item_names [(int)Item.Item_name.LONG_SWORD] = "Long Sword";
		item_names [(int)Item.Item_name.PICKAXE] = "Pickaxe";
		item_names [(int)Item.Item_name.LAST_WHISPER] = "Last Whisper";
		item_names [(int)Item.Item_name.INFINITY_EDGE] = "infinity Edge";
		item_names [(int)Item.Item_name.YOUMUUS_GHOSTBLADE] = "Youmuu's Ghostblade";
		item_names [(int)Item.Item_name.BOOTS_OF_SPEED] = "Boots of speed";
		item_names [(int)Item.Item_name.DUMMY] = "Dummy";


		//prices
		item_prices [(int)Item.Item_name.LONG_SWORD] = 350;
		item_prices [(int)Item.Item_name.PICKAXE] = 875;
		item_prices [(int)Item.Item_name.LAST_WHISPER] = 425;
		item_prices [(int)Item.Item_name.INFINITY_EDGE] = 233;
		item_prices [(int)Item.Item_name.YOUMUUS_GHOSTBLADE] = 666;
		item_prices [(int)Item.Item_name.BOOTS_OF_SPEED] = 300;
		item_prices [(int)Item.Item_name.DUMMY] = 0;

		//comps
		item_comps [(int)Item.Item_name.LAST_WHISPER][0] = (int)Item.Item_name.PICKAXE;
	}
}
