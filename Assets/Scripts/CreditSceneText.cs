using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneText : MonoBehaviour
{
    [SerializeField] private float m_yIncrease = 80.0f;
    private RectTransform m_rectTransform;
    private bool m_moveUp = true;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rectTransform == null || !m_moveUp) return;

        Vector3 localPosition = m_rectTransform.localPosition;
        localPosition.y += m_yIncrease * Time.deltaTime;

        m_rectTransform.localPosition = localPosition;

        if (localPosition.y >= 4624)
            m_moveUp = false;
    }
}
