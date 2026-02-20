using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "ShadowStrike/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName;
    public Sprite portrait;
    public Sprite fullArt;
    public Color characterColor;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float walkSpeed = 5f;
    public float jumpForce = 12f;
    public float lightAttackDamage = 10f;
    public float heavyAttackDamage = 25f;

    [Header("Special Move")]
    public string specialMoveName;
    [TextArea] public string specialMoveDescription;
    public float specialMoveDamage = 40f;

    [Header("Lore")]
    [TextArea] public string characterBio;
}