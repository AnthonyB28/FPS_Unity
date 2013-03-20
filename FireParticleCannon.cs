using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the ParticleCannon prefab. The
/// prefab is a child object of the player and it allows them to
/// fire a stream of particles.
/// 
/// This script accesses the SpawnScript to determine which team 
/// this player is on.
/// 
/// This script accesses the HealthAndDamage script of the struck
/// player to apply damage.
/// 
/// This script accesses the ChangeWeapon script to check that it is
/// the currently selected weapon.
/// </summary>

public class FireParticleCannon : MonoBehaviour {
	
	//Variables Start_________________________________________________________
	
	private Transform parentTransform;
	
	//Used in determining which team
	//can be hurt by this player.
	
	private bool iAmOnRedTeam = false;
	
	private bool iAmOnBlueTeam = false;
	
	
	//Used in setting a weapon fire rate.
	
	private float fireRate = 0.01f;
	
	private float nextFire = 0.0f;
	
	private float particleSpeed = 100;
	
	private float particleSize = 1.5f;
	
	private float particleEnergy = 2;
	
	public float cooldown = 1f;
	private bool cooldownWait = false;
	
	
	//Energy cost. Affects energy value in
	//MyEnergy script.	

	private float energyCost = 1.5f;
	
	
	//Used in determining where and in what orientation
	//the beam will be emitted.
	
	private Transform cameraHeadTransform;

	private Vector3 particleFireFrom = new Vector3();
	
	private Vector3 hitPosition = new Vector3();
	
	
	//Used in determining where and when the particle hit 
	//effect should be instantiated.
	
	private float distanceFromHit;
	
	private float delay;
	
		
	//These variables are used for the rayacast.
	
	private RaycastHit hit;
	
	private float range = 200f;
	
	
	//The particleCannonHit prefab is attached to this
	//in the inspector.
	
	public GameObject particleCannonHit;
	
	
	private ChangeWeapon weaponScript;
	
	private PlayerEnergy energyScript;
	
	private GUIStyle style = new GUIStyle();
	
	
	//Variables End___________________________________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine == true)
		{	
			parentTransform = transform.parent;
	
			cameraHeadTransform = parentTransform.FindChild("CameraHead");
			
			weaponScript = parentTransform.GetComponent<ChangeWeapon>();
			
			energyScript = parentTransform.GetComponent<PlayerEnergy>();
			
			style.fontSize = 40;
			style.normal.textColor = Color.red;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			
			
			//Check which team this player is on. This will determine
			//who can get hurt by this player's beam.
			
			GameObject SpawnM = GameObject.Find("SpawnManager");
			
			SpawnScript script =  SpawnM.GetComponent<SpawnScript>();
			
			if(script.onRed == true)
			{
				iAmOnRedTeam = true;	
			}
			
			if(script.onBlue == true)
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
		if(Input.GetButton ("Fire Weapon") && Time.time > nextFire && Screen.lockCursor == true
				&& energyScript.energy >= energyCost && weaponScript.selectedWeapon == ChangeWeapon.State.particleCannon
				&& cooldownWait == false)
		{
			
			nextFire = Time.time + fireRate;
			
			//Drain the player's energy.
							
			energyScript.energy = energyScript.energy - energyCost;
			
			if(energyScript.energy < 5.0f)
			{
				cooldownWait = true;
				StartCoroutine(BlasterWaitEffect(cooldown));
			}
			
			//Capture the position from which to fire the particle and then emit
			//particles across the network.
			
			particleFireFrom = cameraHeadTransform.position;
				
			networkView.RPC("EmitParticleCannon", RPCMode.All, particleFireFrom, cameraHeadTransform.forward);
			
			
			//Determine where to instantiate the particle hit effect.
			
			if(Physics.Raycast(cameraHeadTransform.position, cameraHeadTransform.forward, out hit, range))
			{
				hitPosition = hit.point;
				
				
				//Calculate how long to wait before instantiating the hit effect.
				
				distanceFromHit = Vector3.Distance(hit.point, cameraHeadTransform.position);
				
				delay = particleEnergy * (distanceFromHit/range);
				
				
				StartCoroutine(BlasterHitEffect(delay, hitPosition));
				
			}			
		}
	}
	
	void OnGUI()
	{
		if(cooldownWait == true)
		{
			//GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"COOLDOWN", style);

		}
	}
	
	
	void OnParticleCollision(GameObject other)
	{	
		if(networkView.isMine == true)
		{				
			if(other.transform.tag == "RedTeamTrigger" && iAmOnBlueTeam == true ||
			   other.transform.tag == "BlueTeamTrigger" && iAmOnRedTeam == true)
			{
				//Reduce the enemy player's health, which is located in the 
				//HealthAndDamage script, and tell the enemy player
				//who struck it.
	
				HealthAndDamage HDScript = other.GetComponent<HealthAndDamage>();
				
				HDScript.myAttacker = parentTransform.name;
					
				HDScript.iWasAttacked = true;
				
				HDScript.hitByParticleCannon = true;
			}	
		}
	}
	
	
	IEnumerator BlasterHitEffect(float waitTime, Vector3 pos)
	{
		yield return new WaitForSeconds(waitTime);
		
		networkView.RPC("ParticleCannonHitEffect", RPCMode.All, pos);
	}
	
	IEnumerator BlasterWaitEffect(float cooldown)
	{
		yield return new WaitForSeconds(cooldown);
		cooldownWait = false;
		
	}
	
	
	[RPC]
	void EmitParticleCannon (Vector3 fireFrom, Vector3 direction)
	{
		particleEmitter.Emit(fireFrom, direction * particleSpeed, particleSize, particleEnergy, Color.white);
	}
	
	
	[RPC]
	void ParticleCannonHitEffect (Vector3 hitLocation)
	{
		Instantiate(particleCannonHit, hitLocation, Quaternion.identity);	
	}
}
