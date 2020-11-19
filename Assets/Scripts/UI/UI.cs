using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

	protected Image bar;
	protected Image bar_bg;
	public Unit unit;
	protected float bar_moving_speed = 0.1f;
	bool hide;

	public virtual void Awake(){
		bar = transform.FindChild ("bar").gameObject.GetComponent<Image>();
		bar_bg = transform.FindChild ("bar_bg").gameObject.GetComponent<Image> ();
		//link itself to its host
		unit = Utility.get_unit_script (transform.parent.gameObject);		
	}

	// Use this for initialization
	public virtual void init () {
		bar = transform.FindChild ("bar").gameObject.GetComponent<Image>();
		bar_bg = transform.FindChild ("bar_bg").gameObject.GetComponent<Image> ();
		//link itself to its host
		unit = Utility.get_unit_script (transform.parent.gameObject);
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (bar_bg.fillAmount > bar.fillAmount) {
			if (bar_bg.fillAmount - bar_moving_speed * Time.deltaTime < bar.fillAmount)
				bar_bg.fillAmount = bar.fillAmount;
			else
				bar_bg.fillAmount -= bar_moving_speed * Time.deltaTime;
		}
	}

	public virtual void update_bar_fillamount(float hp_fill){
		if (!hide)
			bar.fillAmount = hp_fill;

		if ((hp_fill != 1f) && (hide))
			unhide_UI ();
	}

	public virtual void hide_UI(){
		hide = true;
		bar.fillAmount = 0f;
		bar_bg.fillAmount = bar.fillAmount;
	}

	public virtual void unhide_UI(){
		hide = false;
		bar_bg.fillAmount = 1f;
	}

	public virtual void refill(){
		bar.fillAmount = 1f;
		bar_bg.fillAmount = bar.fillAmount;
	}
}
