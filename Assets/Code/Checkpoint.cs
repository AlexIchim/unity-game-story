using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private List<IPlayerRespawnListener> _listeners;

    public void Awake()
    {
        _listeners = new List<IPlayerRespawnListener>();
    }

    private IEnumerator PlayerHitCheckpointCo(int bonus)
    {
        yield break;
    }

    public void PlayerLeftCheckpoint()
    {

    }

    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform);
        foreach (var listener in _listeners)
            listener.OnPlayerRespawnInThisCheckpoint(this, player);
    }

    public void AssignObjectToCheckpoint(IPlayerRespawnListener listener)
    {
        _listeners.Add(listener);
    }
}

