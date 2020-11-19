using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Minion : UI {

	public override void Awake(){
		base.Awake ();
		init ();
	}

	public override void init () {
		base.init ();
		//bar.fillAmount = unit.runtime_u_stats.hp / unit.runtime_u_stats.max_hp;
		bar.fillAmount = 1f;
		if (bar.fillAmount == 1f) {//dont show hp bar when full hp
			hide_UI ();
		} else {
			bar_bg.fillAmount = bar.fillAmount;
		}
	}
}
