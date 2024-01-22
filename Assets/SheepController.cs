using UnityEngine;

public class SheepController : MonoBehaviour
{
    public int health = 3; // Example health
    public Animator animator;
    public GameObject meatPrefab; // Prefab to spawn when sheep is killed

    public bool IsAlive => health > 0;

    public void ReceiveDamage()
    {
        health--;
        if (IsAlive)
        {
            // Tree is still alive, play hit animation
            animator.Play("HitSheep");
        }
        else
        {
            // Tree is chopped down
            Killed();
        }
    }

    public void PlayIdleAnimation()
    {
        animator.Play("Idle");
    }

    private void Killed()
    {
        //animator.Play("Death"); // Play death animation
        Instantiate(meatPrefab, transform.position, Quaternion.identity); // Spawn meat prefab
        // Optionally destroy the sheep object after a delay
        Destroy(gameObject); // Adjust delay as needed
    }
}
