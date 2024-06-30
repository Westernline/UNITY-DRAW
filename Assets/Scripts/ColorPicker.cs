using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public MousePainter mousePainter;
    public Slider redSlider, greenSlider, blueSlider; // Sliders for RGB values
    public Image colorDisplay; // An image component to display the selected color

    void Start()
    {
        // Initialize sliders and add listeners
        redSlider.onValueChanged.AddListener(delegate { UpdateColor(redSlider.value); });
        greenSlider.onValueChanged.AddListener(delegate { UpdateColor(greenSlider.value); });
        blueSlider.onValueChanged.AddListener(delegate { UpdateColor(blueSlider.value); });
        UpdateColor(0); // You can pass any float since it's not used
    }

    public void UpdateColor(float ignored) // Parameter added to match UnityAction<float>
    {
        Color newColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        mousePainter.paintColor = newColor; // Set the color in MousePainter
        colorDisplay.color = newColor; // Update the display color
    }
}
