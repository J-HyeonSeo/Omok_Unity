using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomRequest
{
    public string roomTitle;
    public string playerName;

    public CreateRoomRequest(string roomTitle, string playerName)
    {
        this.roomTitle = roomTitle;
        this.playerName = playerName;
    }
}
