using UnityEngine;
using System.Collections;

public class DartThrowController : MonoBehaviour
{
    public Transform handTransform;      // The hand sprite
    public Transform dartPrefab;         // Prefab of the dart to throw
    public Transform dartHoldPoint;      // Where dart sits in hand
    public float throwForceMultiplier = 10f;
    public int maxThrowsPerRound = 3;

    public int throwsThisRound = 0;
    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private Transform currentDart;

    public delegate void ThrowEvent();
    public event ThrowEvent OnDartThrown;

    void Start()
    {
        SpawnNewDart();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (throwsThisRound >= maxThrowsPerRound) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            // Check if mouse is on the bullseye or dart hold point (simple distance check)
            if (Vector3.Distance(mouseWorld, handTransform.position) < 1f)
            {
                isDragging = true;
                dragStartPos = mouseWorld;
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            // Move hand with mouse, but limit how far back it can go on Y (drag back)
            Vector3 offset = mouseWorld - dragStartPos;
            float maxDragBack = 2.5f;
            Vector3 newPos = handTransform.position + new Vector3(offset.x, Mathf.Clamp(offset.y, -maxDragBack, 0), 0);
            handTransform.position = newPos;

            dragStartPos = mouseWorld; // update for smooth movement
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEndPos.z = 0;

            Vector3 throwDir = (dragStartPos - dragEndPos).normalized; // direction forward and up
            float dragDistance = Vector3.Distance(dragStartPos, dragEndPos);

            ThrowDart(throwDir, dragDistance);
            throwsThisRound++;
            OnDartThrown?.Invoke();

            // Reset hand position (optional smooth reset)
            StartCoroutine(ResetHandPosition());

            if (throwsThisRound < maxThrowsPerRound)
            {
                SpawnNewDart();
            }
        }
    }

    void ThrowDart(Vector3 direction, float strength)
    {
        if (currentDart == null) return;

        Rigidbody2D rb = currentDart.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 1f;

        Vector2 throwVelocity = new Vector2(direction.x, direction.y + 0.5f).normalized * strength * throwForceMultiplier;
        rb.linearVelocity = throwVelocity;

        currentDart.parent = null; // detach from hand
        currentDart = null;
    }

    IEnumerator ResetHandPosition()
    {
        Vector3 startPos = handTransform.position;
        Vector3 targetPos = dartHoldPoint.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            handTransform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    void SpawnNewDart()
    {
        Transform dart = Instantiate(dartPrefab, dartHoldPoint.position, Quaternion.identity);
        dart.SetParent(handTransform);
        dart.localPosition = Vector3.zero;

        Rigidbody2D rb = dart.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0;

        currentDart = dart;
    }

    public void ResetThrows()
    {
        throwsThisRound = 0;
    }
}
