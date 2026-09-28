using UnityEngine;
using TMPro;

public class HitFeedbackUI : MonoBehaviour
{
    public static HitFeedbackUI Instance;

    public TextMeshPro hitText;

    [Header("Timing")]
    public float showDuration = 0.6f;
    public float fadeDuration = 0.3f;

    [Header("Colors")]
    public Color headColor = Color.red;
    public Color bodyColor = Color.white;

    [Header("Placement")]
    public bool billboard = true;

    private float timer;
    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;

        if (hitText == null)
            hitText = GetComponentInChildren<TextMeshPro>();

        if (hitText == null)
        {
            Debug.LogError("HitFeedbackUI finner ingen TextMeshPro!");
            return;
        }

        SetAlpha(0f);
    }

    public void ShowHit(BodyPart part)
    {
        switch (part)
        {
            case BodyPart.Head:
                hitText.text = "HEADSHOT";
                hitText.color = headColor;
                break;
            case BodyPart.Neck:
                hitText.text = "NECK HIT";
                hitText.color = headColor;
                break;
            case BodyPart.Chest:
                hitText.text = "CHEST HIT";
                hitText.color = bodyColor;
                break;
            case BodyPart.Heart:
                hitText.text = "HEART HIT";
                hitText.color = bodyColor;
                break;
            case BodyPart.Stomach:
                hitText.text = "STOMACH HIT";
                hitText.color = bodyColor;
                break;
            case BodyPart.RightArm:
                hitText.text = "RIGHT ARM HIT";
                hitText.color = bodyColor;
                break;
            case BodyPart.LeftArm:
                hitText.text = "LEFT ARM HIT";
                hitText.color = bodyColor;
                break;
            case BodyPart.RightLeg:
                hitText.text = "RIGHT LEG HIT";
                hitText.color = bodyColor;
                break;
            case BodyPart.LeftLeg:
                hitText.text = "LEFT LEG HIT";
                hitText.color = bodyColor;
                break;
        }

        SetAlpha(1f);
        timer = showDuration + fadeDuration;
    }

    public void ShowHit(BodyPart part, Vector3 worldPosition)
    {
        hitText.transform.position = worldPosition + Vector3.up * 0.5f;
        ShowHit(part);
    }

    void Update()
    {
        if (hitText == null) return;

        if (billboard && cam != null)
            hitText.transform.rotation = cam.transform.rotation;

        if (timer <= 0f) return;

        timer -= Time.deltaTime;

        if (timer < fadeDuration)
            SetAlpha(Mathf.Clamp01(timer / fadeDuration));
    }

    void SetAlpha(float a)
    {
        Color c = hitText.color;
        c.a = a;
        hitText.color = c;
    }
}