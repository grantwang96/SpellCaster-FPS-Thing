using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public string RoomID { get; private set; }

    public Room(string roomId) {
        RoomID = roomId;
    }
}
