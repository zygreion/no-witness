using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditSceneText : MonoBehaviour
{
    [SerializeField] private float m_yIncrease = 80.0f;
    [SerializeField] private float m_scrollLimit = 4950.0f; // Increased from 4624 to accommodate the new top text
    
    private RectTransform m_rectTransform;
    private bool m_moveUp = true;
    
    private TMP_Text m_noWitnessText;
    private bool m_showEndText = false;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Find the TextMeshPro components in children
        var boldText = transform.Find("CreditContent C Bold")?.GetComponent<TMP_Text>();
        var noBoldText = transform.Find("CreditContent C NoBold")?.GetComponent<TMP_Text>();
        var leftText = transform.Find("CreditContent L")?.GetComponent<TMP_Text>();
        var rightText = transform.Find("CreditContent R")?.GetComponent<TMP_Text>();

        // Prepend "THANK YOU FOR PLAYING OUR GAME DEMO" to the bold column, and offset the others to align them
        if (boldText != null)
        {
            boldText.text = "\nTHANK YOU FOR PLAYING OUR GAME DEMO\n\n\n" + boldText.text;
        }
        if (noBoldText != null)
        {
            noBoldText.text = "\n\n\n\n\n" + noBoldText.text;
        }
        if (leftText != null)
        {
            leftText.text = "\n\n\n\n\n" + leftText.text;
        }
        if (rightText != null)
        {
            rightText.text = "\n\n\n\n\n" + rightText.text;
        }

        // Setup Skip Button and NO WITNESS End Text
        SetupUI(boldText);
    }

    private void SetupUI(TMP_Text referenceText)
    {
        if (transform.parent == null) return;

        // 1. Create Skip Button
        GameObject skipBtnObj = new GameObject("SkipButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        skipBtnObj.transform.SetParent(transform.parent, false); // parent to Canvas
        
        RectTransform skipRect = skipBtnObj.GetComponent<RectTransform>();
        skipRect.anchorMin = new Vector2(1, 0); // Bottom Right
        skipRect.anchorMax = new Vector2(1, 0);
        skipRect.pivot = new Vector2(1, 0);
        skipRect.anchoredPosition = new Vector2(-100, 50);
        skipRect.sizeDelta = new Vector2(160, 50);

        // Style the button image
        var img = skipBtnObj.GetComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.6f); // Semi-transparent dark background
        
        // Add button listener
        var btn = skipBtnObj.GetComponent<Button>();
        btn.onClick.AddListener(() => {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
            SceneManager.LoadScene("Main-Menu");
        });

        // Add hover/pressed color states
        var colors = btn.colors;
        colors.normalColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        colors.highlightedColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.pressedColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        btn.colors = colors;

        // Add text to button
        GameObject skipTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer));
        skipTextObj.transform.SetParent(skipBtnObj.transform, false);
        var skipText = skipTextObj.AddComponent<TextMeshProUGUI>();
        skipText.text = "SKIP";
        skipText.fontSize = 20;
        skipText.alignment = TextAlignmentOptions.Center;
        skipText.color = Color.white;
        if (referenceText != null)
        {
            skipText.font = referenceText.font;
        }
        
        RectTransform textRect = skipTextObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        // 2. Create NO WITNESS End Text
        GameObject noWitnessObj = new GameObject("NoWitnessEndText", typeof(RectTransform), typeof(CanvasRenderer));
        noWitnessObj.transform.SetParent(transform.parent, false); // parent to Canvas
        
        m_noWitnessText = noWitnessObj.AddComponent<TextMeshProUGUI>();
        m_noWitnessText.text = "NO WITNESS";
        m_noWitnessText.fontSize = 80;
        m_noWitnessText.fontStyle = FontStyles.Bold;
        m_noWitnessText.alignment = TextAlignmentOptions.Center;
        m_noWitnessText.color = new Color(0.8f, 0f, 0f, 0f); // Starts transparent red
        // Load the GODOFWAR SDF font asset from Resources folder
        TMP_FontAsset godOfWarFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/GODOFWAR SDF");
        if (godOfWarFont != null)
        {
            m_noWitnessText.font = godOfWarFont;
        }
        else if (referenceText != null)
        {
            m_noWitnessText.font = referenceText.font;
        }
        
        RectTransform nwRect = noWitnessObj.GetComponent<RectTransform>();
        nwRect.anchorMin = new Vector2(0.5f, 0.5f);
        nwRect.anchorMax = new Vector2(0.5f, 0.5f);
        nwRect.pivot = new Vector2(0.5f, 0.5f);
        nwRect.anchoredPosition = Vector2.zero;
        nwRect.sizeDelta = new Vector2(800, 200);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rectTransform == null) return;

        if (m_moveUp)
        {
            Vector3 localPosition = m_rectTransform.localPosition;
            localPosition.y += m_yIncrease * Time.deltaTime;

            m_rectTransform.localPosition = localPosition;

            if (localPosition.y >= m_scrollLimit)
            {
                m_moveUp = false;
                m_showEndText = true;
            }
        }
        else if (m_showEndText)
        {
            // Fade in NO WITNESS text in the center of the screen
            if (m_noWitnessText != null)
            {
                Color c = m_noWitnessText.color;
                if (c.a < 1.0f)
                {
                    c.a += Time.deltaTime * 0.5f; // Fades in over 2 seconds
                    m_noWitnessText.color = c;
                }
                else
                {
                    m_showEndText = false;
                }
            }
        }
    }
}
