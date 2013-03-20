using UnityEngine;
using System.Collections;

public class CursorControl : MonoBehaviour {
	
	private GameObject multiplayerManager;
	private MultiplayerScript myScript;
	
	private GameObject gameManager;
	private CommunicationWindow commScript;
	private ScoreTable scoreScript;

	// Use this for initialization
	void Start () {
		
	if(networkView.isMine == true)
		{
			multiplayerManager = GameObject.Find ("MultiplayerManager");
			myScript = multiplayerManager.GetComponent<MultiplayerScript>();
			gameManager = GameObject.Find ("GameManager");
			commScript = gameManager.GetComponent<CommunicationWindow>();
			scoreScript = gameManager.GetComponent<ScoreTable>();
		}
		
		else {enabled = false;}
	
	}
	
	// Update is called once per frame
	void Update () {
	if(myScript.showDisconnectWindow == false && commScript.unlockCursor == false
			&& scoreScript.blueWin == false && scoreScript.redWin == false)
		{
			Screen.lockCursor = true;
		}
		
	if(myScript.showDisconnectWindow == true || commScript.unlockCursor == true
			|| scoreScript.blueWin == true || scoreScript.redWin == true)
			
		{
			Screen.lockCursor = false;
		}
	}
}
