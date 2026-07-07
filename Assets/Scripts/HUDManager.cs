using UnityEngine;
using TMPro; // TextMeshPro — ganti ke UnityEngine.UI jika pakai Text biasa

// Taruh script ini di Canvas > HUDPanel GameObject
public class HUDManager : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI coinText;     // Text untuk jumlah koin
    public TextMeshProUGUI keyText;      // Text untuk jumlah kunci
    public TextMeshProUGUI potionText;   // Text untuk jumlah potion
    public TextMeshProUGUI scoreText;    // Text untuk total skor

    [Header("Pop-up Notification")]
    public GameObject itemPickupPopup;   // Panel notifikasi item diambil
    public TextMeshProUGUI popupText;    // Text di popup
    public float popupDuration = 1.5f;  // Berapa lama popup tampil

    private float popupTimer;

    private void Start()
    {
        // Daftarkan diri ke event InventoryManager
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged.AddListener(UpdateHUD);
        }

        UpdateHUD();

        // Sembunyikan popup di awal
        if (itemPickupPopup != null)
            itemPickupPopup.SetActive(false);
    }

    private void Update()
    {
        // Timer untuk sembunyikan popup
        if (popupTimer > 0)
        {
            popupTimer -= Time.deltaTime;
            if (popupTimer <= 0 && itemPickupPopup != null)
                itemPickupPopup.SetActive(false);
        }
    }

    // Update semua text HUD
    public void UpdateHUD()
    {
        if (InventoryManager.Instance == null) return;

        if (coinText != null)
            coinText.text = "Koin: " + InventoryManager.Instance.totalCoins;

        if (keyText != null)
            keyText.text = "Kunci: " + InventoryManager.Instance.totalKeys;

        if (potionText != null)
            potionText.text = "Potion: " + InventoryManager.Instance.totalPotions;

        if (scoreText != null)
            scoreText.text = "Skor: " + InventoryManager.Instance.totalScore;
    }

    // Tampilkan notifikasi item pickup
    public void ShowPickupNotification(string message)
    {
        if (itemPickupPopup != null && popupText != null)
        {
            popupText.text = message;
            itemPickupPopup.SetActive(true);
            popupTimer = popupDuration;
        }
    }
}
