using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//utility class to handle some basic and specific tasks
public class Utility : MonoBehaviour {

	public class Timer{
		public int time_remaining;
		public int max_time;
		int step;//more accurate

		public Timer(float mtime){
			max_time = (int)(mtime*100);
			time_remaining = max_time;
			step = 0;
		}

		public void update_timer(){
			if(!is_over())
				time_remaining -= (int)(step * Time.deltaTime);
		}

		public bool is_over(){
			if (time_remaining <= 0)
				return true;

			return false;
		}
		//immediately finish the timer
		public void finish(){time_remaining = 0;}
		public void change_max_timer(float new_max){max_time = (int)(new_max*100);}
		public void start_timer(){step = 100;}
		public void pause(){step = 0;}
		public void reset_timer(){
			time_remaining = max_time;
			step = 0;
		}

		public void restart(){
			reset_timer ();
			start_timer ();
		}

		//member access
		public int get_max_time(){return max_time;}
		public int get_time_remaining(){return time_remaining;}
	}

	//pass in the root objet, return what we want
	//Unit
	public static GameObject get_champion_object(GameObject obj){return obj.transform.FindChild("Unit").gameObject;}
	public static Transform get_champion_transform(GameObject obj){return obj.transform.FindChild("Unit");}
	public static GameObject get_unit_object(GameObject obj){return obj.transform.FindChild("Unit").gameObject;}
	public static GameObject get_unit_object_peer(GameObject obj){return obj.transform.parent.FindChild("Unit").gameObject;}
	public static Transform get_unit_transform(GameObject obj){return obj.transform.FindChild("Unit");}
	public static Unit get_unit_script(GameObject obj){return obj.transform.FindChild ("Unit").gameObject.GetComponent<Unit>();}
	public static Unit get_unit_script_peer(GameObject obj){return obj.transform.parent.FindChild ("Unit").gameObject.GetComponent<Unit>();}
	public static Champion get_champ_script(GameObject obj){return obj.transform.FindChild ("Unit").gameObject.GetComponent<Unit>() as Champion;}
	public static Minion get_minion_script(GameObject obj){return obj.transform.FindChild ("Unit").gameObject.GetComponent<Unit>() as Minion;}
	//Unit accesseries
	public static Projectile get_projectile_script(GameObject obj){return obj.transform.FindChild("Projectile").gameObject.GetComponent<Projectile>();}
	//NavAgent

	//AI
	public static AI get_AI_script(GameObject obj){return obj.transform.FindChild("AIUnit").gameObject.GetComponent<AI>() as AI;}

	//get peer Auto Attack object
	public static GameObject get_AtkTarget_obj(GameObject child){return child.transform.parent.FindChild ("AttackRadius").gameObject;}

	//UI
	public static UI get_UI(GameObject obj){return obj.transform.FindChild("UI").gameObject.GetComponent<UI>();}
	public static GameObject get_UI_obj(GameObject obj){return obj.transform.FindChild("UI").gameObject;}

	//damage
	public static bool is_crit(int chance){//make the chance to be 100-based
		if (chance <= 0)
			return false;

		if (chance >= 100)
			return true;

		float result = Random.Range (1, 101);
		if(result <= chance)
			return true;

		return false;
	}

	//from LOL wiki
	public static float get_damage(float damage, float armor, float dmg_rdc){
		if (armor >= 0)
			return damage * (100f / (100f + armor)) * (1f-dmg_rdc);
		
		return damage * (2 - (100f / (100f - armor))) * (1f-dmg_rdc);
	}

	public static float get_magic_damage(float damage, float magic_resist, float dmg_rdc){
		if (magic_resist >= 0)
			return damage * (100f / (100f + magic_resist)) * (1f-dmg_rdc);

		return damage * (2 - (100f / (100f - magic_resist))) * (1f-dmg_rdc);
	}

	//get distance
	public static float get_dist_gameobject(GameObject obj1, GameObject obj2){return (obj1.transform.position - obj2.transform.position).magnitude;}
	public static float get_dist_transform(Transform obj1, Transform obj2){return (obj1.position - obj2.position).magnitude;}
	public static float get_dist_position(Vector3 obj1, Vector3 obj2){
		obj2.y = obj1.y;
		return (obj1 - obj2).magnitude;
	}

	//other
	public static int get_random_int(int lower, int upper){
		//what called here is the int version of Range() fuction
		return Random.Range(lower, upper);
	}

	public static bool LayerTargetable(GameObject obj){
		if ((obj.layer == GameSceneConsts.LAYER_DEAD) ||
		   (obj.layer == GameSceneConsts.LAYER_UNTARGETABLE))
			return false;

		return true;
	}

	public static bool TagTargetable(GameObject obj){
		if ((obj.tag == GameSceneConsts.TOWER_TAG) ||
			(obj.tag == GameSceneConsts.CHAMPION_TAG) ||
			(obj.tag == GameSceneConsts.MINION_TAG) ||
			(obj.tag == GameSceneConsts.ITEM_TAG) ||
			(obj.tag == GameSceneConsts.NETURAL_TAG) ||
			(obj.tag == GameSceneConsts.OBJECTIVE_TAG))
			return true;

		return false;
	}

	//the obj is a unit
	public static bool TagUnit(GameObject obj){
		if ((obj.tag == GameSceneConsts.CHAMPION_TAG) ||
			(obj.tag == GameSceneConsts.MINION_TAG) ||
			(obj.tag == GameSceneConsts.NETURAL_TAG))
			return true;

		return false;
	}

	//assume it's already targetable
	public static bool LifeStealable(GameObject obj){
		if ((obj.tag == GameSceneConsts.TOWER_TAG) ||
		    (obj.tag == GameSceneConsts.OBJECTIVE_TAG))
			return false;

		return true;
	}

	public static void setup_dmg_txt(GameObject dmg_indicator, int dmg, Color color){
		Text toSetup = dmg_indicator.transform.FindChild ("dmg_txt").FindChild("UI").FindChild("Text").gameObject.GetComponent<Text>();
		toSetup.text = dmg.ToString ();
		toSetup.color = color;
	}

	public static bool inside_camera_view(Vector3 pos){
		Vector3 screenPoint = GameSceneManager.instance.main_camera.WorldToViewportPoint(pos);
		bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		return onScreen;
	}

	public static void generate_dmg_txt(GameObject obj, int dmg, Color color){
		if (inside_camera_view (obj.transform.position)&&(dmg != 0)) {
			GameObject dmg_txt = Instantiate (GameSceneConsts.dmg_txt_prefab, obj.transform.position, Quaternion.identity) as GameObject;
			setup_dmg_txt (dmg_txt, dmg, color);
		}
	}

	public static int get_num_all_items(){
		return (Item.Item_name.END - Item.Item_name.START - 1);
	}

	public static int get_num_all_champions(){
		return (Champion.champion_name.END - Champion.champion_name.START - 1);
	}

	//check if a unit is at home(so that it can buy stuff)
	public static bool unit_at_home(){
		return true;
	}
}
