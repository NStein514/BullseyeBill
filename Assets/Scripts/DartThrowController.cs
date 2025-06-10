using UnityEngine;
using System.Collections;
using System.Reflection;

public class DartThrowController : MonoBehaviour
{
    public Transform handTransform;         // The hand sprite
    public Transform dartPrefab;            // Prefab of the dart to throw
    public Transform dartHoldPoint;         // Where dart sits in hand
    public float throwForceMultiplier = 10f;
    public int maxThrowsPerRound = 3;

    public int throwsThisRound = 0;
    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private Transform currentDart;

    private Vector3 initialHandPosition;
    private Vector3 dragStartMousePos;
    private Vector3 dragStartHandPos; 

    public delegate void ThrowEvent();
    public event ThrowEvent OnDartThrown;

    void Start()
    {
        initialHandPosition = handTransform.position;
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

            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
            if (hit != null && (hit.transform == handTransform || hit.transform == currentDart))
            {
                isDragging = true;
                dragStartMousePos = mouseWorld;
                dragStartHandPos = handTransform.position;
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3 dragOffset = mouseWorld - dragStartMousePos;

            // Clamp how far back it can go on the Y axis
            float maxDragBack = 2.5f;
            dragOffset.y = Mathf.Clamp(dragOffset.y, -maxDragBack, 0);

            handTransform.position = dragStartHandPos + new Vector3(dragOffset.x, dragOffset.y, 0);
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3 throwDir = (dragStartMousePos - mouseWorld).normalized;
            float dragDistance = Vector3.Distance(dragStartMousePos, mouseWorld);

            ThrowDart(throwDir, dragDistance);
            throwsThisRound++;
            OnDartThrown?.Invoke();

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

        Rigidbody rb = currentDart.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        // Manual override: strong upward + forward arc
        Vector3 throwDirection = new Vector3(direction.x * 0.5f, 1.5f, 2f).normalized;

        // Apply a high enough force for visible arc
        float power = strength * throwForceMultiplier;
        Vector3 throwVelocity = throwDirection * power;

        rb.linearVelocity = throwVelocity;

        currentDart.parent = null;
        currentDart = null;

        Debug.DrawRay(rb.position, throwVelocity, Color.red, 2f); // visualize
    }

    IEnumerator ResetHandPosition()
    {
        Vector3 startPos = handTransform.position;
        Vector3 targetPos = initialHandPosition;
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
        dart.SetParent(dartHoldPoint);
        dart.localPosition = Vector3.zero;

        Rigidbody rb = dart.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false; 

        currentDart = dart;
    }

    public void ResetThrows()
    {
        throwsThisRound = 0;
    }
}
