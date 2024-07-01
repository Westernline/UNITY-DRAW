using UnityEngine;

public class MousePainter : MonoBehaviour {
    public Camera cam;
    public bool mouseSingleClick;
    public Color paintColor;
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;
    private bool eraseMode = false;
    private bool rotateMode = false;
    private Vector3 lastMousePosition;
    private Paintable rotatingPaintable;
    private float rotationSpeed = 10f; // Коефіцієнт для збільшення швидкості обертання

    void Update() {
        if (rotateMode) {
            if (Input.GetMouseButtonDown(0)) {
                lastMousePosition = Input.mousePosition;
                StartRotateObject(Input.mousePosition);
            } else if (Input.GetMouseButton(0)) {
                RotateObject(Input.mousePosition);
                lastMousePosition = Input.mousePosition;
            } else if (Input.GetMouseButtonUp(0)) {
                rotatingPaintable = null;
            }
        } else {
            if (Input.GetMouseButtonDown(0) || (!mouseSingleClick && Input.GetMouseButton(0))) {
                Paint(Input.mousePosition, eraseMode ? Color.clear : paintColor, eraseMode);
            }
        }
    }

    public void ToggleEraseMode() {
        eraseMode = !eraseMode;
    }

    public void ToggleRotateMode() {
        rotateMode = !rotateMode;
    }

    private void Paint(Vector3 screenPosition, Color color, bool erase) {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            Paintable p = hit.collider.GetComponent<Paintable>();
            if (p != null) {
                PaintManager.instance.paint(p, hit.point, radius, hardness, strength, color, erase);
            }
        }
    }

    private void StartRotateObject(Vector3 screenPosition) {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            rotatingPaintable = hit.collider.GetComponent<Paintable>();
        }
    }

    private void RotateObject(Vector3 screenPosition) {
        if (rotatingPaintable != null) {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float angleX = mouseDelta.y * rotationSpeed * Time.deltaTime;
            float angleY = -mouseDelta.x * rotationSpeed * Time.deltaTime;

            rotatingPaintable.transform.Rotate(Vector3.right, angleX, Space.World);
            rotatingPaintable.transform.Rotate(Vector3.up, angleY, Space.World);
        }
    }

    private void ResetPainting(Vector3 screenPosition) {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            transform.position = hit.point;
            Paintable p = hit.collider.GetComponent<Paintable>();
            if (p != null) {
                p.ResetToOriginalTexture();
            }
        }
    }
}
