using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	
	public float energy;
	public float baseEnergy = 100;
	private float rechargeRate = 20;

	// Use this for initialization
	void Start () {
		if(networkView.isMine == true)
		{
			//Access playerStat
			GameObject gameManager = GameObject.Find ("GameManager");
		
		PlayerStats statScript = gameManager.GetComponent<PlayerStats>();
			baseEnergy = statScript.maxEnergy;
			energy = baseEnergy;
		}
		else {enabled=false;}
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//player's energy falls below base, recharge
		if(energy < baseEnergy)
		{
			energy = energy + rechargeRate * Time.deltaTime;
			
		}
		
		//Prevent the player's energy from exceeding the baseEnergy or fall below 0
		if(energy > baseEnergy)
		{
			energy = baseEnergy;
		}
		
		if(energy < 0)
		{
			energy = 0;
		}
	}
}
