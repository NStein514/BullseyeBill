using UnityEngine;

public class DartThrowController : MonoBehaviour
{
    [Header("References")]
    public GameObject dartPrefab;
    public Transform spawnPoint;
    public Camera mainCamera;
    public GameObject handSprite;

    [Header("Throw Settings")]
    public float throwForceMultiplier = 0.1f;
    public float maxPullDistance = 200f;
    public int maxThrowsPerRound = 3;

    private GameObject currentDart;
    private Rigidbody currentRb;
    private Vector2 dragStartPos;
    private Vector2 lastMousePosition;
    private Vector2 mouseVelocity;
    private bool isDragging = false;
    private int throwsUsed = 0;
    private bool dartThrown = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        SpawnNewDart();
    }

    void Update()
    {
        if (dartThrown || currentDart == null || throwsUsed >= maxThrowsPerRound)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            lastMousePosition = dragStartPos;
            isDragging = true;

            currentRb.isKinematic = true;
            currentDart.transform.position = spawnPoint.position;
            handSprite.SetActive(true);
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f));
            handSprite.transform.position = mouseWorldPos;

            currentDart.transform.position = spawnPoint.position;

            mouseVelocity = ((Vector2)Input.mousePosition - lastMousePosition) / Time.deltaTime;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            dartThrown = true;
            throwsUsed++;

            float pullDistance = Mathf.Clamp((dragStartPos - (Vector2)Input.mousePosition).magnitude, 0f, maxPullDistance);
            Vector3 throwDirection = mainCamera.transform.forward + mainCamera.transform.up * 0.2f;
            float force = pullDistance * throwForceMultiplier + mouseVelocity.magnitude * 0.0003f;

            currentRb.isKinematic = false;
            currentRb.AddForce(throwDirection.normalized * force, ForceMode.Impulse);

            handSprite.SetActive(false);

            Invoke(nameof(SpawnNewDart), 1.0f); // delay to simulate flight time

            // Optional: notify RoundManager
            // FindObjectOfType<RoundManager>()?.OnDartThrown(throwsUsed);
        }
    }

    void SpawnNewDart()
    {
        if (throwsUsed >= maxThrowsPerRound)
            return;

        GameObject newDart = Instantiate(dartPrefab, spawnPoint.position, Quaternion.identity);
        currentDart = newDart;
        currentRb = newDart.GetComponent<Rigidbody>();
        currentRb.isKinematic = true;
        dartThrown = false;
    }

    public int GetThrowsUsed() => throwsUsed;

    public void ResetThrows()
    {
        throwsUsed = 0;
        Destroy(currentDart);
        SpawnNewDart();
    }
}
