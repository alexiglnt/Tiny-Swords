public class Tree : Resource
{

    void Start()
    {
        OnCreated();
    }

    public override void OnCreated()
    {
        EcosystemScoreManager.Instance.ChangeScore(impactOnCreation);
    }

    void OnDestroy()
    {
        OnDestroyed();
    }

    public override void OnDestroyed()
    {
        EcosystemScoreManager.Instance.ChangeScore(impactOnDestruction);
    }
}
