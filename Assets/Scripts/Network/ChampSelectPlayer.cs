using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ChampSelectPlayer : NetworkLobbyPlayer {

	//player info
	[SyncVar(hook="OnTeamChanging")]
	public Types.Faction fac;
	[SyncVar(hook="OnChampChanging")]
	public Champion.champion_name champ_chosen;

	public int idx;
	public string name;
	GameObject redcube_prefab;

	//references
	ChampSelectManager ManagerInstance;

	public void OnTeamChanging(Types.Faction _fac){
		print ("OnTeamChanging");
		//fac = _fac;
		//ManagerInstance.OnPlayerChangeFaction (idx);
	}

	public void OnChampChanging(Champion.champion_name _name){
		//champ_chosen = _name;
	}

	void Awake(){
		DontDestroyOnLoad (gameObject);
	}

	void Start(){
		print ("New player Connected");
		redcube_prefab = Resources.Load ("Prefabs/test/RedBoxPrefab") as GameObject;
		//reg player
		ManagerInstance = ChampSelectManager.singleton as ChampSelectManager;
		idx = ManagerInstance.generate_player_idx(this);
		//ManagerInstance.reg_host_player (this);

		//setup player info
		champ_chosen = Champion.champion_name.END;//the dummy one
		name = "Player" + idx.ToString ();
		if(isLocalPlayer)
			Cmdchange_fac( ManagerInstance.PickAFaction (), idx );
		Reg_buttons(idx);
	}

	void Reg_buttons(int _idx){
		ManagerInstance.button_blue.GetComponent<Button>().onClick.AddListener (delegate {ManagerInstance.change_team_BLUE (idx);});
		ManagerInstance.button_red.GetComponent<Button>().onClick.AddListener (delegate {ManagerInstance.change_team_RED (idx);});
		ManagerInstance.button_quit.GetComponent<Button> ().onClick.AddListener (delegate {ManagerInstance.OnClickQuit (idx);});
		//reg champ select button
		for (int i = 0; i < ManagerInstance.champs.transform.childCount; ++i) {
			ChampInfo champinfo = ManagerInstance.champs.transform.GetChild(i).GetComponent<ChampInfo> ();
			Button button = ManagerInstance.champs.transform.GetChild(i).GetComponent<Button>();
			button.onClick.AddListener (delegate{ManagerInstance.sel_champ(champinfo.name, idx);});//champinfo.name
		}
		//reg lock button
		ManagerInstance.champsellist.transform.FindChild("Button_lock").gameObject.GetComponent<Button>().onClick.AddListener(delegate {
			SendReady(true);
		});
	}

	void SendReady(bool ready){
		if (!isLocalPlayer)
			return;

		if(ready)
			SendReadyToBeginMessage();
	}

	[Command]
	public void Cmdchange_fac(Types.Faction _fac, int _idx){
		print ("Cmdchange_fac");
		ManagerInstance.update_min_players ();
		RpcChangefac (_fac);
		//fac = _fac;
		//ManagerInstance.OnPlayerChangeFaction (idx);
	}

	[ClientRpc]
	public void RpcChangefac(Types.Faction _fac){
		//if (!isLocalPlayer)
		//	return;
		print ("RpcChangefac");
		fac = _fac;
		if(_fac != Types.Faction.OTHER)
			ManagerInstance.OnPlayerChangeFaction (idx);
		else
			ManagerInstance.OnPlayerChangeFaction (idx, true);
	}

	[Command]
	public void Cmdchange_champ(Champion.champion_name _name, int _idx){
		RpcChangechamp (_name);
		//fac = _fac;
		//ManagerInstance.OnPlayerChangeFaction (idx);
	}

	[ClientRpc]
	public void RpcChangechamp(Champion.champion_name _name){
		//if (!isLocalPlayer)
		//	return;
		champ_chosen = _name;
		ManagerInstance.refresh_player_panel ();
	}
		
	public override void OnClientExitLobby(){
		print ("exit!");
		//if(isLocalPlayer)
		//	Cmdchange_fac (Types.Faction.OTHER, idx);
		//ManagerInstance.OnPlayerExit (idx);
	}


	//public override void OnClientEnterLobby(){
	//public override void OnStartLocalPlayer(){
	//
	//}

	public override void OnNetworkDestroy(){//strangely this function is not called for host player
		print ("destroyed!");
	}

	//void OnDestroy(){
	//	//ManagerInstance.OnPlayerExit (idx);
	//	print ("OnDestroy");
	//}
}
