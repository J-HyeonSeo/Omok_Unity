using System;

public class GameData
{
    public string roomId;
    public string roomTitle;
    public string[] playerIdList;
    public string blackPlayerId;
    public string whitePlayerId;
    public string otherPlayerName;
    public State nowState;
    /*
     * "turnedAt":[2024,7,14,18,45,7,868696000]
     */
    public long[] turnedAt;
    public string winnerPlayerId;
    public Piece[] board;
}
