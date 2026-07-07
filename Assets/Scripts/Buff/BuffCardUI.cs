using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image cardIconImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardStatText;
    public Button selectButton;
    public GameObject selectedHighlight;

    private BuffData buffData;
    private BuffSelectionManager manager;

    public void Setup(BuffData data, BuffSelectionManager mgr)
    {
        buffData = data;
        manager  = mgr;

        cardIconImage.sprite = data.cardIcon;
        cardNameText.text    = data.buffName;

        // Tampilkan stat utama
        string statText = "";
        if (data.attackMultiplier > 1f)
            statText = $"Attack +{(int)((data.attackMultiplier - 1f) * 100)}%";
        else if (data.speedMultiplier > 1f)
            statText = $"Speed +{(int)((data.speedMultiplier - 1f) * 100)}%";
        else if (data.bonusHP > 0f)
            statText = $"HP +{(int)data.bonusHP}";

        if (cardStatText != null)
            cardStatText.text = statText;

        selectButton.onClick.AddListener(OnConfirmClicked);
    }

    // Klik tombol = langsung pilih sekaligus konfirmasi
    void OnConfirmClicked()
    {
        manager.OnCardSelected(this, buffData);
        manager.OnConfirmClicked();
    }

    public void SetSelected(bool selected)
    {
        if (selectedHighlight != null)
            selectedHighlight.SetActive(selected);
    }
}