using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Taruh script ini di GameObject kosong bernama "InventoryManager" di scene
public class InventoryManager : MonoBehaviour
{
    // Singleton agar bisa diakses dari mana saja
    public static InventoryManager Instance;

    [Header("Inventory Data")]
    public int totalCoins = 0;
    public int totalKeys = 0;
    public int totalPotions = 0;
    public int totalScore = 0;

    // Daftar semua item yang sudah dikumpulkan
    private List<string> collectedItems = new List<string>();

    // Event yang akan di-trigger saat inventory berubah (untuk update UI)
    public UnityEvent OnInventoryChanged;

    private void Awake()
    {
        // Pastikan hanya ada satu InventoryManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Tidak hilang saat ganti scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Tambah item ke inventory
    public void AddItem(Item item)
    {
        collectedItems.Add(item.itemName);
        totalScore += item.itemValue;

        // Hitung berdasarkan jenis item
        switch (item.itemType)
        {
            case ItemType.Coin:
                totalCoins++;
                Debug.Log($"[Inventory] Koin didapat! Total: {totalCoins}");
                break;
            case ItemType.Key:
                totalKeys++;
                Debug.Log($"[Inventory] Kunci didapat! Total: {totalKeys}");
                break;
            case ItemType.Potion:
                totalPotions++;
                Debug.Log($"[Inventory] Potion didapat! Total: {totalPotions}");
                break;
            default:
                Debug.Log($"[Inventory] {item.itemName} didapat!");
                break;
        }

        // Beritahu UI untuk update
        OnInventoryChanged?.Invoke();
    }

    // Cek apakah punya item tertentu
    public bool HasItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    // Hapus item (misal: pakai potion)
    public bool UseItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Potion:
                if (totalPotions > 0)
                {
                    totalPotions--;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
                break;
            case ItemType.Key:
                if (totalKeys > 0)
                {
                    totalKeys--;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
                break;
        }
        return false;
    }
}
