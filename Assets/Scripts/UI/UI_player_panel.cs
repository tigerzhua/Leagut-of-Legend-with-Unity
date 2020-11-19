using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_player_panel : MonoBehaviour {

	GameObject champ_panel;
	GameObject item_panel;
	GameObject shop_panel;
	CanvasScaler canvasScaler;
	public static UI_player_panel instance = null;
	//the host of this UI
	Player player;
	Champion playerChampion;

	float bar_moving_speed = 0.1f;
	//hp and mana
	Image hp_bar;
	Image hp_bar_bg;
	Image mana_bar;
	Image mana_bar_bg;
	//skills
	Image Q, W, E, R, P, D, F;
	Image Q_mask, W_mask, E_mask, R_mask, P_mask, D_mask, F_mask;
	//items
	Image[] items;
	Image[] items_mask;//0~5 for item 1~6, 6 for ward, 7 for recall
	Text gold_text;

	// Use this for initialization
	public void init(Player _player){
		instance = this;
		player = _player;//GameSceneManager.instance.players[GameSceneManager.instance.LoaclPlayerIdx].GetComponent<Player>();
		playerChampion = Utility.get_unit_script(player.gameObject) as Champion;

		champ_panel = transform.FindChild ("ChampPanel").gameObject;
		item_panel = transform.FindChild ("Items").gameObject;
		shop_panel = transform.FindChild ("Shop").gameObject;

		//assign all compnents
		hp_bar = champ_panel.transform.FindChild("hp_bar").GetComponent<Image>();
		hp_bar_bg = champ_panel.transform.FindChild("hp_bar_bg").GetComponent<Image>();
		mana_bar = champ_panel.transform.FindChild("mana_bar").GetComponent<Image>();
		mana_bar_bg = champ_panel.transform.FindChild("mana_bar_bg").GetComponent<Image>();

		P = champ_panel.transform.FindChild("P").GetComponent<Image>();
		P_mask = champ_panel.transform.FindChild("P_mask").GetComponent<Image>();
		Q = champ_panel.transform.FindChild("Q").GetComponent<Image>();
		Q_mask = champ_panel.transform.FindChild("Q_mask").GetComponent<Image>();
		W = champ_panel.transform.FindChild("W").GetComponent<Image>();
		W_mask = champ_panel.transform.FindChild("W_mask").GetComponent<Image>();
		E = champ_panel.transform.FindChild("E").GetComponent<Image>();
		E_mask = champ_panel.transform.FindChild("E_mask").GetComponent<Image>();
		R = champ_panel.transform.FindChild("R").GetComponent<Image>();
		R_mask = champ_panel.transform.FindChild("R_mask").GetComponent<Image>();
		D = champ_panel.transform.FindChild("D").GetComponent<Image>();
		D_mask = champ_panel.transform.FindChild("D_mask").GetComponent<Image>();
		F = champ_panel.transform.FindChild("F").GetComponent<Image>();
		F_mask = champ_panel.transform.FindChild("F_mask").GetComponent<Image>();

		items = new Image[GameSceneConsts.max_item + GameSceneConsts.max_supp_item];//6 items + ward
		items_mask = new Image[GameSceneConsts.max_item + GameSceneConsts.max_supp_item + 1];//+1 for recall

		for(int i = 0; i < items.Length; ++i)
			items [i] = item_panel.transform.FindChild (i.ToString ()).GetComponent<Image> ();

		for (int i = 0; i < items_mask.Length; ++i) {
			string name = i.ToString() + "_mask";
			items_mask [i] = item_panel.transform.FindChild (name).GetComponent<Image> ();
		}

		gold_text = item_panel.transform.FindChild ("shop").FindChild("Gold").GetComponent<Text>();
		canvasScaler = GetComponentInParent<CanvasScaler>();

		init_values ();
		init_shop ();

		close_shop_panel ();
	}

	public void close_shop_panel(){
		shop_panel.SetActive (false);
	}
		
	public void open_shop_panel(){
		shop_panel.SetActive (true);
	}

	public void toggle_shop_panel(){
		if (shop_panel.activeSelf)
			close_shop_panel ();
		else
			open_shop_panel ();
	}

	public void init_values(){
		hp_bar.fillAmount = playerChampion.runtime_u_stats.hp / playerChampion.runtime_u_stats.max_hp;
		hp_bar_bg.fillAmount = hp_bar.fillAmount;

		if (playerChampion.runtime_u_stats.max_mana == 0f)
			mana_bar.fillAmount = 0f;
		else
			mana_bar.fillAmount = playerChampion.runtime_u_stats.mana / playerChampion.runtime_u_stats.max_mana;

		mana_bar_bg.fillAmount = mana_bar.fillAmount;
		gold_text.text = ((int)player.gold).ToString ();

		for (int i = 0; i < items_mask.Length; ++i) {
			items_mask [i].fillAmount = 0f;
		}
	}

	public void init_shop(){
		GameObject itm_page = shop_panel.transform.FindChild ("ShopView").FindChild ("Panel").FindChild("items").gameObject;
		int total_itm = Utility.get_num_all_items ();
		const int num_colum = 4;
		Rect rect = itm_page.GetComponent<RectTransform> ().rect;
		RectTransform rectT = itm_page.GetComponent<RectTransform> ();
		//UnityEngine.Debug.DrawLine (fourcorners[0], fourcorners[1], Color.red);
		//UnityEngine.Debug.DrawLine (new Vector3(rect.xMin, rect.yMin, itm_page.transform.position.z), new Vector3(rect.xMax, rect.yMax, itm_page.transform.position.z), Color.red);
		float x_step = rect.width / ((float)(num_colum+1f)) * Screen.width / canvasScaler.referenceResolution.x;
		float y_step = rect.width / ((float)(num_colum+1f)) * Screen.height / canvasScaler.referenceResolution.y * 1.7f;
		float x_offset = (rectT.TransformPoint (rect.center).x) - 0.3f * rect.width * Screen.width / canvasScaler.referenceResolution.x;
		//draw the page
		int count = 0;
		float x_pos, y_pos, z_pos = itm_page.transform.position.z;
		y_pos = (rectT.TransformPoint (rect.center).y) + 0.5f * rect.height * Screen.height / canvasScaler.referenceResolution.y;
		for (int i = 1; i <= total_itm; ++i) {
			x_pos = count * x_step + x_offset;
			GameObject item = Instantiate (GameSceneConsts.shopItem_prefab, new Vector3(x_pos, y_pos, z_pos), Quaternion.identity) as GameObject;
			item.GetComponent<ShopItem> ().name = (Item.Item_name)i;
			int full_price = GameSceneConsts.item_prices [i];
			for(int j = 0; j < GameSceneConsts.item_comps[i].Length; ++j){
				full_price += GameSceneConsts.item_prices[ GameSceneConsts.item_comps [i] [j] ];
			}
			item.transform.FindChild ("price").gameObject.GetComponent<Text> ().text = full_price.ToString();
			item.transform.SetParent (itm_page.transform);
			item.transform.localScale = new Vector3 (1,1,1);
			//setup button
			Button button = item.GetComponent<Button>();
			button.onClick.AddListener (delegate{GameSceneManager.instance.get_local_champion().add_item(item.GetComponent<ShopItem> ().name);});
			count += 1;
			if (count == num_colum) {
				y_pos -= y_step;
				count = 0;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (player == null)
			return;

		//update hp bar, if necessary
		if (hp_bar_bg.fillAmount > hp_bar.fillAmount) {
			if (hp_bar_bg.fillAmount - bar_moving_speed * Time.deltaTime < hp_bar.fillAmount)
				hp_bar_bg.fillAmount = hp_bar.fillAmount;
			else
				hp_bar_bg.fillAmount -= bar_moving_speed * Time.deltaTime;
		} else if(hp_bar_bg.fillAmount < hp_bar.fillAmount){
			hp_bar_bg.fillAmount = hp_bar.fillAmount;
		}

		//update mana bar, if necessary
		if (mana_bar_bg.fillAmount > mana_bar.fillAmount) {
			if (mana_bar_bg.fillAmount - bar_moving_speed * Time.deltaTime < mana_bar.fillAmount)
				mana_bar_bg.fillAmount = mana_bar.fillAmount;
			else
				mana_bar_bg.fillAmount -= bar_moving_speed * Time.deltaTime;
		} else if(mana_bar_bg.fillAmount < mana_bar.fillAmount){
			mana_bar_bg.fillAmount = mana_bar.fillAmount;
		}
	}

	public void change_gold(int new_gold){
		gold_text.text = new_gold.ToString ();
	}

	public void change_hp(float newfill){
		hp_bar.fillAmount = newfill;
	}

	public void change_mana(float newfill){
		mana_bar.fillAmount = newfill;
	}

	public void refresh_champ_cd(){
		Q_mask.fillAmount = ((float)playerChampion.Q_timer.time_remaining / playerChampion.Q_timer.max_time);
		W_mask.fillAmount = ((float)playerChampion.W_timer.time_remaining / playerChampion.W_timer.max_time);
		E_mask.fillAmount = ((float)playerChampion.E_timer.time_remaining / playerChampion.E_timer.max_time);
		R_mask.fillAmount = ((float)playerChampion.R_timer.time_remaining / playerChampion.R_timer.max_time);
	}
}
