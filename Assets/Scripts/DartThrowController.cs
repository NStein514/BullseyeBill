using Unity.VisualScripting;
using UnityEngine;

public class DartThrowController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody dartRb;
    public Camera mainCamera;
    public Transform dartHoldPoint;
    public GameObject handSprite; 

    [Header("Throw Settings")]
    public float throwForceMultiplier = 0.1f;
    public float maxPullDistance = 200f;
    public int maxThrowsPerRound = 3;

    private Vector2 dragStartPos;
    private bool isDragging = false;
    private int throwsUsed = 0;
    private bool dartThrown = false;

    private Vector2 lastMousePosition;
    private Vector2 mouseVelocity; 

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
            lastMousePosition = dragStartPos;

            // Attach dart to hand
            dartRb.isKinematic = true;
            transform.position = dartHoldPoint.position;
            handSprite.SetActive(true);
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f));
            handSprite.transform.position = mouseWorldPos;

            transform.position = dartHoldPoint.position;

            // Track mouse velocity 
            mouseVelocity = ((Vector2)Input.mousePosition - lastMousePosition) / Time.deltaTime;
            lastMousePosition = Input.mousePosition;
        }

        // On Release 
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            dartThrown = true;
            throwsUsed++;

            float pullDistance = Mathf.Clamp((dragStartPos - (Vector2)Input.mousePosition).magnitude, 0f, maxPullDistance);
            Vector3 throwDirection = mainCamera.transform.forward + mainCamera.transform.up * 0.2f;

            float forceMagnitude = pullDistance * throwForceMultiplier + mouseVelocity.magnitude * 0.0003f;

            dartRb.isKinematic = false;
            dartRb.AddForce(throwDirection.normalized * forceMagnitude, ForceMode.Impulse);

            handSprite.SetActive(false); 
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
        dartRb.isKinematic = true;

        transform.position = dartHoldPoint.position; 
        transform.rotation = Quaternion.identity;

        dartRb.Sleep();

        dartThrown = false; 
    }

    public int GetThrowsUsed() => throwsUsed;

    public void ResetThrows()
    {
        throwsUsed = 0;
        ResetDart(); 
    }
}
