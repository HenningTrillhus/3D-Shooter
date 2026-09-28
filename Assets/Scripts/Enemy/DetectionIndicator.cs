using UnityEngine;
using UnityEngine.UI;

public class DetectionIndicator : MonoBehaviour
{
    [Header("Referanser")]
    public EnemyVision vision;
    public GameObject indicatorRoot;   // objektet over hodet som skrus av/på
    public Renderer indicatorRenderer; // bruk denne hvis indikatoren er en 3D-ting (Quad, Sphere osv.)
    public Renderer indicatorRenderer2; // bruk denne hvis indikatoren er en 3D-ting (Quad, Sphere osv.)
    public Graphic indicatorGraphic;   // bruk denne hvis den er en UI Image på en World Space Canvas

    [Header("Farger")]
    public Color lowColor = Color.green;
    public Color midColor = Color.yellow;
    public Color highColor = Color.red;

    [Header("Oppførsel")]
    public float showThreshold = 0.5f;  // Detection over dette gjør indikatoren synlig
    public bool billboard = true;       // vend alltid mot kameraet
    public bool scaleWithDetection = false;
    public float minScale = 0.5f;
    public float maxScale = 1.2f;

    private Camera cam;
    private Vector3 baseScale;

    void Start()
    {
        cam = Camera.main;

        if (vision == null) vision = GetComponent<EnemyVision>();

        if (indicatorRoot != null)
        {
            baseScale = indicatorRoot.transform.localScale;
            indicatorRoot.SetActive(false);
        }
    }

    void Update()
    {
        if (vision == null || indicatorRoot == null) return;

        float d = vision.Detection;
        bool show = d > showThreshold;

        if (indicatorRoot.activeSelf != show)
            indicatorRoot.SetActive(show);

        if (!show) return;

        // 0-100 -> 0-1, grønn til gul til rød
        float t = d / 100f;
        Color c = t < 0.5f
            ? Color.Lerp(lowColor, midColor, t * 2f)
            : Color.Lerp(midColor, highColor, (t - 0.5f) * 2f);

        if (indicatorRenderer != null) indicatorRenderer.material.color = c;
        if (indicatorRenderer2 != null) indicatorRenderer2.material.color = c;
        if (indicatorGraphic != null) indicatorGraphic.color = c;

        if (scaleWithDetection)
            indicatorRoot.transform.localScale = baseScale * Mathf.Lerp(minScale, maxScale, t);

        if (billboard && cam != null)
            indicatorRoot.transform.rotation = cam.transform.rotation;
    }
}