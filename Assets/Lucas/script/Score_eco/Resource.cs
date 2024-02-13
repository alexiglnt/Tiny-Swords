using UnityEngine;

public abstract class Resource : MonoBehaviour
{
    public string resourceName = "GenericResource";
    public int impactOnCreation = 1;
    public int impactOnDestruction = -1;

    public abstract void OnCreated();
    public abstract void OnDestroyed();
}
