using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkUnit : NetworkBehaviour {

	public Unit unit;
    [SyncVar(hook = "Refresh_UI")]
    public float max_hp;
    [SyncVar(hook = "Refresh_UI")]
    public float cur_hp;
    [SyncVar(hook = "Refresh_UI")]
    public float max_mana;
    [SyncVar(hook = "Refresh_UI")]
    public float cur_mana;

    public void init(Unit _unit, float hp, float mana){
        unit = _unit;
        max_hp = hp;
        cur_hp = max_hp;
        max_mana = mana;
        cur_mana = max_mana;
    }


    [Command]
	public void CmdAutoAttack(){
		GameObject toSpawn = unit.auto_attack (unit.cur_target_obj);
		if (toSpawn != null)
			NetworkServer.Spawn (toSpawn);
	}

	[Command]
	public void CmdActiveSkill(Types.Skill_Slot skill){
	}

    [Command]
    public void Cmdinstantiate_navunit(GameObject navunit){
        NetworkServer.Spawn(navunit);
    }
    //change max hp
    [Command]
    public void CmdchangeMaxHp(float new_maxHP) { RpcChangeMaxHp(new_maxHP); }

    [ClientRpc]
    public void RpcChangeMaxHp(float new_maxHP) { max_hp = new_maxHP; Refresh_UI(new_maxHP); }

    [Command]
    public void CmdchangeMaxMana(float new_maxMana) { RpcChangeMaxMana(new_maxMana); }

    [ClientRpc]
    public void RpcChangeMaxMana(float new_maxMana) { max_mana = new_maxMana; Refresh_UI(new_maxMana); }

    [Command]
    public void CmdchangeCurHp(float new_curHp) { RpcChangeCurHp(new_curHp); }

    [ClientRpc]
    public void RpcChangeCurHp(float new_curHp) { cur_hp = new_curHp; Refresh_UI(new_curHp); }

    [Command]
    public void CmdchangeCurMana(float new_curMana) { RpcChangeCurMana(new_curMana); }

    [ClientRpc]
    public void RpcChangeCurMana(float new_curMana) { cur_mana = new_curMana; Refresh_UI(new_curMana); }

    void Refresh_UI(float new_val){
        unit.ui.update_bar_fillamount( cur_hp/ max_hp);
    }
}
