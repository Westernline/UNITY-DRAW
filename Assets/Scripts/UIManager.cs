using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UIManager : MonoBehaviour
{
    public Paintable[] paintables;

    // UI Elements
    public Button saveButton;
    public Button loadButton;
    public Button resetButton;
    public Button addLayerButton;
    public Button toggleEraseModeButton; // Нова кнопка для перемикання режимів
    public Slider radiusSlider;
    public Slider strengthSlider;
    public Slider hardnessSlider;

    // Image to visualize radius, strength, and hardness
    public Image brushPreviewImage;

    // MousePainter reference
    public MousePainter mousePainter;

    void Start()
    {
        // Assign button click events
        saveButton.onClick.AddListener(SaveTexture);
        loadButton.onClick.AddListener(LoadTexture);
        resetButton.onClick.AddListener(ResetAllPaintables);
        toggleEraseModeButton.onClick.AddListener(ToggleEraseMode); // Додаємо слухач подій для нової кнопки

        // Assign slider value change events
        radiusSlider.onValueChanged.AddListener(ChangeRadius);
        strengthSlider.onValueChanged.AddListener(ChangeStrength);
        hardnessSlider.onValueChanged.AddListener(ChangeHardness);

        // Initialize sliders with current values
        radiusSlider.value = mousePainter.radius;
        strengthSlider.value = mousePainter.strength;
        hardnessSlider.value = mousePainter.hardness;

        // Update image to initial values
        UpdateBrushPreviewImage();
    }


    public void ChangeRadius(float value)
    {
        mousePainter.radius = value;
        UpdateBrushPreviewImage();
    }

    public void ChangeStrength(float value)
    {
        mousePainter.strength = value;
        UpdateBrushPreviewImage();
    }

    public void ChangeHardness(float value)
    {
        mousePainter.hardness = value;
        UpdateBrushPreviewImage();
    }

    private void UpdateBrushPreviewImage()
    {
        // Update the size of the brush preview based on the radius
        float newSize = mousePainter.radius * 100; // Assuming radius is normalized [0,1]
        brushPreviewImage.rectTransform.sizeDelta = new Vector2(newSize, newSize);

        // Update the transparency of the brush preview based on the strength
        Color color = brushPreviewImage.color;
        color.a = mousePainter.strength; // Assuming strength is normalized [0,1]
        brushPreviewImage.color = color;

        // Update the sharpness of the edges based on the hardness
        // Assuming you have a shader that supports hardness
        Material material = brushPreviewImage.material;
        if (material != null)
        {
            material.SetFloat("_Hardness", mousePainter.hardness); // Assuming hardness is normalized [0,1]
        }

        // Update the color of the brush preview
        brushPreviewImage.color = new Color(mousePainter.paintColor.r, mousePainter.paintColor.g, mousePainter.paintColor.b, color.a);
    }

    public void SaveTexture()
    {
        foreach (var paintable in paintables)
        {
            if (paintable != null)
            {
                Texture2D maskTexture = ToTexture2D(paintable.getMask());
                Texture2D supportTexture = ToTexture2D(paintable.getSupport());

                SaveTextureToBinary(supportTexture, paintable.name + "_support_texture.dat");
                SaveTextureToBinary(maskTexture, paintable.name + "_mask_texture.dat");
            }
        }
    }

    public void LoadTexture()
    {
        foreach (var paintable in paintables)
        {
            if (paintable != null)
            {
                LoadAndApplyTexture(paintable.getSupport(), paintable, "_MainTex");
                LoadAndApplyTexture(paintable.getMask(), paintable, "_MaskTexture");
            }
        }
    }
    
    public void ResetAllPaintables()
    {
        foreach (var paintable in paintables)
        {
            if (paintable != null)
            {
                paintable.ResetTextures();
            }
        }
    }

    private void LoadAndApplyTexture(RenderTexture renderTexture, Paintable paintable, string shaderPropertyID)
    {
        string textureFileName = shaderPropertyID == "_MainTex" ? "_support_texture.dat" : "_mask_texture.dat";
        string path = Path.Combine(Application.persistentDataPath, paintable.name + textureFileName);
        Texture2D texture = LoadTextureFromBinary(path);
        if (texture != null)
        {
            ApplyTextureToRenderTexture(texture, renderTexture);
            paintable.getRenderer().material.SetTexture(shaderPropertyID, renderTexture);
        }
        else
        {
            Debug.LogError("Failed to load texture: " + path);
        }
    }

    private static Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private static void SaveTextureToBinary(Texture2D texture, string filename)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, filename);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(fileStream, textureBytes);
        }
        Debug.Log("Texture saved to " + path);
    }

    private static Texture2D LoadTextureFromBinary(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                byte[] textureBytes = formatter.Deserialize(fileStream) as byte[];
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(textureBytes);
                return texture;
            }
        }
        else
        {
            Debug.LogError("File not found at " + path);
            return null;
        }
    }

    private void ApplyTextureToRenderTexture(Texture2D texture, RenderTexture renderTexture)
    {
        RenderTexture.active = renderTexture;
        Graphics.Blit(texture, renderTexture);
    }

    private void ToggleEraseMode()
    {
        mousePainter.ToggleEraseMode();
    }
}
