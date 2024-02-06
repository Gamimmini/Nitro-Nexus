using UnityEngine;

public class MenuCamControl : MonoBehaviour
{
    [SerializeField] Transform currentMount;
    [SerializeField] float speedFactor = 0.1f;
  
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentMount.position, speedFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentMount.rotation, speedFactor);
    }

    public void setMount(Transform newMount)
    {
        currentMount = newMount;
    }
}
