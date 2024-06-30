using UnityEngine;

public class Paintable : MonoBehaviour {
    public const int TEXTURE_SIZE = 1024;

    public float extendsIslandOffset = 1;

    private RenderTexture extendIslandsRenderTexture;
    private RenderTexture uvIslandsRenderTexture;
    private RenderTexture maskRenderTexture;
    private RenderTexture supportTexture;
    private RenderTexture originalTexture;

    private Renderer rend;

    private int maskTextureID = Shader.PropertyToID("_MaskTexture");
    private int mainTexID = Shader.PropertyToID("_MainTex");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        supportTexture.filterMode = FilterMode.Bilinear;

        originalTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        Graphics.Blit(supportTexture, originalTexture); // Копіювання поточної текстури як оригінальної
        Graphics.Blit(supportTexture, maskRenderTexture); // Копіювання поточної текстури як оригінальної

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        PaintManager.instance.initTextures(this);

        // Додаємо посилання на текстуру для _MainTex
        rend.material.SetTexture(mainTexID, supportTexture);
    }

    void OnDisable() {
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
    }

    public void ResetTextures() {
        GL.PushMatrix();
        RenderTexture.active = maskRenderTexture;
        GL.Clear(true, true, Color.clear);

        RenderTexture.active = supportTexture;
        GL.Clear(true, true, Color.clear);
        GL.PopMatrix();

        if (rend != null) {
            rend.material.SetTexture("_MainTex", supportTexture);
            rend.material.SetTexture("_MaskTexture", maskRenderTexture);
        }
    }

    public void ResetToOriginalTexture() {
        Graphics.Blit(originalTexture, maskRenderTexture);
        rend.material.SetTexture("_MaskTexture", maskRenderTexture);
        rend.material.SetTexture("_MainTex", supportTexture);
    }
}
