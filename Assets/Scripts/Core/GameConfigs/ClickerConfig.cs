using UnityEngine;

[CreateAssetMenu(fileName = "ClickerConfig", menuName = "Game/Configs/Clicker Config")]
public class ClickerConfig : ScriptableObject
{
    [Header("Currency Settings")]
    public string CurrencyName = "Coins";
    
    [Header("Click Settings")]
    public int ClickReward = 1;
    public int ClickEnergyCost = 1;
    
    [Header("Auto Collect Settings")]
    public float AutoCollectInterval = 3f;
    public int AutoCollectReward = 1;
    public int AutoCollectEnergyCost = 1;
    
    [Header("Energy Settings")]
    public int MaxEnergy = 1000;
    public int EnergyRestoreAmount = 10;
    public float EnergyRestoreInterval = 10f;
    public int InitialEnergy = 1000;
    
    [Header("VFX Settings")]
    public float ParticleLifetime = 1f;
    public float CoinFlyDuration = 0.5f;
    public float ButtonPressScale = 0.9f;
    public float ButtonPressDuration = 0.1f;
}