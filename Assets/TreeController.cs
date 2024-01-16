using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public int health = 5; // Example health
    public Animator animator;
    public GameObject woodPrefab;

    public bool IsAlive => health > 0;

    public void ReceiveDamage()
    {
        health--;
        if (IsAlive)
        {
            // Tree is still alive, play hit animation
            animator.Play("Hit");
        }
        else
        {
            // Tree is chopped down
            ChoppedDown();
        }
    }

    public void PlayIdleAnimation()
    {
        animator.Play("Idle");
    }

    public void ChoppedDown()
    {
        animator.Play("Chopped");
        Instantiate(woodPrefab, transform.position, Quaternion.identity); // Spawn wood prefab
        Destroy(GetComponent<Collider2D>());//remove the collider 
        Destroy(gameObject, 10f); // Destroy the tree after 10 second
    }
}
