using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float xSpeed;
    public float ySpeed;
    public bool backwards;
    private GameObject mainCamera;
    private Vector3 previousCameraPosition;

    void Awake()
    {
        mainCamera = GameObject.Find("MainCamera");
    }

    void Start()
    {
        previousCameraPosition = mainCamera.transform.position;
    }

    void Update()
    {
        Vector3 distance = mainCamera.transform.position - previousCameraPosition;
        float direction = backwards ? -1f : 1f;
        transform.position += Vector3.Scale(distance, new Vector3(xSpeed, ySpeed)) * direction;

        previousCameraPosition = mainCamera.transform.position;
    }
}
