using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class DartThrowController : MonoBehaviour
{
    private List<Transform> activeThrownDarts = new List<Transform>();
    public Transform handTransform;
    [System.Serializable]
    public struct HandSpriteSet
    {
        public Sprite idle;
        public Sprite pulledBack;
        public Sprite throwing; 
    }
    public HandSpriteSet[] handSpriteSets;
    public Sprite[] dartSprites; 
    public Transform dartPrefab;
    public Transform dartHoldPoint;
    public float throwForceMultiplier = 8f;
    public int maxThrowsPerRound = 3;

    public int throwsThisRound = 0;
    private bool isDragging = false;
    private Transform currentDart;

    private float dragStartTime;
    private float dragEndTime; 

    private Vector3 initialHandPosition;
    private Vector3 dragStartMousePos;
    private Vector3 dragStartHandPos;

    public delegate void ThrowEvent();
    public event ThrowEvent OnDartThrown;

    void Start()
    {
        CustomizationManager.LoadSelections();
        SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].idle);
        initialHandPosition = handTransform.position;
        SpawnNewDart();
    }

    void Update()
    {
        HandleInput();
        UpdateThrownDartScales();
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
                dragStartTime = Time.time;
                dragStartMousePos = mouseWorld;
                dragStartHandPos = handTransform.position;

                SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].pulledBack);

                // Make sure dart is held tightly
                if (currentDart != null)
                {
                    currentDart.SetParent(dartHoldPoint);
                    currentDart.localPosition = Vector3.zero;
                    currentDart.localRotation = Quaternion.identity;
                }
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3 dragOffset = mouseWorld - dragStartMousePos;

            float maxVisualDrag = 2.5f;
            float clampedY = dragOffset.y < 0 ? Mathf.Clamp(dragOffset.y, -maxVisualDrag, 0) : dragOffset.y;
            Vector3 visualOffset = new Vector3(dragOffset.x, clampedY, 0);

            handTransform.position = dragStartHandPos + visualOffset;

            // Keep dart locked in the hand while dragging
            if (currentDart != null)
            {
                currentDart.position = dartHoldPoint.position;
                currentDart.rotation = dartHoldPoint.rotation;
            }
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector3 mouseWorldEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldEnd.z = 0;

            // Convert to throw direction and strength
            float dragDistance = (dragStartMousePos - mouseWorldEnd).magnitude;

            dragEndTime = Time.time;

            Vector3 dragVector = (mouseWorldEnd - dragStartMousePos);
            float dragDuration = Mathf.Max(Time.time - dragStartTime, 0.01f);
            float flickSpeed = dragVector.magnitude / dragDuration;
            float strength = Mathf.Clamp(flickSpeed, 5f, 25f); // adjust as needed

            ThrowDart(dragVector, strength);

            SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].throwing);
            Invoke(nameof(ResetHandSpriteToIdle), 1f); // Go back to idle sprite after 1 sec

            throwsThisRound++;
            OnDartThrown?.Invoke();

            StartCoroutine(ResetHandPosition());

            if (throwsThisRound < maxThrowsPerRound)
                SpawnNewDart();
        }
    }

    void ThrowDart(Vector3 dragVector, float strength)
    {
        if (currentDart == null) return;

        Rigidbody rb = currentDart.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        Vector3 throwDirection = new Vector3(dragVector.x, dragVector.y, 1f).normalized;

        float power = strength * throwForceMultiplier;
        rb.linearVelocity = throwDirection * power;

        currentDart.parent = null;
        activeThrownDarts.Add(currentDart);
        currentDart = null;

        Debug.DrawRay(rb.position, throwDirection * power, Color.red, 2f); 
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

    void SetHandSprite(Sprite sprite)
    {
        handTransform.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    void ResetHandSpriteToIdle()
    {
        SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].idle);
    }

    void SpawnNewDart()
    {
        if (dartPrefab == null || dartHoldPoint == null) return;

        Transform dart = Instantiate(dartPrefab, dartHoldPoint.position, Quaternion.identity);
        dart.SetParent(dartHoldPoint, worldPositionStays: false);
        dart.localRotation = Quaternion.identity;

        Rigidbody rb = dart.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        SpriteRenderer sr = dart.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = dartSprites[CustomizationManager.SelectedDartIndex];

        currentDart = dart;
    }

    public void ResetThrows()
    {
        throwsThisRound = 0;
    }

    void UpdateThrownDartScales()
    {
        for (int i = activeThrownDarts.Count - 1; i >= 0; i--)
        {
            Transform dart = activeThrownDarts[i];

            if (dart == null)
            {
                activeThrownDarts.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(initialHandPosition, dart.position);

            // Clamp the distance for visual scale bounds 
            float minScale = 0.07f; 
            float maxScale = 0.3f; 
            float maxDistance = 25f;

            float t = Mathf.Clamp01(distance / maxDistance);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            dart.localScale = new Vector3(scale, scale, scale); 
        }
    }
}
