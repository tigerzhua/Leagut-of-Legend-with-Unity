using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//maintain a list of anyone inside the effective range of this unit
public class AttackTarget : MonoBehaviour {

	public List<GameObject> targets;
	public Collider champ_col;//yourself
	public Unit host;
	public Types.Faction fac;
	List<GameObject> listedby;//who targeted me?
	List<GameObject> temp_untargetable;//someone still in range but still untargetable

	// Update is called once per frame
	void Update () {
	
	}

	public void init(Collider _col, Types.Faction _fac, float _radius){
		champ_col = _col;
		host = _col.gameObject.GetComponent<Unit> ();
		fac = _fac;
		GetComponent<CapsuleCollider> ().radius = _radius;
		targets = new List<GameObject> ();
		listedby = new List<GameObject> ();
		temp_untargetable = new List<GameObject> ();
	}

	public void inform_target_source_death(){
		foreach (GameObject source in listedby) {
			if(source != null)
				source.GetComponent<AttackTarget> ().recheck_targets (host.gameObject);//not at best performance
		}
		listedby.Clear ();
	}

	public void recheck_targets(GameObject src){
		if (src == host.cur_target_obj)
			host.cur_target_obj = null;

		List<GameObject> trashbin = new List<GameObject>();
		for (int i = 0; i < targets.Count; ++i) {
			if (targets [i] == null)
				trashbin.Add (targets [i]);
			else {
				if (!Utility.LayerTargetable (targets [i])) {
					trashbin.Add (targets [i]);
					if (!temp_untargetable.Contains (targets [i]))
						temp_untargetable.Add (targets [i]);
				}
			}
		}

		foreach (GameObject obj in trashbin) {
			targets.Remove (obj);
		}

		trashbin.Clear ();
		for (int i = 0; i < listedby.Count; ++i) {
			if (listedby [i] == null)
				trashbin.Add (listedby [i]);
			else {
				if (Utility.get_unit_object_peer (listedby [i]) == null)
					trashbin.Add (listedby [i]);
				else {
					if (!Utility.LayerTargetable (listedby [i])) {
						trashbin.Add (listedby [i]);
						if (!temp_untargetable.Contains (targets [i]))
							temp_untargetable.Add (targets [i]);
					}
				}
			}
		}

		foreach (GameObject obj in trashbin) {
			listedby.Remove (obj);
		}
	}
		
	public void move_back_list_element(){//move stuff back from temp_untargetable
		List<GameObject> trashbin = new List<GameObject>();
		for (int i = 0; i < temp_untargetable.Count; ++i) {
			if (temp_untargetable [i] == null)
				trashbin.Add (temp_untargetable [i]);
			else {
				if (Utility.LayerTargetable (temp_untargetable [i])) {
					OnTriggerEnter_obj (temp_untargetable[i]);
					trashbin.Add (temp_untargetable [i]);
				}
			}
		}
		foreach (GameObject obj in trashbin) {
			temp_untargetable.Remove (obj);
		}

		trashbin.Clear ();
	}

	void OnTriggerEnter(Collider col){
		//dont do anything if it's not local, or if it's a human
		if ( (!host.netID.isLocalPlayer)&&(host.isPlayer) )
			return;
		//..and not a server-controlled object
		if (!host.netID.isServer)
			return;
		
		if(col!=champ_col){
			if (col.gameObject.GetComponent<Unit> () != null) {
				Unit tgt = col.gameObject.GetComponent<Unit> ();
				if ( (Utility.LayerTargetable(col.gameObject))&&(!tgt.AtkTarget.GetComponent<AttackTarget>().listedby.Contains (gameObject)))//inform the other side we targeted them
					tgt.AtkTarget.GetComponent<AttackTarget>().listedby.Add (gameObject);
			}

			if( (col.gameObject.GetComponent<Unit> ().fac != fac)&&(Utility.TagTargetable(col.gameObject)))
				targets.Add(col.gameObject);

			if ((col.gameObject.GetComponent<Unit> ().fac != fac)&&(!Utility.LayerTargetable(col.gameObject))){
				if(!temp_untargetable.Contains(col.gameObject))
					temp_untargetable.Add(col.gameObject);
			}
		}
	}

	void OnTriggerExit(Collider col){
		//dont do anything if it's not local, or if it's a human
		if ( (!host.netID.isLocalPlayer)&&(host.isPlayer) )
			return;
		//..and not a server-controlled object
		if (!host.netID.isServer)
			return;

		if (col.gameObject.GetComponent<Unit> () != null) {
			Unit tgt = col.gameObject.GetComponent<Unit> ();
			if (tgt.AtkTarget.GetComponent<AttackTarget>().listedby.Contains (gameObject))//inform the other side we targeted them
				tgt.AtkTarget.GetComponent<AttackTarget>().listedby.Remove (gameObject);
		}

		if (targets.Contains (col.gameObject))
			targets.Remove (col.gameObject);

		if (temp_untargetable.Contains (col.gameObject))
			temp_untargetable.Remove (col.gameObject);
	}

	void OnTriggerEnter_obj(GameObject obj){
		if(obj!=host.gameObject){
			if( (obj.GetComponent<Unit> ().fac != fac)&&(Utility.TagTargetable(obj)))
				targets.Add(obj);
		}
	}
}
