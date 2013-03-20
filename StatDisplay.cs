using UnityEngine;
using System.Collections;

public class StatDisplay : MonoBehaviour {
	
	public Texture healthTex;
	public Texture energyTex;
	public Texture rescourceTex;
	
	private float health;
	private int healthForDisplay;
	private float healthBarLength;
	private int healthBarHeight = 15;
	private GUIStyle healthStyle = new GUIStyle();
	
	private float energy;
	private int energyForDisplay;
	private float energyBarLength;
	private int energyBarHeight = 15;
	private GUIStyle energyStyle = new GUIStyle();
	
	private float rescources;
	private int rescourcesForDisplay;
	private float rescourcesBarLength;
	private int rescourcesBarHeight = 15;
	private GUIStyle rescourcesStyle = new GUIStyle();
	
	private int boxW = 160;
	private int boxH = 85;
	private int labelH = 20;
	private int labelW = 35;
	private int padding = 10;
	private int gap = 120;
	private float commonLeft;
	private float commonTop;
	
	private HealthAndDamage HDScript;
	private PlayerEnergy energyScript;
	private PlayerResource rescourceScript;
	
	// Use this for initialization
	void Start () {
	if(networkView.isMine == true)
		{
			Transform triggerTransform = transform.FindChild("Trigger");
			HDScript = triggerTransform.GetComponent<HealthAndDamage>();
			
			
			energyScript = gameObject.GetComponent<PlayerEnergy>();
			rescourceScript = gameObject.GetComponent<PlayerResource>();
	
			healthStyle.normal.textColor = Color.red;
			healthStyle.fontStyle = FontStyle.Bold;
			energyStyle.normal.textColor = Color.cyan;
			energyStyle.fontStyle = FontStyle.Bold;
			rescourcesStyle.normal.textColor = Color.green;
			rescourcesStyle.fontStyle = FontStyle.Bold;
			
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		health = HDScript.myHealth;
		healthForDisplay = Mathf.CeilToInt (health);
		
		//How long should the bar be?
		//always 100% length max
		healthBarLength = (health/HDScript.maxHealth) * 100;
		
		energy = energyScript.energy;
		energyForDisplay = Mathf.CeilToInt(energy);
		
		energyBarLength = (energy/energyScript.baseEnergy) * 100;
		
		rescources = rescourceScript.resource;
		rescourcesForDisplay = Mathf.CeilToInt(rescources);
		
		rescourcesBarLength = (rescources/rescourceScript.baseResource) * 100;
	}
	
	void OnGUI()
	{
		commonLeft = Screen.width / 2 + 360;
		commonTop = Screen.height / 2 + 280;
		//Draw box behind health bar
		GUI.Box (new Rect(commonLeft, commonTop, boxW, boxH), "");
		//Draw grey box behind health bar
		GUI.Box (new Rect(commonLeft + padding, commonTop + padding, 100, healthBarHeight), "");
		//Draw health bar texture
		GUI.DrawTexture(new Rect(commonLeft + padding, commonTop + padding, healthBarLength, healthBarHeight), healthTex);
		GUI.Label(new Rect(commonLeft + gap, commonTop + padding, labelW, labelH), healthForDisplay.ToString(), healthStyle);
		
		//Draw grey box behind energy bar
		GUI.Box (new Rect(commonLeft + padding, commonTop + energyBarHeight + padding * 2, 100, energyBarHeight), "");
		//Draw energy bar texture
		GUI.DrawTexture(new Rect(commonLeft + padding, commonTop + energyBarHeight + padding  * 2, energyBarLength, energyBarHeight), energyTex);
		GUI.Label(new Rect(commonLeft + gap, commonTop + energyBarHeight + padding * 2, labelW, labelH), energyForDisplay.ToString(), energyStyle);
		
		//Draw grey box behind energy bar
		GUI.Box (new Rect(commonLeft + padding, commonTop + rescourcesBarHeight * 2 + padding * 3, 100, rescourcesBarHeight), "");
		//Draw energy bar texture
		GUI.DrawTexture(new Rect(commonLeft + padding, commonTop + rescourcesBarHeight * 2 + padding  * 3, rescourcesBarLength, rescourcesBarHeight), rescourceTex);
		GUI.Label(new Rect(commonLeft + gap, commonTop + rescourcesBarHeight * 2 + padding * 3, labelW, labelH), rescourcesForDisplay.ToString(), rescourcesStyle);
	}
	
}
