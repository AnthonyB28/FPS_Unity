///	<summary>
/// PlayerDatabase building PlayerList
///</summary>

public class PlayerDataClass{
	
	public int networkPlayer;
	public string playerName;
	public int playerScore;
	public string playerTeam;
	
	public PlayerDataClass Constructor()
	{
		PlayerDataClass capture = new PlayerDataClass();
		capture.networkPlayer = networkPlayer;
		capture.playerName = playerName;
		capture.playerScore = playerScore;
		capture.playerTeam = playerTeam;
		return capture;
	}
	
	
}