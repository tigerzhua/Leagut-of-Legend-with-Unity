using UnityEngine;
using System.Collections;

//This parent class is for auto-attack
//since auto attack happens very often, I'll keep it simple
public class Projectile : MonoBehaviour {

	BoxCollider col;
	Types.damage_combo dmg;
	bool guided;
	bool invalid;
	float speed;
	GameObject target;
	Vector3 targetPos;
	GameObject parent_obj;
	Unit host;

	public void init(Unit _host, Types.damage_combo _dmg, float _speed, bool _guided = false, GameObject _tgt = null){
		col = GetComponent<BoxCollider>();
		host = _host;
		parent_obj = transform.parent.gameObject;

		guided = _guided;
		invalid = false;
		target = _tgt;
		targetPos = _tgt.transform.position;
		dmg = _dmg;
		speed = _speed * GameSceneConsts.dist_multiplier;

		transform.LookAt (target.transform.position);
	}

	void Update(){
		Vector3 dir = transform.forward;
		if (guided) {
			if ( (!invalid)&&(target != null)&&(Utility.LayerTargetable(target))) {
				targetPos = target.transform.position;
				transform.LookAt (target.transform.position);
				dir = (target.transform.position - transform.position).normalized;
				if (speed * Time.deltaTime >= Utility.get_dist_transform (transform, target.transform)) {
					transform.position = target.transform.position;
				} else {
					transform.position += dir * speed * Time.deltaTime;
				}
			} else {//e.g., target destroyed when projectile in mid air
				invalid = true;
				dir = (targetPos - transform.position).normalized;
				transform.LookAt (targetPos);
				if (speed * Time.deltaTime >= Utility.get_dist_position(transform.position, targetPos)) {
					transform.position = targetPos;
					self_destruction ();
				} else {
					transform.position += dir * speed * Time.deltaTime;
				}
			}
		} else {
			transform.position += dir * speed * Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider col){
		if (guided) {
			if (col.gameObject == target) {
				Unit tgt = target.GetComponent<Unit> ();
				tgt.change_incombat_state (true);//put this before receive damage incase of null
				if (tgt.receive_damage (host, true, dmg)) {
					if(host.cur_target_obj == target)
						host.cur_target_obj = null;
				}
				self_destruction ();
			}
		}
	}

	void self_destruction(){
		//TODO test only
		Destroy(transform.parent.gameObject);
	}
}
