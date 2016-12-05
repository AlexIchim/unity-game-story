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

    public void PlayerHitCheckpoint()
    {
        Debug.Log("Player hit checkpoint before infoke couritine;");
        StartCoroutine(PlayerHitCheckpointCo(LevelManager.Instance.CurrentTimeBonus));
    }
    private IEnumerator PlayerHitCheckpointCo(int bonus)
    {
        Debug.Log("Player hit checkpoint;");
        FloatingText.Show("Checkpoint!", "CheckpointText", new CenteredTextPositioner(2));
        yield return new WaitForSeconds(2f);
        FloatingText.Show(string.Format("+{0} time bonus!", bonus), "CheckpointText", new CenteredTextPositioner(2));
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

