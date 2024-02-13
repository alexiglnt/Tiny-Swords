using UnityEngine;

public class EcosystemScoreManager : MonoBehaviour
{
    public static EcosystemScoreManager Instance;
    private int score;
    private bool canAffectScore = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScore(int amount)
    {
        if (canAffectScore)
        {
            score += amount;
            Debug.Log("Current Ecosystem Score: " + score);
        }
    }

    public void EnableScoring()
    {
        canAffectScore = true;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
