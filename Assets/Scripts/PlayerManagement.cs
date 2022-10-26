using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerManagement")]
public class PlayerManagement : ScriptableObject
{
    public const int MaxPlayer = 2;
    public GameObject[] playerArray { get; private set; } = new GameObject[MaxPlayer];
    
    public void SetPlayer(int playerIndex, GameObject playerObject)
    {
        Debug.Assert(playerIndex < MaxPlayer);
        if (playerIndex >= MaxPlayer)
            return;

        playerArray[playerIndex] = playerObject;
    }
}
