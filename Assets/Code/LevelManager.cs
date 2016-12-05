using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

/* Singleton - Globally*/
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Player Player { get; private set; }
    public CameraController Camera { get; private set; }


    public void Awake()
    {

    }

    public void Start()
    {

    }

    public void Update()
    {

    }

    public void KillPlayer()
    {

    }

    private IEnumerator KillPlayerCo()
    {
        yield break;
    }

}
