using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Champion : UI {

	Image bar_mana;
	GameObject lines;//parent of all lines
	Text level_txt;

	public Champion champ;
	// Use this for initialization
	public override void init () {
		base.init ();
		bar_mana = transform.FindChild ("bar_mana").gameObject.GetComponent<Image> ();
		lines = transform.FindChild ("lines").gameObject;
		level_txt = transform.FindChild ("lv_txt").gameObject.GetComponent<Text>();
		champ = unit as Champion;

		//set up init values
		bar.fillAmount = champ.runtime_u_stats.hp / champ.runtime_u_stats.max_hp;
		if (champ.runtime_u_stats.max_mana > 0)
			bar_mana.fillAmount = champ.runtime_u_stats.mana / champ.runtime_u_stats.max_mana;
		else
			bar_mana.fillAmount = 0f;
		bar_bg.fillAmount = bar.fillAmount;
		level_txt.text = champ.runtime_c_stats.level.ToString();

		draw_lines();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (bar_bg.fillAmount > bar.fillAmount) {
			if (bar_bg.fillAmount - bar_moving_speed * Time.deltaTime < bar.fillAmount)
				bar_bg.fillAmount = bar.fillAmount;
			else
				bar_bg.fillAmount -= bar_moving_speed * Time.deltaTime;
		}
	}

	public override void update_bar_fillamount(float hp_fill){
		bar.fillAmount = hp_fill;
	}

	public void update_level(){
		level_txt.text = champ.runtime_c_stats.level.ToString();
		update_hp ();
	}

	public void update_bar_mana_fillamount(float mana_fill){
		bar_mana.fillAmount = mana_fill;
	}

	public void update_hp(){
		draw_lines ();
	}

	//draw the black lines on the hp bar
	void draw_lines(){
		int num_hurd = (int)Mathf.Floor(champ.runtime_u_stats.max_hp / 100f);
		int num_thod = (int)Mathf.Floor(champ.runtime_u_stats.max_hp / 1000f);

		//clear existing lines
		foreach(Transform child in lines.transform) {
			Destroy(child.gameObject);
		}

		//create new lines
		float len = lines.GetComponent<RectTransform>().rect.width;
		float half_len = len * 0.5f;
		float hgt = lines.GetComponent<RectTransform> ().rect.height;

		float interval_h = 100f*(len / champ.runtime_u_stats.max_hp);
		float interval_t = 1000f*(len / champ.runtime_u_stats.max_hp);

		Vector3 parent_pos = lines.transform.position;
		Vector3 offset_pos;
		//draw hundred lines first
		for (int i = 1; i <= num_hurd; ++i) {
			if (i % 10 == 0)
				continue;
			
			offset_pos = new Vector3 (interval_h*i-half_len, 0.022f,0);
			GameObject h_line = Instantiate (GameSceneConsts.hundred_line_prefab, parent_pos+offset_pos, lines.transform.rotation) as GameObject;
			//h_line.GetComponent<RectTransform>().
			h_line.transform.SetParent (lines.transform);
		}

		//draw hundred lines first
		for (int i = 1; i <= num_thod; ++i) {
			offset_pos = new Vector3 (interval_t*i-half_len, 0,0);
			GameObject t_line = Instantiate (GameSceneConsts.thousand_line_prefab, parent_pos+offset_pos, lines.transform.rotation) as GameObject;
			t_line.transform.SetParent (lines.transform);
		}
	}
}
