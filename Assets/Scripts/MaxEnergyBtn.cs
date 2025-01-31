using UnityEngine;

public class MaxEnergyBtn : MonoBehaviour
{
    PlayerStats playerStats;
    SoundManager soundManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        soundManager = SoundManager.instance;
    }

    public void MaxEnergy()
    {
        if (playerStats == null || soundManager == null) return;

        playerStats.Recharge();
        soundManager.BtnSound();
    }
}
