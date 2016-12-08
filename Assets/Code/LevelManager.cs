using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/* Singleton - Globally*/
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Player Player { get; private set; }
    public CameraController Camera { get; private set; }

    /* Time since we started game. */
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }

    public int CurrentTimeBonus
    {
        get
        {
            var secondDifference = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDifference) * BonsuSecondMultiplier;
        }
    }



    private List<Checkpoint> _checkpoints;
    private int _currentCheckpointIndex;
    private DateTime _started;
    private int _savePoints;


    public Checkpoint DebugSpawn;
    public int BonusCutoffSeconds;
    public int BonsuSecondMultiplier;

    public void Awake()
    {
        _savePoints = GameManager.Instance.Points;
        Instance = this;
    }

    public void Start()
    {
        _started = DateTime.UtcNow;
        _checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t=>t.transform.position.x).ToList();
        _currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;

        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraController>();

        /*Handling stars spawning at start of game. */
        var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (var listener in listeners)
        {
            for (var i = _checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;

                _checkpoints[i].AssignObjectToCheckpoint(listener);
                break;
            }
        }
#if UNITY_EDITOR
        /* If we set a debug spawn this will spawn the player, else the checkpoint.*/
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#else
        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#endif
    }

    public void Update()
    {
        var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
        if (isAtLastCheckpoint)
            return;

        var distanceToNextCheckpoint = _checkpoints[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckpoint > 0)
            return;

        _checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();
        _currentCheckpointIndex++;
        _checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();

        //todo : TIME BONUS
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savePoints = GameManager.Instance.Points;
        _started = DateTime.UtcNow;

        
    }

    public void GotoNextLevel(string levelName)
    {
        StartCoroutine(GotoNextLevelCo(levelName));
    }

    private IEnumerator GotoNextLevelCo(string levelName)
    {
        Player.FinishLevel();
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        FloatingText.Show(string.Format("Level Complete !", GameManager.Instance.Points), "CheckpointText", new CenteredTextPositioner(4f));
        yield return new WaitForSeconds(1f);

        FloatingText.Show(string.Format("{0} points!", GameManager.Instance.Points), "CheckpointText", new CenteredTextPositioner(1f));
        yield return new WaitForSeconds(5f);

        if (string.IsNullOrEmpty(levelName))
            Application.LoadLevel("StartScreen");
        else
            Application.LoadLevel(levelName);
    }

    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }

    private IEnumerator KillPlayerCo()
    {
        Player.Kill();
        Camera.IsFollowing = false;
        yield return new WaitForSeconds(2f);

        Camera.IsFollowing = true;

        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);

        //TOD: points
        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savePoints);
    }

}
