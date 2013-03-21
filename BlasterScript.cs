using UnityEngine;
using System.Collections;

public class BlasterScript : MonoBehaviour {
	private Transform myTransform;
	
	private float projectileSpeed = 20;
	
	private bool expended = false;
	
	//Lifespan of the projectile
	private float expiretime = 5;
	
	//ray projected inront of projectile
	private RaycastHit hit;
	
	//Range of the ray
	private float range = 1.5f;
	
	public GameObject blasterExplosion;
	
	//Hit Detection
	public string team;
	public string myOriginator;
	
	// Use this for initialization
	void Start () {
	myTransform = transform;
	StartCoroutine(DestroyMyselfAfterSomeTime());
	}
	
	// Update is called once per frame
	void Update () {
		//Translate the projectile in the up direction
		myTransform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);
		
		//If the ray hits something then execute this code.
		if(Physics.Raycast(myTransform.position, myTransform.up, out hit, range) &&
			expended == false)
		{
		
			//If the collider has the tag of Floor then..
		if(hit.transform.tag == "Floor")
			{
			expended = true;
				Instantiate(blasterExplosion, hit.point, Quaternion.identity);
			//Make the projectile invisible
			myTransform.renderer.enabled = false;
			myTransform.light.enabled = false;
			}
			//Check for tag of team
		if(hit.transform.tag == "BlueTeamTrigger" || hit.transform.tag == "RedTeamTrigger" || hit.transform.tag == "ConstructionBlock")
			{
				expended = true;
				Instantiate(blasterExplosion, hit.point, Quaternion.identity);
				//Make the projectile invisible
				myTransform.renderer.enabled = false;
				myTransform.light.enabled = false;
			if(hit.transform.tag == "BlueTeamTrigger" && team == "red")
				{
					HealthAndDamage HDscript = hit.transform.GetComponent<HealthAndDamage>();
					HDscript.iWasAttacked = true;
					HDscript.myAttacker = myOriginator;
					HDscript.hitByBlaster = true;
				}
				if(hit.transform.tag == "RedTeamTrigger" && team=="blue")
				{
					HealthAndDamage HDscript = hit.transform.GetComponent<HealthAndDamage>();
					HDscript.iWasAttacked = true;
					HDscript.myAttacker = myOriginator;
					HDscript.hitByBlaster = true;
				}
			}
		}
		
			
		
		
	}
	
	IEnumerator DestroyMyselfAfterSomeTime()
	{
		//Wait for the timer
		yield return new WaitForSeconds(expiretime);
		Destroy(myTransform.gameObject);
	}
}
