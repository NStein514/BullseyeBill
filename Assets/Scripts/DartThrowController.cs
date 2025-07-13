using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartThrowController : MonoBehaviour
{
    private List<Transform> activeThrownDarts = new List<Transform>();
    public Transform handTransform;
    public Transform dartSpawnPoint;

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
    private bool dartHasBeenThrown = false;
    private Transform currentDart;

    private float dragStartTime;
    private Vector3 dragStartMousePos;
    private Vector3 dragStartHandPos;
    private Vector3 initialHandPosition;

    public delegate void ThrowEvent();
    public event ThrowEvent OnDartThrown;

    private bool canGrabDart = false;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        CustomizationManager.LoadSelections();
        SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].idle);

        initialHandPosition = handTransform.position;
        SpawnNewDart();
    }

    void Update()
    {
        UpdateHandPosition();
        HandleInput();
        UpdateThrownDartScales();
    }

    public void StartNewRound()
    {
        ResetThrows();
        SpawnNewDart();
    }

    void UpdateHandPosition()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        handTransform.position = mouseWorld;
    }

    void HandleInput()
    {
        if (throwsThisRound >= maxThrowsPerRound || currentDart == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = dartSpawnPoint.position.z;

            Collider[] hits = Physics.OverlapSphere(mouseWorld, 3.0f); 
            foreach (Collider hit in hits)
            {
                if (hit.transform == currentDart || hit.transform.IsChildOf(currentDart))
                {
                    isDragging = true;
                    dragStartTime = Time.time;
                    dragStartMousePos = handTransform.position;
                    dragStartHandPos = handTransform.position;

                    SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].pulledBack);

                    currentDart.SetParent(handTransform);
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

            Vector3 dragVector = (mouseWorldEnd - dragStartMousePos);
            float dragDuration = Mathf.Max(Time.time - dragStartTime, 0.01f);
            float flickSpeed = dragVector.magnitude / dragDuration;
            float strength = Mathf.Clamp(flickSpeed, 5f, 25f);

            ThrowDart(dragVector, strength);
        }
    }

    void ThrowDart(Vector3 dragVector, float strength)
    {
        if (currentDart == null) return;

        Rigidbody rb = currentDart.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        Vector3 throwDirection = new Vector3(dragVector.x, dragVector.y, 1f).normalized;
        float power = strength * throwForceMultiplier;
        rb.linearVelocity = throwDirection * power;

        currentDart.SetParent(null);
        activeThrownDarts.Add(currentDart);
        currentDart = null;

        dartHasBeenThrown = true;
        throwsThisRound++;

        OnDartThrown?.Invoke();

        SetHandSprite(handSpriteSets[CustomizationManager.SelectedHandIndex].throwing);
        Invoke(nameof(ResetHandSpriteToIdle), 1f);

        StartCoroutine(ResetHandPosition());

        if (throwsThisRound < maxThrowsPerRound)
        {
            StartCoroutine(SpawnNextDartAfterDelay(0.25f));
        }
    }

    IEnumerator SpawnNextDartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewDart();
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
        if (dartPrefab == null || dartSpawnPoint == null) return;

        Transform dart = Instantiate(dartPrefab, dartSpawnPoint.position, Quaternion.identity);
        dart.SetParent(null);
        dart.localRotation = Quaternion.identity;

        Rigidbody rb = dart.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = dart.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        SpriteRenderer sr = dart.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = dartSprites[CustomizationManager.SelectedDartIndex];

        StartCoroutine(DelayedSetCurrentDart(dart));
    }

    IEnumerator DelayedSetCurrentDart(Transform dart)
    {
        canGrabDart = false;
        yield return new WaitForFixedUpdate();
        currentDart = dart;
        yield return null;
        canGrabDart = true;
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
            float minScale = 0.07f;
            float maxScale = 0.3f;
            float maxDistance = 25f;

            float t = Mathf.Clamp01(distance / maxDistance);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            dart.localScale = new Vector3(scale, scale, scale);
        }
    }
}
