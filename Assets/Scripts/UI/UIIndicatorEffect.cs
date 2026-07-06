using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIIndicatorEffect : MonoBehaviour
{
    public enum EffectType
    {
        Blink,      // Efek berkedip (memudarkan opacity)
        Bob,        // Efek memantul naik-turun atau kiri-kanan
        Both        // Kedua efek aktif bersamaan
    }

    [Header("Effect Settings")]
    [SerializeField] private EffectType m_effectType = EffectType.Blink;
    [SerializeField] private float m_speed = 5f; // Kecepatan efek

    [Header("Blink Settings")]
    [Range(0f, 1f)] [SerializeField] private float m_minAlpha = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float m_maxAlpha = 1f;

    [Header("Bob Settings")]
    [SerializeField] private float m_bobAmount = 5f; // Jarak pantulan dalam unit pixel
    [SerializeField] private bool m_verticalBob = true; // true = naik-turun, false = kiri-kanan

    private CanvasGroup m_canvasGroup;
    private Image m_image;
    private TextMeshProUGUI m_tmpText;
    private Vector3 m_startPosition;

    private void Start()
    {
        m_startPosition = transform.localPosition;

        // Mendeteksi komponen yang ada untuk diubah opacity-nya
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_image = GetComponent<Image>();
        m_tmpText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // 1. Logika Berkedip (Blink)
        if (m_effectType == EffectType.Blink || m_effectType == EffectType.Both)
        {
            float lerpVal = (Mathf.Sin(Time.time * m_speed) + 1f) / 2f; // Mengubah rentang -1..1 menjadi 0..1
            float targetAlpha = Mathf.Lerp(m_minAlpha, m_maxAlpha, lerpVal);

            if (m_canvasGroup != null)
            {
                m_canvasGroup.alpha = targetAlpha;
            }
            else if (m_image != null)
            {
                Color c = m_image.color;
                m_image.color = new Color(c.r, c.g, c.b, targetAlpha);
            }
            else if (m_tmpText != null)
            {
                Color c = m_tmpText.color;
                m_tmpText.color = new Color(c.r, c.g, c.b, targetAlpha);
            }
        }

        // 2. Logika Memantul (Bob)
        if (m_effectType == EffectType.Bob || m_effectType == EffectType.Both)
        {
            float offset = Mathf.Sin(Time.time * m_speed) * m_bobAmount;
            if (m_verticalBob)
            {
                transform.localPosition = m_startPosition + new Vector3(0, offset, 0);
            }
            else
            {
                transform.localPosition = m_startPosition + new Vector3(offset, 0, 0);
            }
        }
    }
}
