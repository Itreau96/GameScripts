using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    // Player data values
    public int kills; // Number of enemies defeated
    public int ring; // Current ring achieved
    public bool newPlayer; // Determines whether or not player is new
}
