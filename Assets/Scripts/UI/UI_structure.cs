using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_structure : UI {

	Image bg_black;
	Image bg_gray;

	public override void Awake(){
		base.Awake ();
		init ();
	}

	// Use this for initialization
	public override void init () {
		base.init ();
		bg_black = transform.FindChild ("bar_black").gameObject.GetComponent<Image>();
		bg_gray = transform.FindChild ("bar_gray").gameObject.GetComponent<Image> ();

		//set up init values
		//bar.fillAmount = unit.runtime_u_stats.hp / unit.runtime_u_stats.max_hp;
		bar.fillAmount = 1f;
		if (bar.fillAmount == 1f) {//dont show hp bar when full hp
			hide_UI ();
		} else {
			unhide_UI ();
			bar_bg.fillAmount = bar.fillAmount;
		}
	}

	public override void unhide_UI(){
		base.unhide_UI ();
		bg_black.fillAmount = 1f;
		bg_gray.fillAmount = 1f;
	}

	public override void hide_UI(){
		base.hide_UI ();
		bg_black.fillAmount = 0f;
		bg_gray.fillAmount = 0f;
	}
}
