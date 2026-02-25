using UnityEngine;

public class ScanMaskFollow : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // khoảng cách tới camera

        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0);
    }
}
