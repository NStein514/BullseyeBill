using Unity.VisualScripting;
using UnityEngine;

public class DartThrowController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody dartRb;
    public Camera mainCamera;

    [Header("Throw Settings")]
    public float throwForceMultiplier = 15f;
    public float maxPullDistance = 200f;
    public int maxThrowsPerRound = 3;

    private Vector2 dragStartPos;
    private bool isDragging = false;
    private int throwsUsed = 0;
    private bool dartThrown = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (dartRb == null)
            dartRb = GetComponent<Rigidbody>();

        ResetDart();
    }

    void Update()
    {
        if (dartThrown || throwsUsed >= maxThrowsPerRound)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 dragEndPos = Input.mousePosition;
            Vector2 dragVector = dragStartPos - dragEndPos; // Pull bad is positive 

            float pullDistance = Mathf.Clamp(dragVector.magnitude, 0, maxPullDistance);
            Vector3 forceDirection = mainCamera.transform.forward + mainCamera.transform.up * (dragVector.y / maxPullDistance);

            ThrowDart(forceDirection.normalized * pullDistance * throwForceMultiplier);
            isDragging = false;
        }
    }

    void ThrowDart(Vector3 force)
    {
        dartRb.isKinematic = false;
        dartRb.AddForce(force, ForceMode.Impulse);
        dartThrown = true;
        throwsUsed++;

        //FindObjectOfType<RoundManager>()?.OnDartThrown(throwsUsed);
    }

    public void ResetDart()
    {
        transform.position = new Vector3(0, 1, -5);
        transform.rotation = Quaternion.identity;

        dartRb.linearVelocity = Vector3.zero;
        dartRb.angularVelocity = Vector3.zero;
        dartRb.isKinematic = true;

        dartThrown = false;
    }

    public int GetThrowsUsed() => throwsUsed;

    public void ResetThrows()
    {
        throwsUsed = 0;
        ResetDart(); 
    }
}
