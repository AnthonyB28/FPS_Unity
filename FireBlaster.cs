using UnityEngine;
using System.Collections;

public class FireBlaster : MonoBehaviour {
	
	public GameObject blaster;
	
	private Transform myTransform;
	private Transform cameraHeadTransform;
	private ChangeWeapon changeScript;
	
	
	private Vector3 launchPosition = new Vector3();
	
	//Control rate of fire
	private float fireRate = 0.2f;
	private float nextFire = 0;
	
	private bool iRed = false;
	private bool iBlue = false;
	
	private PlayerEnergy energyScript;
	private float energyCost = 10;
	
	// Use this for initialization
	void Start () {
		if(networkView.isMine == true)
		{
		 	myTransform = transform;
			cameraHeadTransform = myTransform.FindChild ("CameraHead");
			
			//Find SpawnManager and get SpawnScript for team
			GameObject spawnManager = GameObject.Find ("SpawnManager");
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			changeScript = myTransform.GetComponent<ChangeWeapon>();
			
			
			if(spawnScript.onRed == true)
			{
				iRed = true;
			}
			
			
			if(spawnScript.onBlue == true)
			{
				iBlue = true;
			}
			
			energyScript = myTransform.GetComponent<PlayerEnergy>();
			
		}
		else
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		
		
		if(Input.GetButton ("Fire Weapon") && Time.time > nextFire && Screen.lockCursor == true
				&& energyScript.energy >= energyCost && changeScript.selectedWeapon == ChangeWeapon.State.blaster)
		{
			nextFire = Time.time + fireRate;
			energyScript.energy = energyScript.energy - energyCost;
			
			launchPosition = cameraHeadTransform.TransformPoint(0,0,0.2f);
			
			if(iRed == true)
			{
				
			networkView.RPC("SpawnProjectile", RPCMode.All,launchPosition,
			                Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90,
			                                                    myTransform.eulerAngles.y, 0), myTransform.name, "red");
			}
			//Instantiate(blaster,launchPosition,Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90,
																//myTransform.eulerAngles.y, 0));
			if(iBlue == true)
			{
				networkView.RPC("SpawnProjectile", RPCMode.All,launchPosition,
			                Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90,
			                                                    myTransform.eulerAngles.y, 0), myTransform.name, "blue");
			}
		}
	}
	
	[RPC]
	void SpawnProjectile (Vector3 position, Quaternion rotation, string originatorName, string team)
	{
		GameObject go = Instantiate(blaster, position, rotation) as GameObject;	
		BlasterScript bScript = go.GetComponent<BlasterScript>();
		bScript.myOriginator = originatorName;
		bScript.team = team;
	}
}
