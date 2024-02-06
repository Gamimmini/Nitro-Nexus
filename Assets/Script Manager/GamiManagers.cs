using UnityEngine;

public class GamiManagers : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.LoadComponents();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Reset()
    {
        this.LoadComponents();
    }
    protected virtual void LoadComponents()
    {

    }
}
