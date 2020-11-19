using UnityEngine;
using System.Collections;

public class Types : MonoBehaviour {
	public struct display_coeff{
		public bool follow_camera;
		public float camera_speed;
		public float camera_move_threshold;

		public void init(){
			camera_speed = 1200 * GameSceneConsts.dist_multiplier;
			camera_move_threshold = 50.0f;
			follow_camera = false;
		}
	}

	public enum Faction{
		RED,
		BLUE,
		NUTURAL,
		OTHER
	}

	public enum Lanes{
		TOP,
		MID,
		BOT,
		JG,
		OTHER,
	}

    public enum Skill_Slot{
        Q,
        W,
        E,
        R,
        P,
    }

	public struct damage_combo{
		public float physical_dmg;
		public float armor_pen_percent;
		public float armor_pen;
		public float magic_dmg;
		public float magic_pen_percent;
		public float magic_pen;
		public float true_dmg;
		public float pure_dmg;

		public void init(){
			physical_dmg = 0f;
			armor_pen_percent = 0f;
			armor_pen = 0f;
			magic_dmg = 0f;
			magic_pen_percent = 0f;
			magic_pen = 0f;
			true_dmg = 0f;
			pure_dmg = 0f;
		}

		public static damage_combo operator * (float l, damage_combo r){
			damage_combo result = new damage_combo();
			result.physical_dmg = r.physical_dmg * l;
			result.armor_pen_percent = r.armor_pen_percent * l;
			result.armor_pen = r.armor_pen * l;
			result.magic_dmg = r.magic_dmg * l;
			result.magic_pen_percent = r.magic_pen_percent * l;
			result.magic_pen = r.magic_pen * l;
			result.true_dmg = r.true_dmg * l;
			result.pure_dmg = r.pure_dmg * l;	
			return result;
		}

		public static damage_combo operator + (damage_combo l, damage_combo r){
			l.physical_dmg += r.physical_dmg;
			l.armor_pen_percent += r.armor_pen_percent;
			l.armor_pen += r.armor_pen;
			l.magic_dmg += r.magic_dmg;
			l.magic_pen_percent += r.magic_pen_percent;
			l.magic_pen += r.magic_pen;
			l.true_dmg += r.true_dmg;
			l.pure_dmg += r.pure_dmg;	
			return l;
		}

		//public static damage_combo zero{ get;}
	}
}
