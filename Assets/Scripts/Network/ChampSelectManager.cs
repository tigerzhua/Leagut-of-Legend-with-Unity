using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChampSelectManager : NetworkLobbyManager {

	List<GameObject> RED_players = new List<GameObject>();
	List<GameObject> BLUE_players = new List<GameObject> ();

	public Champion.champion_name[] champs_selected;
	//public Champion.champion_name[] ChampionNames = new Champion.champion_name[GameSceneConsts.max_players];//type of champions
	//UIs
	GameObject UIPanel;
	public GameObject champsellist;
	public GameObject champs;//the small window contains every champion
	GameObject bluePanel, redPanel;
	GameObject NewGamePanel;
	GameData db;//data base for the champ sel scene
	//buttons
	public GameObject button_blue, button_red, button_quit;
	//prefabs
	GameObject playerPanel_prefab, playerPanel_prefab_inverse;
	GameObject champButton_prefab;

    void Start(){
		//getting and assemble components
		LoadResources ();
		db = GetComponent<GameData> ();
		db.init ();
		UIPanel = transform.FindChild ("UI").gameObject;
		champsellist = UIPanel.transform.FindChild ("ChampSelList").gameObject;
		champs = champsellist.transform.FindChild ("Champs").gameObject;
		generate_champ_list ();
		champsellist.SetActive (true);
		NewGamePanel = UIPanel.transform.FindChild ("NewGame").gameObject;
		bluePanel = UIPanel.transform.FindChild ("BluePanel").gameObject;
		redPanel = UIPanel.transform.FindChild ("RedPanel").gameObject;
		button_blue = UIPanel.transform.FindChild ("Button_BLUE").gameObject;
		button_red = UIPanel.transform.FindChild ("Button_RED").gameObject;
		button_quit = UIPanel.transform.FindChild ("Button_Quit").gameObject;

		champs_selected = new Champion.champion_name[GameSceneConsts.max_players];
		for (int i = 0; i < champs_selected.Length; ++i) {
			champs_selected [i] = Champion.champion_name.START;
		}
	}

	void LoadResources(){
		playerPanel_prefab = Resources.Load ("Prefabs/Network/PlayerInfoPanel") as GameObject;
		playerPanel_prefab_inverse = Resources.Load ("Prefabs/Network/PlayerInfoPanel_inverse") as GameObject;
		champButton_prefab = Resources.Load ("Prefabs/UI/ChampSel/ChampSelButton") as GameObject;
	}
		
	public void update_min_players(){
		int count = 0;
		for (int i = 0; i < lobbySlots.Length; ++i) {
			if (lobbySlots [i] != null)
				count += 1;
		}
		minPlayers = count;
	}

	//public void CheckReadyToBegin(){
	//	print ("Ready received!");
	//}

	//check if everyone is ready, start the game when true
	public override void OnLobbyServerPlayersReady(){
		print ("everyone ready!");
		base.OnLobbyServerPlayersReady ();
		//ServerChangeScene (playScene);
		//OnLobbyServerSceneLoadedForPlayer (, );
	}

	void OnLevelWasLoaded(int level) {
		if (level == 2) {//"main scene"
			//NetworkServer.SpawnObjects();
			UIPanel.SetActive (false);
		}

	}

	//public override void OnServerSceneChanged(string sceneName){
	//
	//}

	//void OnPlayerDisconnected(NetworkPlayer player) {
	//	print ("OnPlayerDisconnected");
	//}

	public void reg_host_player(ChampSelectPlayer _player){}
		
	public Types.Faction PickAFaction(){
		if (BLUE_players.Count < GameSceneConsts.max_per_team)
			return Types.Faction.BLUE;
		else if(RED_players.Count < GameSceneConsts.max_per_team)
			return Types.Faction.RED;

		return Types.Faction.OTHER;
	}

	public void OnPlayerChangeFaction(int _idx, bool isQuit = false){
		print ("OnPlayerChangeFaction");
		refresh_player_panel (isQuit);
	}

	public int generate_player_idx(ChampSelectPlayer _player){
		//go through all players
		for (int i = 0; i < lobbySlots.Length; ++i) {
			if (lobbySlots [i].gameObject == _player.gameObject)
				return i;
		}

		return -1;
	}
	/*
	public void OnLobbyClientExit(){
		print ("OnLobbyClientExit");
	}
	public void OnLobbyClientDisconnect(NetworkConnection conn){
		print ("OnLobbyClientDisconnect");
	}

	public void OnLobbyServerConnect(NetworkConnection conn){
		print ("OnLobbyServerConnect");
	}
	*/

	public void OnPlayerExit(int _idx){
		print ("OnPlayerExit");
		ChampSelectPlayer player = lobbySlots [_idx].GetComponent<ChampSelectPlayer> ();
		player.Cmdchange_fac (Types.Faction.OTHER, _idx);
	}
	/*
	public void OnLobbyStartClient(){
		print ("OnLobbyServerDisconnect");
	}

	public void OnLobbyStartHost(){
		print ("started");
	}

	public void OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short it){
		print ("dsds");
	}
	*/

	//buttons
	public void change_team_RED(int _idx){
		ChampSelectPlayer player = lobbySlots [_idx].GetComponent<ChampSelectPlayer> ();
		if (!player.isLocalPlayer)
			return;
		
		player.Cmdchange_fac (Types.Faction.RED, _idx);
		//OnPlayerChangeFaction (_idx);
	}

	public void change_team_BLUE(int _idx){
		ChampSelectPlayer player = lobbySlots [_idx].GetComponent<ChampSelectPlayer> ();
		if (!player.isLocalPlayer)
			return;

		player.Cmdchange_fac (Types.Faction.BLUE, _idx);
		//OnPlayerChangeFaction (_idx);
	}

	public void sel_champ(Champion.champion_name _name, int _idx){
		ChampSelectPlayer player = lobbySlots [_idx].GetComponent<ChampSelectPlayer> ();
		if (!player.isLocalPlayer)
			return;
		
		player.Cmdchange_champ (_name, _idx);
	}

	public void open_ChampSel(){champsellist.SetActive (true);}
	public void close_ChampSel(){champsellist.SetActive (false);}

	public void OnClickHost(){StartHost (); closeNewGamePanel ();}
	public void OnClickJoin(){StartClient (); closeNewGamePanel ();}
	public void OnClickQuit(int _idx){
		ChampSelectPlayer player = lobbySlots [_idx].GetComponent<ChampSelectPlayer> ();
		if (!player.isLocalPlayer)
			return;
		
		player.Cmdchange_fac (Types.Faction.OTHER, _idx);
		//StopClient ();
		//StopHost (); 
		//openNewGamePanel ();
	}
	public void closeNewGamePanel(){NewGamePanel.SetActive (false);}
	public void openNewGamePanel(){NewGamePanel.SetActive (true);}

	//graphics
	public void refresh_player_panel(bool isQuit = false){
		//make the team list first
		BLUE_players.Clear ();
		RED_players.Clear ();
		for (int i = 0; i < lobbySlots.Length; ++i) {
			if (lobbySlots [i] != null) {
				ChampSelectPlayer player = lobbySlots [i].gameObject.GetComponent<ChampSelectPlayer> ();
				if (player.fac == Types.Faction.RED)
					RED_players.Add (player.gameObject);
				else if (player.fac == Types.Faction.BLUE)
					BLUE_players.Add (player.gameObject);
			}
		}

		string obj_name;
		//clear existing objects
		for (int j = 0; j < bluePanel.transform.childCount; ++j){
			for(int k = 0; k < bluePanel.transform.GetChild(j).childCount; ++k)
				Destroy (bluePanel.transform.GetChild(j).GetChild(k).gameObject);
		}

		for (int i = 0; i < BLUE_players.Count; ++i) {
			obj_name = "P" + (i+1).ToString ();
			GameObject child = bluePanel.transform.FindChild (obj_name).gameObject;
			GameObject newPanel = Instantiate (playerPanel_prefab, child.transform.position, Quaternion.identity) as GameObject;
			newPanel.transform.SetParent(child.transform);
			newPanel.transform.localScale = new Vector3 (1, 1, 1);
			ChampSelectPlayer player = BLUE_players[i].GetComponent<ChampSelectPlayer> ();
			newPanel.transform.FindChild ("Name").GetComponent<Text> ().text = player.name;
		}

		for (int j = 0; j < redPanel.transform.childCount; ++j){
			for(int k = 0; k < redPanel.transform.GetChild(j).childCount; ++k)
				Destroy (redPanel.transform.GetChild(j).GetChild(k).gameObject);
		}

		for (int i = 0; i < RED_players.Count; ++i) {
			obj_name = "P" + (GameSceneConsts.max_per_team+i+1).ToString ();
			GameObject child = redPanel.transform.FindChild (obj_name).gameObject;
			GameObject newPanel = Instantiate (playerPanel_prefab_inverse, child.transform.position, Quaternion.identity) as GameObject;
			newPanel.transform.SetParent(child.transform);
			newPanel.transform.localScale = new Vector3 (1, 1, 1);
			ChampSelectPlayer player = RED_players[i].GetComponent<ChampSelectPlayer> ();
			newPanel.transform.FindChild ("Name").GetComponent<Text> ().text = player.name;
		}
		if (isQuit) {
			StopClient ();
			//StopHost (); 
			//openNewGamePanel ();
		}
	}

	void generate_champ_list(){
		int total_num = Utility.get_num_all_champions();
		for (int i = 1; i <= total_num; ++i) {
			GameObject newChamp = Instantiate (champButton_prefab, Vector3.zero, Quaternion.identity) as GameObject;
			ChampInfo champinfo = newChamp.GetComponent<ChampInfo> ();
			champinfo.name = (Champion.champion_name)i;
			Button button = newChamp.GetComponent<Button>();
			newChamp.transform.FindChild("Name").gameObject.GetComponent<Text>().text = GameData.champ_names[(int)champinfo.name];
			newChamp.transform.SetParent (champs.transform);
		}
	}

	void copy_player_selection(){
		for (int i = 0; i < BLUE_players.Count; ++i) {
			champs_selected [i] = BLUE_players [i].GetComponent<ChampSelectPlayer> ().champ_chosen;
		}
		for (int i = 0; i < RED_players.Count; ++i) {
			champs_selected [i+GameSceneConsts.max_per_team] = RED_players [i].GetComponent<ChampSelectPlayer> ().champ_chosen;
		}
	}

	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
		print ("OnLobbyServerSceneLoadedForPlayer");
		ChampSelectPlayer lobbyScript = lobbyPlayer.GetComponent<ChampSelectPlayer> ();
		print (lobbyScript.idx);
		//if (lobbyScript.isLocalPlayer) {
			UIPanel.SetActive (false);
		
			Player playerScript = gamePlayer.GetComponent<Player> ();
			if (lobbyScript.fac == Types.Faction.BLUE)
				gamePlayer.transform.position = GameSceneConsts.blue_team_respawn_pt.transform.position;
			else if (lobbyScript.fac == Types.Faction.RED)
				gamePlayer.transform.position = GameSceneConsts.red_team_respawn_pt.transform.position;
			
		playerScript.takedown_paras (lobbyScript.champ_chosen, lobbyScript.fac, lobbyScript.idx);
		playerScript.idx = lobbyScript.idx;
			//playerScript.init (lobbyScript.champ_chosen, lobbyScript.fac, lobbyScript.idx);
			return true;
		//}
		return false;
	}
}
