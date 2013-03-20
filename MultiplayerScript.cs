using UnityEngine;
using System.Collections;

public class MultiplayerScript : MonoBehaviour {
	
	private string titleMessage = "Prototype";
	private string connectToIP = "127.0.0.1";
	private int connectionPort = 26500;
	private bool useNAT = true;
	private string ipAddress;
	private string port;
	private int numOfPlayers = 10;
	public string playerName;
	public string serverName;
	public string serverNameForClient;
	private bool iWantToSetupAServer = false;
	private bool iWantToConnectToAServer = false;
	
	
	private Rect connectionWindowRect;
	private int connectionWindowWidth = 400;
	private int connectionWindowHeight = 280;
	private int buttonHeight = 60;
	private int leftIndent;
	private int topIndent;
	
	private Rect serverDisWindowRect;
	private int serverDisWindowWidth = 200;
	private int serverDisWindowHeight = 150;
	private int serverDisWindowLeftIndent = 10;
	private int serverDisWindowTopIndent = 10;
	
	private Rect clientDisWindowRect;
	private int clientDisWindowWidth = 300;
	private int clientDisWindowHeight = 170;
	public bool showDisconnectWindow = false;
	
	public int winScore = 20;
	private int scoreButtonWidth = 20;
	private GUIStyle plainStyle = new GUIStyle();
	
	// Use this for initialization
	// Load serverName
	void Start () {
	serverName = PlayerPrefs.GetString ("serverName");
	if(serverName == "")
		{
			serverName = "Server";
		}
	playerName = PlayerPrefs.GetString ("playerName");
		if(playerName == "")
		{
			playerName = "Player";
		}
		
		plainStyle.alignment = TextAnchor.MiddleLeft;
		plainStyle.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	if(Input.GetKeyDown(KeyCode.Escape))
		{
			showDisconnectWindow = !showDisconnectWindow;
		}
	}
	
	
	void ConnectWindow(int windowID)
	{
		//Gap
		GUILayout.Space(15);
		
		//When the player launches, option to create server
		if(iWantToSetupAServer == false && iWantToConnectToAServer == false)
		{
			if(GUILayout.Button("Setup a Server", GUILayout.Height (buttonHeight)))
			{
				iWantToSetupAServer = true;
			}
			
			GUILayout.Space(10);
			
			if(GUILayout.Button("Connect to a Server", GUILayout.Height (buttonHeight)))
			{
				iWantToConnectToAServer = true;
			}
			
			GUILayout.Space(10);
			
			if(Application.isWebPlayer == false && Application.isEditor == false)
			{
				if(GUILayout.Button("Exit Prototype", GUILayout.Height (buttonHeight)))
				{
					Application.Quit();
				}
			}
		}
		
		//Player wants to setup
		if(iWantToSetupAServer == true)
		{
			//Type server name
			GUILayout.Label ("Server Name:");
			serverName = GUILayout.TextField(serverName);
			GUILayout.Space (5);
			
			//Type Port Number
			GUILayout.Label("Server Port:");
			connectionPort = int.Parse (GUILayout.TextField(connectionPort.ToString ()));
			GUILayout.Space (10);
			
			//Create Server
			if(GUILayout.Button("Start Server", GUILayout.Height(30)))
			{
				Network.InitializeServer(numOfPlayers, connectionPort, useNAT);
				
				//Save ServerName to PlayerPrefs
				PlayerPrefs.SetString("serverName", serverName);
				
				//Tell the ScoreTable script the win criteria, local function not [RPC]
				TellEveryoneWinningCriteria(winScore);
				
				iWantToSetupAServer = false;
			}
			
			if(GUILayout.Button("Back", GUILayout.Height (30)))
			{
				iWantToSetupAServer = false;
			}
		}
		
		//Player wants to connect
		if(iWantToConnectToAServer == true)
		{
			//Type Player Name
			GUILayout.Label ("Player name:");
			playerName = GUILayout.TextField(playerName);
			GUILayout.Space (5);
			
			//Type IP server
			GUILayout.Label ("Server IP:");
		 	connectToIP = GUILayout.TextField(connectToIP);
			GUILayout.Space (5);
			
			//Type Server Port
			GUILayout.Label ("Server Port:");
		 	connectionPort = int.Parse (GUILayout.TextField(connectionPort.ToString ()));
			GUILayout.Space (5);
			
			
			//Connect to Server
			if(GUILayout.Button("Connect", GUILayout.Height(30)))
			{
				//Cannot join empty name
				if(playerName == "")
				{
					playerName = "Player";
				}
				
				if(playerName != "")
				{
					Network.Connect (connectToIP,connectionPort);
				    PlayerPrefs.SetString ("playerName", playerName);
				}
			}
			
			if(GUILayout.Button("Back", GUILayout.Height (30)))
			{
				iWantToConnectToAServer = false;
			}
		}
		
	}
	
	void ServerDisconnectWindow(int windowID)
	{
		GUILayout.Label ("Server name: " + serverName);
		GUILayout.Label ("# of Players: " + Network.connections.Length);
		if(Network.connections.Length >= 1)
		{
		GUILayout.Label ("Avg. Ping: " + Network.GetAveragePing(Network.connections[0]));
		}
		if(GUILayout.Button ("Shutdown Server"))
		{
			Network.Disconnect();
		}
		
	}
	
	void ClientDisconnectWindow(int windowID)
	{
		GUILayout.Label ("Server Name: " + serverName);
		GUILayout.Label ("Ping: " + Network.GetAveragePing(Network.connections[0]));
		GUILayout.Space (7);
		
		//Disconnect
		if(GUILayout.Button ("Disconnect", GUILayout.Height (25)))
		{
			Network.Disconnect();
		}
		GUILayout.Space (5);
		
		if(GUILayout.Button ("Return to Game", GUILayout.Height (25)))
		{
			showDisconnectWindow = false;
		}
	}
	
	void OnDisconnectedFromServer()
	{
		//Player loses connection, restart
		Application.LoadLevel ("Prototype");
	}
	
	void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
		//Remove the RPCs from the player and destroy player's objects if player gone.
		Network.RemoveRPCs (networkPlayer);
		Network.DestroyPlayerObjects(networkPlayer);
	}
	
	void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		//Send RPCs to connected players
		networkView.RPC("TellPlayerServerName",networkPlayer, serverName);
		
		networkView.RPC ("TellEveryoneWinningCriteria", networkPlayer, winScore);
	}
	
	void OnGUI()
	{
		//Player disconnected
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			//Window setting
			leftIndent = (Screen.width/2) - connectionWindowWidth / 2;
			topIndent = Screen.height / 2 - connectionWindowWidth / 2;
			connectionWindowRect = new Rect(leftIndent,topIndent,connectionWindowWidth, connectionWindowHeight);
			connectionWindowRect = GUILayout.Window (0, connectionWindowRect, ConnectWindow, titleMessage);
		}
		
		//Server Running
		if(Network.peerType == NetworkPeerType.Server)
		{
			serverDisWindowRect = new Rect(serverDisWindowLeftIndent, serverDisWindowTopIndent,
											serverDisWindowWidth, serverDisWindowHeight);
			serverDisWindowRect = GUILayout.Window(1, serverDisWindowRect, ServerDisconnectWindow, "");
			
		//Server can change score.
			GUI.Box (new Rect(10, 190, 170, 40), "");
			GUILayout.BeginArea(new Rect(15,200,180,60));
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Win Score:", plainStyle, GUILayout.Width(70), GUILayout.Height(scoreButtonWidth));
			GUILayout.Label (winScore.ToString(), plainStyle, GUILayout.Width(30), GUILayout.Height(scoreButtonWidth));
			if(GUILayout.Button("+", GUILayout.Width (scoreButtonWidth), GUILayout.Height (scoreButtonWidth)))
			{
				if(winScore >= 10)
				{
					winScore = winScore + 10;
				}
				if(winScore < 10)
				{
					winScore = winScore + 9;
				}
				
				networkView.RPC ("TellEveryoneWinningCriteria", RPCMode.All, winScore);
			}
			if(GUILayout.Button("-", GUILayout.Width (scoreButtonWidth), GUILayout.Height (scoreButtonWidth)))
			{
				winScore = winScore - 10;
				if(winScore <= 0)
				{
					winScore = 1;
				}
				
				networkView.RPC ("TellEveryoneWinningCriteria", RPCMode.All, winScore);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		
		if(Network.peerType == NetworkPeerType.Client && showDisconnectWindow == true)
		{
			clientDisWindowRect = new Rect(Screen.width /2 - clientDisWindowWidth/2,
											Screen.height /2 - clientDisWindowHeight / 2,
											clientDisWindowWidth, clientDisWindowHeight);
			clientDisWindowRect = GUILayout.Window (1, clientDisWindowRect, ClientDisconnectWindow, "");
		}	
	}
	
	
	
	
	//Tells players the serverName and win.
	[RPC]
	void TellPlayerServerName(string server)
	{
		serverName = server;
		
	}
	[RPC]
	void TellEveryoneWinningCriteria(int winScore)
	{
		GameObject gameManager = GameObject.Find ("GameManager");
		ScoreTable scoreScript = gameManager.GetComponent<ScoreTable>();
		scoreScript.winScore = winScore;
	}
	
	
}
