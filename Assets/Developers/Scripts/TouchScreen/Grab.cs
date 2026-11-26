using UnityEngine;

public class Grab : MonoBehaviour, IGrabbable
{
     [SerializeField] private bool isGrabbed = true;
    Rigidbody2D rb;

    public void UnGrab()
    {
        
    }

    void IGrabbable.Grab()
    {
        isGrabbed = true;
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
