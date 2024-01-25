using UnityEngine;

public class MineController : MonoBehaviour
{
    public int health = 5; // Use health as a hit counter
    public Animator animator;
    public GameObject goldPrefab; // Prefab to spawn when gold is produced

    public bool IsAlive => health > 0;

    public void ReceiveDamage()
    {
        health--;
        if (IsAlive)
        {
            // Mine is still intact, play hit animation
            animator.Play("Producing");
        }
        else
        {
            // Mine is depleted
            ProduceGold();
        }
    }

    public void PlayIdleAnimation()
    {
        animator.Play("Idle");
    }

    private void ProduceGold()
    {
        Vector3 goldPosition = transform.position;
        goldPosition.y -= 1f;
        Instantiate(goldPrefab, goldPosition, Quaternion.identity); // Spawn gold prefab

    }
}
