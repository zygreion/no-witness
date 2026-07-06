using UnityEngine;
using System.Collections.Generic;

public class BuffSelectionManager : MonoBehaviour
{
    [Header("Buff Options (isi 3 buff)")]
    public List<BuffData> availableBuffs;

    [Header("Card Prefab & Container")]
    public GameObject buffCardPrefab;
    public Transform cardContainer;

    [Header("Panel Buff (untuk disembunyikan setelah pilih)")]
    public GameObject buffPanel;

    private List<BuffCardUI> spawnedCards = new List<BuffCardUI>();
    private BuffData selectedBuff;

    void Start()
    {
        // Panel sudah aktif saat scene dimulai
        // Game di-pause sampai buff dipilih
        Time.timeScale = 0f;
        SpawnCards();
    }

    void SpawnCards()
    {
        foreach (var buffData in availableBuffs)
        {
            GameObject cardObj = Instantiate(buffCardPrefab, cardContainer);
            BuffCardUI cardUI = cardObj.GetComponent<BuffCardUI>();
            cardUI.Setup(buffData, this);
            spawnedCards.Add(cardUI);
        }
    }

    public void OnCardSelected(BuffCardUI clickedCard, BuffData buff)
    {
        selectedBuff = buff;

        foreach (var card in spawnedCards)
            card.SetSelected(false);

        clickedCard.SetSelected(true);
    }

    public void OnConfirmClicked()
    {
        if (selectedBuff == null)
        {
            Debug.LogWarning("Belum ada kartu yang dipilih!");
            return;
        }

        // Apply buff ke player
        if (BuffApplier.Instance != null)
        {
            BuffApplier.Instance.ApplyBuff(selectedBuff);
        }
        else
        {
            Debug.LogWarning("[BuffSelectionManager] BuffApplier.Instance belum ada.");
        }

        // Sembunyikan panel buff
        if (buffPanel != null)
            buffPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
    }
}