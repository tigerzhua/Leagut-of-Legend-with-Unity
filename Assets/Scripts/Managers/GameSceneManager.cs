using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameSceneManager : MonoBehaviour {

	const int BLUE_STARTING_IDX = 0;
	const int RED_STARTING_IDX = 5;

	public static GameSceneManager instance = null;
	int num_players_RED = 0;
	int num_players_BLUE = 0;
	public GameObject[] players;//The top level player object, assign in inspector(size = 11)
	public bool[] isPlayerActive;
	public Utility.Timer[] PlayerDCTimer;//disconnect timer
	public Champion[] champions;//scripts
	GameObject[] Team_red;
	GameObject[] Team_blue;
	Champion.champion_name[] ChampionNames;//type of champions
	GameObject Map;
	public Camera main_camera;

	//IMPORTANT
	public int LoaclPlayerIdx;//the index of the human player on this machine
	UI_player_panel playerUI;
	public GameSceneCtrl ctrlScript;
	public UI_player_panel LocalUI;

	//all the stats

	void Setup(){
		isPlayerActive = new bool[10];
		champions = new Champion[10];
		ChampionNames = new Champion.champion_name[10];
		Team_red = new GameObject[5];
		Team_blue = new GameObject[5];

		for (int i = 0; i < isPlayerActive.Length; ++i) {
			isPlayerActive [i] = false;
			ChampionNames[i] = Champion.champion_name.START;
		}

		//Network Manager is already there
		ChampSelectManager champsel = GameObject.Find("NetworkManager").GetComponent<ChampSelectManager>();
		//for(int i = 0; i < champsel.BLUE_players; ++i){

		isPlayerActive [0] = true;
		isPlayerActive [5] = true;
	}

	void load_map(){
		Map = GameObject.Find ("Map_local");//TODO: should be instantiated
		GameSceneConsts.blue_team_respawn_pt = Map.transform.FindChild ("Spawn").FindChild ("Blue_spawn").gameObject;
		GameSceneConsts.red_team_respawn_pt = Map.transform.FindChild ("Spawn").FindChild ("Red_spawn").gameObject;
	}

	void Awake(){
		print ("Awake");
		instance = this;
		Setup ();
		load_map ();
		//init the input module
		gameObject.GetComponent<GameSceneConsts>().init();
		ctrlScript = gameObject.GetComponent<GameSceneCtrl> ();
		LocalUI = transform.FindChild("UI").gameObject.GetComponent<UI_player_panel>();
		//spawn all players
		/*
		for (int i = 0; i < players.Length; ++i) {
			if (!isPlayerActive [i])
				continue;

			if (i < RED_STARTING_IDX) {//blue team
				players [i] = Instantiate (GameSceneConsts.player_prefab, GameSceneConsts.blue_team_respawn_pt.transform.position, Quaternion.identity) as GameObject;
				players[i].GetComponent<Player> ().init(ChampionNames[i], Types.Faction.BLUE);
			} else {//red team
				players [i] = Instantiate (GameSceneConsts.player_prefab, GameSceneConsts.red_team_respawn_pt.transform.position, Quaternion.identity) as GameObject;
				players[i].GetComponent<Player> ().init(ChampionNames[i], Types.Faction.RED);
			}

			if (players [i].GetComponent<Player> ().isLocalPlayer) {
				print (i);
			}
		}
		*/

		//TODO: argument is tst only
		//send the local player on this machine to controller
		//gameObject.GetComponent<GameSceneCtrl>().init(players [LoaclPlayerIdx], ChampionNames[LoaclPlayerIdx]);
		main_camera = GameObject.Find ("Main Camera").gameObject.GetComponent<Camera>();
		//Utility.get_unit_script (players [LoaclPlayerIdx]).isPlayer = true;

		//init the UI
		//playerUI = transform.FindChild("UI").gameObject.GetComponent<UI_player_panel>();
		//playerUI.init ();

		//get all champions
		//champions[LoaclPlayerIdx] = Utility.get_unit_script(players[LoaclPlayerIdx]) as Champion;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public GameObject get_local_player(){
		return players [LoaclPlayerIdx];
	}

	public Champion get_local_champion(){
		return champions [LoaclPlayerIdx];
	}

	public void register_local_player(Player _player){
		LoaclPlayerIdx = _player.idx;
		players [LoaclPlayerIdx] = _player.gameObject;
	}
}
