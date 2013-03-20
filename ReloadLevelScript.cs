using UnityEngine;
using System.Collections;

public class ReloadLevelScript : MonoBehaviour {
	public bool reloadLevel = false;
	public bool restartingMatch = false;
	public float waitTime = 0.1f;
	private static bool created = false;
	
	
	void Awake()
	{
		//ReloadLevel must be only one
		if(created == false)
		{
			DontDestroyOnLoad(gameObject);
			created = true;
		}
		
		else
		{
			Destroy(gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	//when team wins, scoretable will set reloadLevel
		if(reloadLevel == true && Network.isServer)
		{
			//RPC to restart
			networkView.RPC ("serverRestart",RPCMode.All);
			reloadLevel = false;
		}
		if(restartingMatch == true)
		{
			
			//PlayerDatabase tell it that we are restarting
			//Allow player to choose team in spawn
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
			dataScript.matchRestarted = true;
			GameObject spawnManager = GameObject.Find ("SpawnManager");
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			spawnScript.matchRestart = true;
			
			restartingMatch = false;
		}
	}
	
	[RPC]
	void serverRestart()
	{
		Network.RemoveRPCs(Network.player);
		Network.SetSendingEnabled(0, false);
		Network.SetSendingEnabled (1, false);
		Network.isMessageQueueRunning = false;
		Application.LoadLevel("Prototype");
		StartCoroutine (Delay ());
	}
	
	IEnumerator Delay()
	{
		yield return new WaitForSeconds(waitTime);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
		Network.SetSendingEnabled (1, true);
		
		restartingMatch = true;
	}
}
