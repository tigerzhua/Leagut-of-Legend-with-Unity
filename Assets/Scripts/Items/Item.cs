using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public struct Item_flags{
	}

	public enum Item_name{
		START,//mark of begin
		LONG_SWORD,
		PICKAXE,
		LAST_WHISPER,
		INFINITY_EDGE,
		YOUMUUS_GHOSTBLADE,
		BOOTS_OF_SPEED,
		END,//mark of end
		DUMMY,//Dummy
	}

	int cost;
	public Types.damage_combo Onhit_dmg;//on hit
	public Unit.unit_stats stats_add_base;//add stats to base stats
	public Unit.unit_stats stats_mult_base;//add stats to base stats
	public Unit.unit_stats stats_add;//if add ad stats, should be put here
	public Unit.unit_stats stats_mult;
	public Item_name ItemName;
	Item_name[] comps;//components

	public void init(Item_name _name){
		cost = 0;//cost of itself, not incluing components
		comps = new Item_name[5];//max 5 components
		Onhit_dmg.init ();
		stats_add.init ();
		stats_mult.init ();
		stats_add_base.init ();
		stats_mult_base.init ();
		ItemName = _name;
		load_item_stats (_name);
	}

	//This is definitely not the best way of doing things....
	void load_item_stats(Item_name _name){
		switch(_name){
		case Item_name.LONG_SWORD:
			cost = 350;
			stats_add.damage = 10f;
			break;
		case Item_name.PICKAXE:
			cost = 875;
			stats_add.damage = 25;
			break;
		case Item_name.LAST_WHISPER:
			cost = 425;
			stats_add.damage = 25f;
			stats_mult.armor_pen_percent = 0.3f;
			break;
		case Item_name.YOUMUUS_GHOSTBLADE:
			cost = 1000;
			stats_add.damage = 60f;
			stats_add.crd = 0.1f;
			break;
		default:
			break;
		}
	}

	public int get_cost(){return cost;}
}
