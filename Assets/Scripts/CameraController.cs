using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public float zoomSpeed = 2f;
    public float minZoom = -3f;
    public float maxZoom = -15f;

    private BattlefieldBoardManager boardManager;
    private Vector3 lastMousePosition;

    void Start()
    {
        boardManager = FindObjectOfType<BattlefieldBoardManager>();
    }

    void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
    }

    void HandleCameraMovement()
    {
        // Перемещение камеры при зажатии ПКМ
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x * panSpeed * Time.deltaTime, -delta.y * panSpeed * Time.deltaTime, 0);
            transform.Translate(move, Space.World);

            lastMousePosition = Input.mousePosition;
        }

        // Ограничение перемещения камеры по границам экрана
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -panBorderThickness, boardManager.columns * boardManager.cellSize + panBorderThickness);
        pos.y = Mathf.Clamp(pos.y, -panBorderThickness, boardManager.rows * boardManager.cellSize + panBorderThickness);
        transform.position = pos;
    }

    void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Vector3 pos = transform.position;
            pos.z = Mathf.Clamp(pos.z + scroll * zoomSpeed, maxZoom, minZoom);
            transform.position = pos;
        }
    }
}
