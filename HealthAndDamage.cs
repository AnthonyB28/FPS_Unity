using UnityEngine;
using System.Collections;

/// <summary>
/// Health and damage manages the health of the player across the network.
/// Attached to Trigger GameObject on each player.
/// </summary>

public class HealthAndDamage : MonoBehaviour {
	
	private GameObject parentObject;
	
	public string myAttacker;
	public bool iWasAttacked;
	
	//Figure out player was hit by
	public bool hitByBlaster = false;
	public float blasterD = 18;
	
	public bool hitByParticleCannon = false;
	public float particleD = 1.5f;
	
	private bool destroyed = false;
	
	//Player Health
	public float myHealth = 100;
	public float maxHealth = 100;
	private float healthRegen = 1.3f;
	public float previousHealth = 100;
	
	public bool takingFallDamage = false;
	public float fallDamage;
	
	// Use this for initialization
	void Start () {
	parentObject = transform.parent.gameObject;
		
		if(networkView.isMine == true)
		{
			//Access playerStats
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerStats statScript = gameManager.GetComponent<PlayerStats>();
			maxHealth = statScript.maxHealth;
			myHealth = maxHealth;
			networkView.RPC("UpdateMyMaxHealthEverywhere", RPCMode.OthersBuffered, maxHealth);
			networkView.RPC ("UpdateMyCurrentHealthEverywhere", RPCMode.Others, myHealth);
			networkView.RPC ("UpdateMyHealthRecordEverywhere", RPCMode.AllBuffered, myHealth);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(iWasAttacked == true)
		{
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
			
			//Find player, hit if attacker is running this
			for(int i = 0; i < dataScript.PlayerList.Count; i++)
			{
				if(myAttacker == dataScript.PlayerList[i].playerName)
				{
					//if attacker is the same, then carry out attack
					if(int.Parse(Network.player.ToString ()) == dataScript.PlayerList[i].networkPlayer)
					{
						if(hitByBlaster == true && destroyed == false)
						{
							myHealth = myHealth - blasterD;
							networkView.RPC ("UpdateMyCurrentAttackerEverywhere",
											RPCMode.Others, myAttacker);
							
							//Send out RPC to update target's health
							networkView.RPC ("UpdateMyCurrentHealthEverywhere",
											RPCMode.Others, myHealth);
						
							hitByBlaster = false;
						}
						
			//Fall Damage
			if(takingFallDamage == true && destroyed == false )
			{
				myHealth = myHealth - fallDamage;
				myAttacker = "suicide";
				networkView.RPC ("UpdateMyCurrentAttackerEverywhere",RPCMode.Others, myAttacker);
				networkView.RPC ("UpdateMyCurrentHealthEverywhere",
											RPCMode.Others, myHealth);
				takingFallDamage = false;
				}
						
			//Apply damage if the player is hit by an enemy particle cannon
			if(hitByParticleCannon == true && destroyed == false)
			{
				myHealth = myHealth - particleD;
				networkView.RPC ("UpdateMyCurrentAttackerEverywhere",RPCMode.Others, myAttacker);
				networkView.RPC ("UpdateMyCurrentHealthEverywhere",
											RPCMode.Others, myHealth);
				hitByParticleCannon = false;
			}
			
			//Once player is dead, destroyed is set to true and the attacker gets score.
			if(myHealth <= 0 && destroyed == false)
			{
				myHealth = 0;
				destroyed = true;
							
				//The attacking player should be the only one getting a score, no attacking player then forget it
				if(myAttacker != "suicided")
				{
				GameObject attacker = GameObject.Find (myAttacker);
				PlayerScore scoreScript = attacker.GetComponent<PlayerScore>();
				scoreScript.iDestroyedEnemy = true;
				scoreScript.enemiesDestroyedInOneHit++;			
				}
				
			}
					}
				}
			}
			iWasAttacked = false;
		}//END OF if iWasAttacked
		
		
		
		//Each player has their own destruction
		if(myHealth <= 0 && networkView.isMine == true)
		{
			//Access spawn script set the destroyed to true
			GameObject spawnManager = GameObject.Find ("SpawnManager");
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			spawnScript.destroyed = true;
			
			//Remove Player RPC
			Network.RemoveRPCs(Network.player);
			
			//Update CombatWindow
			networkView.RPC("TellEveryoneCombat",RPCMode.All,myAttacker,parentObject.name);
			
			//Tell everyone else
			networkView.RPC ("DestroySelf", RPCMode.All);
		}
		
		if(myHealth > 0 && networkView.isMine == true)
		{
			if(myHealth != previousHealth)
			{
				networkView.RPC("UpdateMyHealthRecordEverywhere", RPCMode.AllBuffered, myHealth);
			}
		}
		
		if(myHealth < maxHealth)
		{
			myHealth = myHealth + healthRegen * Time.deltaTime;
		}
		
		if(myHealth > maxHealth)
		{
			myHealth = maxHealth;
		}
		
	}


	[RPC]
		void UpdateMyCurrentAttackerEverywhere(string attacker)
	{
		myAttacker = attacker;
	}
	[RPC]
		void UpdateMyCurrentHealthEverywhere(float health)
	{
		myHealth = health;
	}
	[RPC]
		void DestroySelf()
	{
		Destroy (parentObject);
	}
	[RPC]
		void UpdateMyHealthRecordEverywhere(float health)
	{
		previousHealth = health;
		
	}
	[RPC]
		void TellEveryoneCombat (string attacker, string destroyed)
	{
		GameObject gameManager = GameObject.Find ("GameManager");
		CombatWindow combatScript = gameManager.GetComponent<CombatWindow>();
		combatScript.attackerName = attacker;
		combatScript.destroyedName = destroyed;
		combatScript.addNewEntry = true;
	}
	
	[RPC]
	void UpdateMyMaxHealthEverywhere(float health)
	{
		maxHealth = health;
	}

}

