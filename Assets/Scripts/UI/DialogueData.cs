using UnityEngine;

[System.Serializable]
public class DialogueSentence
{
    public string speakerName;          // Nama karakter yang sedang berbicara
    
    [TextArea(3, 10)]
    public string sentence;            // Kalimat percakapan
    
    public Sprite speakerPortrait;     // Gambar portrait karakter pembicara (opsional)
    
    public Sprite backgroundImage;     // Gambar background kustom untuk kalimat ini (opsional)
}

[System.Serializable]
public class Dialogue
{
    public DialogueSentence[] sentences; // Kumpulan kalimat dalam satu percakapan
}
