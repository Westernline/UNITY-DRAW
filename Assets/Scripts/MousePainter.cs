using UnityEngine;

public class MousePainter : MonoBehaviour {
    public Camera cam;
    public bool mouseSingleClick;
    public Color paintColor;
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;
    private bool eraseMode = false;

    void Update() {
        if (Input.GetMouseButtonDown(0) || (!mouseSingleClick && Input.GetMouseButton(0))) {
            Paint(Input.mousePosition, eraseMode ? Color.clear : paintColor, eraseMode);  // Перемикання режимів
        }
    }

    public void ToggleEraseMode() {
        eraseMode = !eraseMode;
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
