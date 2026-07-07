using UnityEngine;

// Taruh script ini di setiap GameObject item yang bisa diambil
public class Item : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Item";        // Nama item
    public int itemValue = 1;               // Nilai / poin item
    public ItemType itemType = ItemType.Coin; // Jenis item
    public float healAmount = 20f;          // Khusus Potion: jumlah HP yang dipulihkan

    [Header("Effects")]
    public GameObject collectEffect;        // Efek partikel saat diambil (opsional)
    public AudioClip collectSound;          // Suara saat diambil (opsional)

    [Header("Bobbing Animation")]
    public bool enableBobbing = true;       // Animasi naik-turun
    public float bobbingSpeed = 2f;
    public float bobbingHeight = 0.1f;

    private Vector3 startPosition;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Animasi item naik turun
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Dipanggil ketika player menyentuh item
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        // Kirim info ke InventoryManager
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(this);
        }

        // Khusus Potion: langsung heal player
        if (itemType == ItemType.Potion && BuffApplier.Instance != null)
        {
            BuffApplier.Instance.Heal(healAmount);
        }

        // Spawn efek partikel
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Mainkan suara
        if (collectSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Hancurkan item dari scene
        Destroy(gameObject);
    }
}

// Enum untuk jenis item
public enum ItemType
{
    Coin,
    Key,
    Potion,
    Weapon,
    Other
}