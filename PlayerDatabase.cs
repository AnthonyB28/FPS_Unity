using UnityEngine;
using System.Collections;
using System.Collections.Generic; //C# lists

/// <summary>
/// Player database manages PlayerList
/// attach to GameManager
/// </summary>

public class PlayerDatabase : MonoBehaviour {
	public List<PlayerDataClass> PlayerList = new List<PlayerDataClass>();
		
		public NetworkPlayer networkPlayer;
		public bool nameSet = false;
		public string playerName;
		public bool scored = false;
		public int playerScore;
	
	public bool joinedTeam = false;
	public string playerTeam;
	
	public List<NetworkPlayer> nPlayerList = new List<NetworkPlayer>();
	public bool matchRestarted = false;
	public bool addPlayerAgain = false;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if(nameSet == true)
		{
			//EDIT player record
			networkView.RPC ("EditPlayerListWithName", RPCMode.AllBuffered, Network.player, playerName);
			nameSet = false;
		}
		
	if(scored == true)
		{
			networkView.RPC ("EditPlayerListWithScore", RPCMode.AllBuffered, Network.player, playerScore);
			scored = false;
		}
		
	if(joinedTeam == true)
		{
			networkView.RPC ("EditPlayerListWithTeam", RPCMode.AllBuffered, Network.player, playerTeam);
			joinedTeam = false;
		}
		
		//Match restarted, playerList deleted, need to be added again
		if(Network.isServer == true && addPlayerAgain == true)
		{
			foreach(NetworkPlayer netPlayer in nPlayerList)
			{
				networkView.RPC ("AddPlayerToList", RPCMode.AllBuffered, netPlayer);
				
			}
			nPlayerList.Clear ();
			addPlayerAgain = false;
		}
		if(Network.isClient == true && matchRestarted == true)
		{
			networkView.RPC ("AddPlayerBack", RPCMode.Server, Network.player);
			matchRestarted = false;
		}
	}
	
	void OnPlayerConnected(NetworkPlayer netPlayer)
	{
		//Add player to the list
		networkView.RPC ("AddPlayerToList", RPCMode.AllBuffered, netPlayer);
	
	}
	
	void OnPlayerDisconnected(NetworkPlayer netPlayer)
	{
		//Add player to the list
		networkView.RPC ("RemovePlayerFromList", RPCMode.AllBuffered, netPlayer);
	
	}
	
	[RPC]
	void AddPlayerToList(NetworkPlayer nPlayer)
	{
	PlayerDataClass capture = new PlayerDataClass();
		capture.networkPlayer = int.Parse (nPlayer.ToString());
		PlayerList.Add (capture);	
	}
	
	[RPC]
	void RemovePlayerFromList(NetworkPlayer nPlayer)
	{
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse (nPlayer.ToString ()))
			{
				PlayerList.RemoveAt(i);
			}
		}
	}
	
	[RPC]
	void EditPlayerListWithName(NetworkPlayer nPlayer, string pName)
	{
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse (nPlayer.ToString ()))
			{
				PlayerList[i].playerName = pName;
			}
		}
	}
	
	[RPC]
	void EditPlayerListWithScore(NetworkPlayer nPlayer, int pScore)
	{
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse (nPlayer.ToString ()))
			{
				PlayerList[i].playerScore = pScore;
			}
		}
	}
	
	[RPC]
	void EditPlayerListWithTeam(NetworkPlayer nPlayer, string pTeam)
	{
		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].networkPlayer == int.Parse (nPlayer.ToString ()))
			{
				PlayerList[i].playerTeam = pTeam;
			}
		}
	}
	[RPC]
	void AddPlayerBack(NetworkPlayer nPlayer)	
	{
		nPlayerList.Add (nPlayer);
		addPlayerAgain = true;
	}
}
