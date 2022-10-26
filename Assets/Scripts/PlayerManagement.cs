using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerManagement")]
public class PlayerManagement : ScriptableObject
{
    public const int MaxPlayer = 2;
    public NetworkObject[] playerArray { get; private set; } = new NetworkObject[MaxPlayer];
    
    public void SetPlayer(int playerIndex, NetworkObject playerObject)
    {
        if (playerIndex >= MaxPlayer)
            return;

        playerArray[playerIndex] = playerObject;
    }
}
