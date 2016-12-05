using System;
using UnityEngine;

public class PointStar : MonoBehaviour, IPlayerRespawnListener
{
    public GameObject Effect;
    public int PointsToAdd = 10;

    

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        GameManager.Instance.AddPoints(PointsToAdd);
        Instantiate(Effect, transform.position, transform.rotation);

        gameObject.SetActive(false);

        //Show floating text after collecting stars
        FloatingText.Show(string.Format("+{0}!", PointsToAdd), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1f, 2));
    }

    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
}