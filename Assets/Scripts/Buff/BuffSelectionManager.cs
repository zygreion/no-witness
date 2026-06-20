using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BuffSelectionManager : MonoBehaviour
{
    [Header("Buff Options (isi 3 buff)")]
    public List<BuffData> availableBuffs;

    [Header("Card Prefab & Container")]
    public GameObject buffCardPrefab;
    public Transform cardContainer;

    [Header("Scene Tujuan")]
    public string dungeon2SceneName = "Dungeon2";

    private List<BuffCardUI> spawnedCards = new List<BuffCardUI>();
    private BuffData selectedBuff;

    void Start()
    {
        Time.timeScale = 0f; // pause game
        SpawnCards();
    }

    void SpawnCards()
    {
        foreach (var buffData in availableBuffs)
        {
            GameObject cardObj = Instantiate(buffCardPrefab, cardContainer);
            BuffCardUI cardUI  = cardObj.GetComponent<BuffCardUI>();
            cardUI.Setup(buffData, this);
            spawnedCards.Add(cardUI);
        }
    }

    public void OnCardSelected(BuffCardUI clickedCard, BuffData buff)
    {
        selectedBuff = buff;

        // Reset highlight semua kartu
        foreach (var card in spawnedCards)
            card.SetSelected(false);

        clickedCard.SetSelected(true);
    }

    // Hubungkan ke tombol "Konfirmasi"
    public void OnConfirmClicked()
    {
        if (selectedBuff == null)
        {
            Debug.LogWarning("Belum ada kartu yang dipilih!");
            return;
        }

        PlayerStats.Instance.ApplyBuff(selectedBuff);

        Time.timeScale = 1f; // resume game
        SceneManager.LoadScene(dungeon2SceneName);
    }
}