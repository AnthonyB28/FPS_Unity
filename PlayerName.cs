using UnityEngine;
using System.Collections;

public class PlayerName : MonoBehaviour {
	
	public string playerName;

	// Use this for initialization
	void Awake () {
	
		if(networkView.isMine == true)
		{
			playerName = PlayerPrefs.GetString ("playerName");
			
			foreach(GameObject nameCheck in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if(playerName == nameCheck.name)
				{
					float x = Random.Range(0, 1000);
					playerName = "(" + x.ToString() + ")";
					PlayerPrefs.SetString("playerName", playerName);
				}
			}
			//Update local GameManager with player's name
			UpdateLocalGameManager(playerName);
			//Send out RPC
			networkView.RPC("UpdateMyNameEverywhere", RPCMode.AllBuffered, playerName);
		}
	}
	
	void UpdateLocalGameManager(string pName)
	{
		//Append name to list
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		dataScript.nameSet = true;
		dataScript.playerName = pName;
		
		//Supply the communicationWindowScript with the player's name
		CommunicationWindow commScript = gameManager.GetComponent<CommunicationWindow>();
		commScript.playerName = pName;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	[RPC]
	void UpdateMyNameEverywhere(string pName)
	{
		//Change gameobjectname from clone
		gameObject.name = pName;
		playerName = pName;
		PlayerLabel labelScript = transform.GetComponent<PlayerLabel>();
		labelScript.playerName = pName;
	}
}
