using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the Player and allows them
/// to fire rockets.
/// 
/// This script accesses the PlayerResource script on this player
/// to check their resource level and to deduct resource.
/// 
/// This script accesses the ChangeWeapon script on this player
/// to check if the currently selected weapon is the Rocket.
/// 
/// This script accesses the SpawnScript to determine which
/// team this player is on and ultimately which team the rocket should
/// belong to.
/// </summary>


public class FireRocket : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	//The rocket prefab is attached to this in the
	//inspector.
	
	public GameObject rocket;
	
	
	//Used in setting what team this projectile belongs to
	
	private bool iAmOnRedTeam = false;
	
	private bool iAmOnBlueTeam = false;
	
	
	//Used in setting a weapon fire rate.
	
	private float fireRate = 1.0f;
	
	private float nextFire = 0.0f;
	
	
	//Resource cost. Affects resource value in
	//the PlayerResource script.	

	private float energyCost = 80f;
	
	
	//Used in determining where and in what orientation
	//the projectile will be spawned in.
	
	private Vector3 rocketFireFrom = new Vector3();
	
	
	//cached variables
	
	private Transform myTransform;
	
	private Transform cameraHeadTransform;
	
	private ChangeWeapon weaponScript;
	
	private PlayerEnergy energyScript;
	
	
	//Variables End___________________________________________________________
	
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{
			myTransform = transform;
	
			cameraHeadTransform = myTransform.FindChild("CameraHead");
			
			weaponScript = myTransform.GetComponent<ChangeWeapon>();
			
			energyScript = myTransform.GetComponent<PlayerEnergy>();
			
			
			//Access the SpawnScript and find out what team this player
			//is on. The rocket that is instantiated depends
			//on what team the player is on.
			
			GameObject SpawnM = GameObject.Find("SpawnManager");
			
			SpawnScript spawnScript = SpawnM.GetComponent<SpawnScript>();
			
			if(spawnScript.onRed == true)
			{
				iAmOnRedTeam = true;	
			}
			
			if(spawnScript.onBlue == true)
			{
				iAmOnBlueTeam = true;	
			}
		}
		
		else
		{
			enabled = false;	
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Screen.lockCursor == true && 
		   Input.GetButton("Fire Weapon") && 
		   Time.time > nextFire &&
		   energyScript.energy >= energyCost &&
		   weaponScript.selectedWeapon == ChangeWeapon.State.rocket)
		{			
			
			nextFire = Time.time + fireRate;	
			
			
			//Fire the rocket from just infront of the player.
			
			rocketFireFrom = cameraHeadTransform.TransformPoint(0, 0, 0.3f);
			
			
			//Drain the player's resource.
						
			energyScript.energy -= energyCost;
			
			
			//Instantiate a team specific rocket across the network.
			
			if(iAmOnRedTeam == true)
			{	
				networkView.RPC("SpawnRocket", RPCMode.All, rocketFireFrom, 
				                Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90f, 
				                                 myTransform.eulerAngles.y, 0), myTransform.gameObject.name, "red");
			}
			
			if(iAmOnBlueTeam == true)
			{
				networkView.RPC("SpawnRocket", RPCMode.All, rocketFireFrom, 
				                Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90f, 
				                                 myTransform.eulerAngles.y, 0), myTransform.gameObject.name, "blue");
			}
		}
	}
	
	
	[RPC]
	void SpawnRocket(Vector3 location, Quaternion rot, string originatorName, string team)
	{
		GameObject go = Instantiate(rocket, location, rot) as GameObject;
		
		RocketScript script = go.GetComponent<RocketScript>();
				
		script.myOriginator = originatorName;	
		
		script.team = team;
	}
}
