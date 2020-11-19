using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//the class with all command funtions
public class ChampSelNetworkPlayer : NetworkBehaviour {

	public ChampSelectManager ManagerInstance;
	int idx;

	public void init(int _idx){
		ManagerInstance = ChampSelectManager.singleton as ChampSelectManager;
		idx = _idx;
	}

	[Command]
	//exit
	public void CmdOnClientExitLobby(){
		ManagerInstance.OnPlayerExit (idx);
	}

	[Command]
	//choose/change faction
	public void CmdOnPlayerChangeFaction(){
		//ManagerInstance.OnPlayerChangeFaction (idx);
		/*
		ChampSelectPlayer player = ManagerInstance.players[idx].GetComponent<ChampSelectPlayer> ();
		if (player.fac == Types.Faction.RED) {
			if (!ManagerInstance.RED_players.Contains (player.gameObject)) {
				ManagerInstance.RED_players.Add (player.gameObject);
				ManagerInstance.RED_idxs.Add (player.idx);
			}if (ManagerInstance.BLUE_players.Contains (player.gameObject)) {
				ManagerInstance.BLUE_players.Remove (player.gameObject);
				ManagerInstance.BLUE_idxs.Add (player.idx);
			}
		} else if (player.fac == Types.Faction.BLUE) {
			if (!ManagerInstance.BLUE_players.Contains (player.gameObject)) {
				ManagerInstance.BLUE_players.Add (player.gameObject);
				ManagerInstance.BLUE_idxs.Add (player.idx);
			}if (ManagerInstance.RED_players.Contains (player.gameObject)) {
				ManagerInstance.RED_players.Remove (player.gameObject);
				ManagerInstance.RED_idxs.Remove (player.idx);
			}
		}
		*/
		ManagerInstance.refresh_player_panel ();
	}


}
