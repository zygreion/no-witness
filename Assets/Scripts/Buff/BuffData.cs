using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "No Witness/Buff Data")]
public class BuffData : ScriptableObject
{
    [Header("Info Kartu")]
    public string buffName;
    [TextArea] public string description;
    public Sprite cardIcon;

    [Header("Efek Buff")]
    public float attackMultiplier = 1f;   // 1.5 = ATK +50%
    public float speedMultiplier = 1f;    // 1.4 = SPD +40%
    public float bonusHP = 0f;            // 80 = HP +80
}