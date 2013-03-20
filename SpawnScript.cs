using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {
	//If player needs to spawn
	private bool justConnected = false;
	
	//Which team
	public bool onRed = false;
	public bool onBlue = false;
	
	//Window
	private Rect joinRect;
	private string joinTitle = "Team Selection";
	private int joinWidth = 330;
	private int joinHeight = 370;
	private int joinLeftIndent;
	private int joinTopIndent;
	private int buttonHeight = 40;
	
	public Transform redTeamPlayer;
	public Transform blueTeamPlayer;
	
	private int redTeamGroup = 0;
	private int blueTeamGroup = 1;
	
	private GameObject[] redSpawnPoints;
	private GameObject[] blueSpawnPoints;
	
	public bool destroyed = false;
	public bool firstSpawn = false;
	
	//Select team again if match restarted
	public bool matchRestart = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//Unity function
	void OnConnectedToServer()
	{
		justConnected = true;
	}
	
	//Only shows if just connected
	void JoinTeamWindow (int windowID)
	{
		//Stats function
		GameObject gameManager = GameObject.Find ("GameManager");
		
		PlayerStats statScript = gameManager.GetComponent<PlayerStats>();
		
		statScript.Stats();
		
		if(justConnected == true || matchRestart == true)
		{
			if(GUILayout.Button ("Join Red Team", GUILayout.Height (buttonHeight)))
			{
				onRed = true;
				justConnected = false;
				matchRestart = false;
				firstSpawn = true;
				SpawnRedTeamPlayer();
			}
			
			if(GUILayout.Button ("Join Blue Team", GUILayout.Height (buttonHeight)))
			{
				onBlue = true;
				justConnected = false;
				matchRestart = false;
				firstSpawn = true;
				SpawnBlueTeamPlayer();
			}
		}
		
	//Respawn if destroyed
		if(destroyed == true && matchRestart == false)
		{
			if(GUILayout.Button("Respawn", GUILayout.Height (buttonHeight * 2)))
			{
				if(onRed == true)
				{
					SpawnRedTeamPlayer();
				}
				if(onBlue == true)
				{
					SpawnBlueTeamPlayer();
				}
				destroyed = false;
			}
		}
	}
	
	void OnGUI()
	{
		if(justConnected == true || destroyed == true || matchRestart == true && Network.isClient)
		{
			Screen.lockCursor = false;
			joinLeftIndent = Screen.width /2 - joinWidth / 2;
			joinTopIndent = Screen.height / 2 - joinHeight / 2;
			joinRect = new Rect(joinLeftIndent, joinTopIndent, joinWidth, joinHeight);
			joinRect = GUILayout.Window (0, joinRect, JoinTeamWindow, joinTitle);
		}
	}
	
	void SpawnRedTeamPlayer()
	{
		//Find spawn points
		redSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnRedTeam");
		GameObject randomRedSpawn = redSpawnPoints[Random.Range(0,redSpawnPoints.Length)];
	    Network.Instantiate(redTeamPlayer,randomRedSpawn.transform.position,randomRedSpawn.transform.rotation,redTeamGroup);
		
		//Access PlayerDatabase and give them a team
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		dataScript.joinedTeam = true;
		dataScript.playerTeam = "red";
	}
	
	void SpawnBlueTeamPlayer()
	{
		//Find spawn points
		blueSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnBlueTeam");
		GameObject randomBlueSpawn = blueSpawnPoints[Random.Range(0,blueSpawnPoints.Length)];
	    Network.Instantiate(blueTeamPlayer,randomBlueSpawn.transform.position,randomBlueSpawn.transform.rotation,blueTeamGroup);
		
		//Access PlayerDatabase and give them a team
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		dataScript.joinedTeam = true;
		dataScript.playerTeam = "blue";
	}
}
