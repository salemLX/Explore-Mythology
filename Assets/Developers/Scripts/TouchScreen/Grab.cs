using UnityEngine;

public class Grab : MonoBehaviour, IGrabbable
{
    Rigidbody2D rb;
    float originalGravity;
    Vector3 grabOffset;
    public Camera arenaCamera;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void IGrabbable.Grab()
    {

        rb.linearVelocity = Vector2.zero;   
        rb.gravityScale = 0f;
    }

    public void UnGrab()
    {
        rb.gravityScale = originalGravity;
    }

    Vector2 MouseWorld()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = arenaCamera.WorldToScreenPoint(transform.position).z;
        return arenaCamera.ScreenToWorldPoint(mousePosition);
    }

    void OnMouseDown()
    {
        if (arenaCamera == null)
        {
            Debug.Log("Arena Camera is NOT ASSIGNED");
            return;
        }

        // Work out how far the click was from the item's center
        grabOffset = transform.position - GetMouseWorldPosition();
        grabOffset.z = 0;   // never shift the item toward or away from the camera
    }

    void OnMouseDrag()
    {
        if (arenaCamera == null) return;

        // Move to the mouse, but keep the same gap from the click
        transform.position = GetMouseWorldPosition() + grabOffset;
    }
    void OnMouseUp()
    {
        UnGrab();
    }
}