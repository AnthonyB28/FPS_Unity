using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// This script is attached to the GameManager.
/// 
/// This script is accessed by the HealthAndDamage script to
/// check what the player's max health is.
/// 
/// This script is accessed by the PlayerEnergy script to 
/// check what the player's max energy is.
/// 
/// This script is accessed by the PlayerResource script to
/// check what the player's max resource is.
/// 
/// This script is accessed by the Crouching script to determine
/// what the player's normal movement speed is.
/// 
/// This script is accessed by the SpawnScript so that it can draw
/// the JoinTeamWindow such that the player can choose their attributes.
/// 
/// </summary>


public class PlayerStats : MonoBehaviour {
	
	//Variables Start_________________________________________________________
		
	//Used for GUI controls.
		
	private int labelHeight = 40;
	
	private int labelWidth = 100;
	
	private int buttonWidth = 40;
	
	
	//Used in setting the player attributes
	
	private float healthMultiplier = 10;
	
	private float energyMultiplier = 10;
	
	private float resourceMultiplier = 20;
	
	private float speedMultiplier = 1;
	
	
	private float TotalStatPoints = 37;
	
	private float statPointsLeft;
	
	
	private float healthStat;
	
	private float energyStat;
	
	private float resourceStat;
	
	private float speedStat;
	
	
	private float maxHealthStat = 20;
	
	private float maxEnergyStat = 20;
	
	private float maxResourceStat = 20;
	
	private float maxSpeedStat = 5;
	
	
	private float minHealthStat = 1;
	
	private float minEnergyStat = 1;
	
	private float minResourceStat = 1;
	
	private float minSpeedStat = 3;
	
	
	private float defaultHealthStat = 10;
	
	private float defaultEnergyStat = 10;
	
	private float defaultResourceStat = 5;
	
	private float defaultSpeed = 4;
	
	
	public float maxHealth;
	
	public float maxEnergy;
	
	public float maxResource;
	
	public float maxSpeed;
	
	
	//Used for GUIStyles.
	
	private GUIStyle healthStyle = new GUIStyle();
	
	private GUIStyle energyStyle = new GUIStyle();
	
	private GUIStyle resourceStyle = new GUIStyle();
	
	private GUIStyle speedStyle = new GUIStyle();
	
	private GUIStyle plainStyle = new GUIStyle();
	
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		//Restoring the attribute setup the player last used.
		//If they are playing for the first time then use some
		//default values as the player prefs will not have been
		//set yet.
		
		healthStat = PlayerPrefs.GetFloat("healthStat");
		
		if(healthStat == 0)
		{
			healthStat = defaultHealthStat;
		}
		
		energyStat = PlayerPrefs.GetFloat("energyStat");
		
		if(energyStat == 0)
		{
			energyStat = defaultEnergyStat;	
		}
		
		resourceStat = PlayerPrefs.GetFloat("resourceStat");
		
		if(resourceStat == 0)
		{
			resourceStat = defaultResourceStat;	
		}
		
		speedStat = PlayerPrefs.GetFloat("speedStat");
		
		if(speedStat == 0)
		{
			speedStat = defaultSpeed;
		}
		
		
		//Setting the font for the labels that will display stats.
		
		plainStyle.alignment = TextAnchor.MiddleLeft;
				
		plainStyle.normal.textColor = Color.white;
		
		
		healthStyle.normal.textColor = Color.green;
		
		healthStyle.alignment = TextAnchor.MiddleLeft;

		
		energyStyle.normal.textColor = Color.cyan;
		
		energyStyle.alignment = TextAnchor.MiddleLeft;
		
		
		resourceStyle.normal.textColor = Color.magenta;
		
		resourceStyle.alignment = TextAnchor.MiddleLeft;
		
		
		speedStyle.normal.textColor = Color.yellow;
		
		speedStyle.alignment = TextAnchor.MiddleLeft;
	}
	
	
	//This function is called by the SpawnScript and is used in the 
	//JoinTeamWindow function to draw the window such that the player
	//can choose their attributes.
	
	public void Stats()
	{
		//The max values will be the ones used to define the player.
		
		maxHealth = healthStat * healthMultiplier;
		
		maxEnergy = energyStat * energyMultiplier;
		
		maxResource = resourceStat * resourceMultiplier;
		
		maxSpeed = speedStat * speedMultiplier;
		
		statPointsLeft = TotalStatPoints - healthStat - energyStat - resourceStat - speedStat;
		
		
		//Ensure that no stat can be greater than the limit set
		//for it.
		
		if(healthStat > maxHealthStat)
		{
			healthStat = maxHealthStat;	
		}
		
		if(energyStat > maxEnergyStat)
		{
			energyStat = maxEnergyStat;	
		}
		
		if(resourceStat > maxResourceStat)
		{
			resourceStat = maxResourceStat;	
		}
		
		if(speedStat > maxSpeedStat)
		{
			speedStat = maxSpeedStat;	
		}
		
		
		//Reset stats if the total number of stat points is greater
		//than the limit.
		
		if((healthStat + energyStat + resourceStat + speedStat) > TotalStatPoints)
		{
			healthStat = defaultHealthStat;
			
			energyStat = defaultEnergyStat;
			
			resourceStat = defaultResourceStat;
			
			speedStat = defaultSpeed;
		}
		
		
		GUILayout.Space(10);
		
		
		GUILayout.BeginHorizontal("box");
		
		GUILayout.Label("Attribute points not allocated", plainStyle);
		
		GUILayout.Label(statPointsLeft.ToString(), plainStyle);
			
		GUILayout.EndHorizontal();	
		
		
		//GUI setup for health attribute
		
		GUILayout.BeginHorizontal("box");
		
		GUILayout.Label("Health", healthStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
		
		GUILayout.Label(maxHealth.ToString(), healthStyle, GUILayout.Width(buttonWidth), GUILayout.Height(labelHeight));
		
		if(GUILayout.Button("+", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(healthStat < maxHealthStat && statPointsLeft > 0)
			{
				healthStat++;
				
				PlayerPrefs.SetFloat("healthStat", healthStat);
			}
		}
		
		if(GUILayout.Button("-", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(healthStat > minHealthStat)
			{
				healthStat--;
				
				PlayerPrefs.SetFloat("healthStat", healthStat);
			}
		}	
		
		GUILayout.Label("max " + (maxHealthStat * healthMultiplier).ToString(), healthStyle, GUILayout.Width(60), GUILayout.Height(labelHeight));
		
		GUILayout.EndHorizontal();
		
		
		//GUI setup for energy attribute
		
		GUILayout.BeginHorizontal("box");
		
		GUILayout.Label("Energy", energyStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
		
		GUILayout.Label(maxEnergy.ToString(), energyStyle, GUILayout.Width(buttonWidth), GUILayout.Height(labelHeight));
		
		if(GUILayout.Button("+", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(energyStat < maxEnergyStat && statPointsLeft > 0)
			{
				energyStat++;	
				
				PlayerPrefs.SetFloat("energyStat", energyStat);
			}
		}
		
		if(GUILayout.Button("-", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(energyStat > minEnergyStat)
			{
				energyStat--;	
				
				PlayerPrefs.SetFloat("energyStat", energyStat);
			}
		}
		
		GUILayout.Label("max " + (maxEnergyStat * energyMultiplier).ToString(), energyStyle, GUILayout.Width(60), GUILayout.Height(labelHeight));
		
		GUILayout.EndHorizontal();
		
		
		//GUI setup for resource attribute
		
		GUILayout.BeginHorizontal("box");
		
		GUILayout.Label("Resource", resourceStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
		
		GUILayout.Label(maxResource.ToString(), resourceStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth));
		
		if(GUILayout.Button("+", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(resourceStat < maxResourceStat && statPointsLeft > 0)
			{
				resourceStat++;
				
				PlayerPrefs.SetFloat("resourceStat", resourceStat);
			}
		}
		
		if(GUILayout.Button("-", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(resourceStat > minResourceStat)
			{
				resourceStat--;	
				
				PlayerPrefs.SetFloat("resourceStat", resourceStat);
			}
		}
		
		GUILayout.Label("max " + (maxResourceStat * resourceMultiplier).ToString(), resourceStyle, GUILayout.Width(60), GUILayout.Height(labelHeight));
		
		GUILayout.EndHorizontal();
		
		
		//GUI setup for speed attribute
		
		GUILayout.BeginHorizontal("box");
		
		GUILayout.Label("Speed", speedStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
		
		GUILayout.Label(maxSpeed.ToString(), speedStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth));
		
		if(GUILayout.Button("+", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(speedStat < maxSpeedStat && statPointsLeft > 0)
			{
				speedStat++;
				
				PlayerPrefs.SetFloat("speedStat", speedStat);
			}
		}
		
		if(GUILayout.Button("-", GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)))
		{
			if(speedStat > minSpeedStat)
			{
				speedStat--;	
				
				PlayerPrefs.SetFloat("speedStat", speedStat);
			}
		}
		
		GUILayout.Label("max " + (maxSpeedStat * speedMultiplier).ToString(), speedStyle, GUILayout.Width(60), GUILayout.Height(labelHeight));
		
		GUILayout.EndHorizontal();
		
		
		GUILayout.Space(10);
	}
	
}
