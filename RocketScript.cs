using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is attached to Rocket projectiles. It causes
/// the rocket to fly forward and if it hits something it carries
/// out a hit detection process. The hit detection process only 
/// happens on the game instance of the player who launched the
/// rocket and if blocks were hit then an RPC is sent to server.
/// 
/// This script accesses the HealthAndDamge script of struck players.
/// 
/// This script accesses the "BlockAsksToRemoveItself scripts for 
/// all types of blocks and this is done by sending an RPC to the 
/// server.
/// 
/// This script is accessed by the FireRocket script which supplies
/// the name of the player that launched this rocket.
/// </summary>

public class RocketScript : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	
	public GameObject rocketExplosion;
		
	public string team;
	
	
	//Cached variables.
	
	private Transform myTransform;
	
	private bool expended = false;
	
	private PlayerDatabase playerData;
	
	
	//The rocket will destroy itself after this time
		
	private float expireTime = 12f;
	
	
	//The speed the rocket will translate at.
	
	private float rocketSpeed = 20.0f;
	
	
	//The player object that instantiated this rocket.
	//This is filled in by the FireRocket script.
	
	public string myOriginator;
	
	
	//These variables are used for the rayacast.
	
	private RaycastHit hit;
	
	private RaycastHit rocketHit;

	
	private float range = 1.0f;
	
	private float blastRadius = 3;
		
	private float blastRocketDamage = 80;
	
	private float damageDelivered;
	
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		
		
		GameObject gameManager = GameObject.Find("GameManager");
		
		playerData = gameManager.GetComponent<PlayerDatabase>();
		
		
		StartCoroutine(DestroyMySelfAfterSomeTime());
		rigidbody.AddForce(myTransform.up * 1100);
	}
	
	void FixedUpdate()
	{
		//Physics
		rigidbody.AddForce (myTransform.up* rocketSpeed);
		if(rigidbody.velocity != Vector3.zero)
		{
			myTransform.rotation = Quaternion.LookRotation(rigidbody.velocity);
			myTransform.rotation = Quaternion.Euler (myTransform.eulerAngles.x + 90, myTransform.eulerAngles.y, 0);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//The rocket is translated with Vector3.up becuase that
		//is its local orientation. It was made pointing upwards.
		
		//myTransform.Translate(Vector3.up * rocketSpeed * Time.deltaTime);
		
	
		
		
		if(Physics.Raycast( myTransform.position, myTransform.up, out rocketHit, range) && expended == false)
		{
			expended = true;
			
			
			//The hit detection portion of this script is to only run on the game instance
			//of the player who launched this rocket. This is so that RPCs can be used cleanly
			//to ensure good hit detection. Go through the PlayerList to find out if this script
			//is running on the computer of the player who fired the rocket as only they
			//should carry out the hit detection process.
			
			for(int i = 0; i < playerData.PlayerList.Count; i++)
			{
				//Find the player be name in the list.
				
				if(playerData.PlayerList[i].playerName == myOriginator)
				{	
					//Now check if the Network ID for this player is the same as that of the player
					//who is running this script in their game instance. If they are the same then
					//this script is running in the game of the player who launched the rocket.
					
					if(int.Parse(Network.player.ToString()) == playerData.PlayerList[i].networkPlayer)
					{
						//Check what was hit right in front of the rocket.
						
						WhatWasHitByOverlapSphere(rocketHit.transform.collider, rocketHit.point);	
						
						
						//Capture all colliders struck in a radius around the rocket.
			
						List<Collider> struckObjects = new List<Collider>(Physics.OverlapSphere(rocketHit.point, blastRadius));
						
						foreach(Collider objectsHit in struckObjects)
						{
							WhatWasHitByOverlapSphere(objectsHit, rocketHit.point);
						}
					}
				}
			}
			
			//Instantiate an explosion at the point where the rocket hits.
			
			Instantiate(rocketExplosion, rocketHit.point, Quaternion.identity);
			
			
			//Rather than destroying the rocket it is important to disable it visually because
			//it needs to send the RPCs to the server. The rocket will be destroyed in time anyway
			//due to the coroutine used in the Start function.
			
			myTransform.GetComponentInChildren<TrailRenderer>().enabled = false;
			
			
			//The tube and fins are child objects so we setup a list to capture the
			//renderers for each of them and then we go through the list and disable
			//their renderers.
			
			Renderer[] rocketRenderers = myTransform.GetComponentsInChildren<Renderer>();
			
			foreach (Renderer x in rocketRenderers)
			{
				x.enabled = false;	
			}
			
		}
	}
	
	
	
	//This function is used by the overlap sphere hit detection check.
	
	void WhatWasHitByOverlapSphere (Collider obj, Vector3 explosionPos)
	{
		if(obj.tag == "RedTeamTrigger" && team == "blue")
		{	
			//A linecast is sent out to check if there is no obstruction between
			//the enemy player and the source of the explosion. The enemy is only
			//hit if there is no obstruction. Players on the same team are not counted
			//as an obstruction and all of them in range will be hit.
								
			if(Physics.Linecast(myTransform.position, obj.transform.position, out hit))
			{
				if(hit.transform.tag == "RedTeamTrigger")
				{
					GameObject enemy = (GameObject) obj.transform.gameObject;
					
					
					//Calculate how much damage to deliver based on far the hit point
					//of the rocket is from the struck player.
					
					damageDelivered = blastRocketDamage / 
						(Vector3.Distance(explosionPos, obj.transform.position) / blastRadius);
					
					
					//Access the HealthAndDamage script and apply damage to the
					//struck player.
					
					HealthAndDamage script = enemy.GetComponent<HealthAndDamage>();
					
					script.myAttacker = myOriginator;
						
					script.iWasAttacked = true;
					
					script.hitByRocket = true;	
					
					script.rocketD = damageDelivered;
				}
			}
		}
		
		
		if(obj.tag == "BlueTeamTrigger" && team == "red")
		{	
			//A linecast is sent out to check if there is no obstruction between
			//the enemy player and the source of the explosion. The enemy is only
			//hit if there is no obstruction. Players on the same team are not counted
			//as an obstruction and all of them in range will be hit.
			
			if(Physics.Linecast(myTransform.position, obj.transform.position, out hit))
			{					
				if(hit.transform.tag == "BlueTeamTrigger")
				{
					GameObject enemy = (GameObject) obj.transform.gameObject;
					
					
					//Calculate how much damage to deliver based on far the hit point
					//of the rocket is from the struck player.
					
					damageDelivered = blastRocketDamage / 
						(Vector3.Distance(explosionPos, obj.transform.position) / blastRadius);
				
					
					//Access the HealthAndDamage script and apply damage to the
					//struck player.
					
					HealthAndDamage script = enemy.GetComponent<HealthAndDamage>();
					
					script.myAttacker = myOriginator;
						
					script.iWasAttacked = true;
					
					script.hitByRocket = true;	
					
					script.rocketD = damageDelivered;
				}
			}
		}
		
		
		
		//If a block was hit then send an RPC to the server to have the block destroyed.
		//The use of RPCs is very important and ensures that we get solid hit detection.
		
		if(obj.tag == "ConstructionBlock")
		{				
			networkView.RPC("RocketTellServerConstructionBlockIsHit", RPCMode.Server, obj.name);
		}
				
		
		//if(obj.tag == "AirBlock")
		//{				
		//	networkView.RPC("RocketTellServerAirBlockIsHit", RPCMode.Server, obj.name);	
		//}	
	}
	
	
	
	
	
	//Used to destroy the rocket after some time if
	//it hasn't hit anything so that it doesn't keep flying
	//on forever.
		
	IEnumerator DestroyMySelfAfterSomeTime() 
	{
		yield return new WaitForSeconds(expireTime);	
		
		Instantiate(rocketExplosion, myTransform.position, Quaternion.identity);
		
		Destroy(gameObject);
	}
	
	
	[RPC]
	void RocketTellServerConstructionBlockIsHit (string blockName)
	{
		GameObject block = GameObject.Find(blockName);
		
		ConstructionBlockAsksToRemoveItself script = block.transform.GetComponent<ConstructionBlockAsksToRemoveItself>();
				
		script.iAmHit = true;
	}
	
	
	//[RPC]
	//void RocketTellServerAirBlockIsHit (string blockName)
	//{
	//	GameObject block = GameObject.Find(blockName);	
		
	//	AirBlockAsksToRemoveItself script = block.transform.GetComponent<AirBlockAsksToRemoveItself>();
				
	//	script.iAmHit = true;
	//}
}
