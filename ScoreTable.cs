using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This script is attached to the GameManager and is responsible
/// for displaying the player scoreboard and keeping team scores up
/// to date.
/// 
/// This script makes use of the PlayerDataClass so that it can construct
/// the SortingList.
/// 
/// This script accesses the PlayerDatabase so that the PlayerList can be
/// copied into the SortingList.
/// 
/// This script is accessed by the PlayerScore script for updating the team
/// score.
/// </summary>


public class ScoreTable : MonoBehaviour {
	
	//Variables Start___________________________________
	
	//These variables are used in displaying the scoreboard.
	
	public bool showScoreTable = false;
	
	public List<PlayerDataClass> SortingList = new List<PlayerDataClass>();
	
	private GUIStyle myStyle = new GUIStyle();
	
	
	//These are used in managing the team score and displaying it.
	
	private GUIStyle redHeaderStyle = new GUIStyle();
	
	private GUIStyle blueHeaderStyle = new GUIStyle();
	
	public bool updateRedScore = false;
	
	public bool updateBlueScore = false;
	
	public int enemiesDestroyedInOneHit;
	
	public bool serverRefreshScore = false;
	
	public int redTeamScore;
	
	public int blueTeamScore;
	
	//Winning score
	private GUIStyle winStyle = new GUIStyle();
	public bool redWin = false;
	public bool blueWin = false;
	public int winScore;
	public int waitTime = 7;
	
	//Variables End_____________________________________
	
	
	// Use this for initialization
	void Start () 
	{
		myStyle.fontStyle = FontStyle.Bold;
		
		myStyle.normal.textColor = Color.white;
		
		
		redHeaderStyle.fontSize = 16;
		
		redHeaderStyle.fontStyle = FontStyle.Bold;
		
		redHeaderStyle.normal.textColor = Color.red;
		
		
		blueHeaderStyle.fontSize = 16;
		
		blueHeaderStyle.fontStyle = FontStyle.Bold;
		
		blueHeaderStyle.normal.textColor = Color.blue;
		
		winStyle.fontSize = 40;
		winStyle.normal.textColor = Color.white;
		winStyle.fontStyle = FontStyle.Bold;
		winStyle.alignment = TextAnchor.MiddleCenter;
		
		//get winning criteria from the multiplayer script
		GameObject multiManager = GameObject.Find("MultiplayerManager");
		MultiplayerScript multiScript = multiManager.GetComponent<MultiplayerScript>();
		winScore = multiScript.winScore;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButton("Show Player Scores"))
		{
			showScoreTable = true;	
		}
		
		if(Input.GetButtonUp("Show Player Scores"))
		{
			showScoreTable = false;	
		}
		
		
		//Whenever the player's score increases in their PlayerScore script.
		//The PlayerScore script will send a signal to this script to increment the
		//overall team score. An RPC is sent out across the network so that everyone
		//gets the latest team score.
		
		if(updateRedScore == true)
		{
			for(int i = 0; i < enemiesDestroyedInOneHit; i++)
			{
				networkView.RPC("UpdateRedTeamScore", RPCMode.All);	
			}
			
			enemiesDestroyedInOneHit = 0;
			
			updateRedScore = false;
		}
		
		
		if(updateBlueScore == true)
		{
			for(int i = 0; i < enemiesDestroyedInOneHit; i++)
			{
				networkView.RPC("UpdateBlueTeamScore", RPCMode.All);	
			}
			
			enemiesDestroyedInOneHit = 0;
			
			updateBlueScore = false;
		}
		
		
		//The server needs to refresh the score because when the player who
		//scored leaves their RPCs are deleted. That means that new players will 
		//never be able to receive the message for their team score to be incremented.
		
		if(Network.isServer && serverRefreshScore == true)
		{
			networkView.RPC("ServerRefreshScore", RPCMode.AllBuffered, 
			                redTeamScore, blueTeamScore);	
			
			serverRefreshScore = false;
		}
		
		
		//Either team reaches win score, then activate bool
		if(blueTeamScore >= winScore)
		{
			blueWin = true;
		}
		
		if(redTeamScore >= winScore)
		{
			redWin = true;
		}
		
		
		
	}
	
	void OnGUI ()
	{
		if(showScoreTable == true)
		{
			//Clear the list used for displaying players and their scores.
			
			SortingList.Clear();
			
			
			//Access the PlayerDatabase as we will copy the contents of the 
			//PlayerList.
			
			PlayerDatabase dataScript = transform.GetComponent<PlayerDatabase>();
			
			
			//Copy each item in the PlayerList into the SortingList.
			
			for(int i = 0; i < dataScript.PlayerList.Count; i++)
			{
				SortingList.Add(dataScript.PlayerList[i]);	
			}
			
			
			//Sort the SortingList. By default players will be sorted in order of
			//ascending score so we'll need to reverse this when we come to actually
			//displaying the score.
			
			SortingList.Sort(delegate(PlayerDataClass player1, PlayerDataClass player2)
			{
				return player1.playerScore.CompareTo(player2.playerScore);
			});
			
			
			//Display the scoreboard header.
			
			GUI.Box(new Rect(Screen.width / 2 - 260, 10, 520, 30),"");
			
			GUI.Label(new Rect(Screen.width / 2 - 150, 15, 300, 30),
								"Team Score to Win: " + winScore.ToString (), myStyle);
			
			
			//Start a new GUI area on the left portion of the screen. This area will
			//be used for displaying red team scores.
			
			GUILayout.BeginArea(new Rect(Screen.width / 2 - 260, 50, 250, Screen.height - 10));
			
			
			GUILayout.BeginVertical("box");
			
			GUILayout.BeginHorizontal("");
			
			//Display a header with the team name and score.
			
			GUILayout.Label("Red Team" , redHeaderStyle, GUILayout.Width(200));
			
			GUILayout.Label(redTeamScore.ToString(), redHeaderStyle, GUILayout.Width(40));
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
			
			
			
			//Go through the SortingList in reverse and pick out each player that
			//belongs to the red team and display their name and score.
			
			for(int i = SortingList.Count - 1; i >= 0; i--)
			{
				if(SortingList[i].playerTeam == "red")
				{
					GUILayout.BeginHorizontal("box");
					
					GUILayout.Label(SortingList[i].playerName, myStyle, GUILayout.Width(200));
					
					GUILayout.Label(SortingList[i].playerScore.ToString(), myStyle, GUILayout.Width(40));
					
					GUILayout.EndHorizontal();
				}
			}
			
			GUILayout.EndArea();
			
			
			
			
			
			//Start a new GUI area on the right portion of the screen. This area will
			//be used for displaying blue team scores.
			
			GUILayout.BeginArea(new Rect(Screen.width / 2 + 10, 50, 250, Screen.height - 10));
			
			
			GUILayout.BeginVertical("box");
			
			GUILayout.BeginHorizontal("");
			
			//Display a header with the team name and score.
			
			GUILayout.Label("Blue Team" , blueHeaderStyle, GUILayout.Width(200));
			
			GUILayout.Label(blueTeamScore.ToString(), blueHeaderStyle, GUILayout.Width(40));
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
			
			
			//Go through the SortingList in reverse and pick out each player that
			//belongs to the blue team and display their name and score.
			
			for(int i = SortingList.Count - 1; i >= 0; i--)
			{
				if(SortingList[i].playerTeam == "blue")
				{
					GUILayout.BeginHorizontal("box");
					
					GUILayout.Label(SortingList[i].playerName, myStyle, GUILayout.Width(200));
					
					GUILayout.Label(SortingList[i].playerScore.ToString(), myStyle, GUILayout.Width(40));
					
					GUILayout.EndHorizontal();
				}
			}
			
			GUILayout.EndArea();
			
			
			
		}
		//When team wins, display this box
			
		if(blueWin == true)
		{
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"Blue Team Won!",winStyle);
			
			if(Network.isServer)
			{
				StartCoroutine(RestartMatch());
			}
		}
		
		if(redWin == true)
		{
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),"Red Team Won!",winStyle);
			
			if(Network.isServer)
			{
				StartCoroutine(RestartMatch());
			}
		}
			
	}
	
	
	[RPC]
	void UpdateRedTeamScore ()
	{
		redTeamScore++;
		
		serverRefreshScore = true;
	}
	
	[RPC]
	void UpdateBlueTeamScore ()
	{
		blueTeamScore++;
		
		serverRefreshScore = true;
	}
	
	[RPC]
	void ServerRefreshScore (int redScore, int blueScore)
	{
		redTeamScore = redScore;
		
		blueTeamScore = blueScore;
	}
	
	//Set the reloadScript to reload the level
	void restartGame()
	{
		GameObject reload = GameObject.Find("ReloadLevel");
		ReloadLevelScript reloadScript = reload.GetComponent<ReloadLevelScript>();
		reloadScript.reloadLevel = true;
	}
	
	IEnumerator RestartMatch()
	{
		yield return new WaitForSeconds(waitTime);
		restartGame();
	}
	
	
	
	
	
	
	
	
	
	
	
	
}
