using UnityEngine;

/// <summary>
/// Um script para dar funcionalidade ao bot�o de recarregar energia.
/// </summary>

public class MaxEnergyBtn : MonoBehaviour
{
    PlayerStats playerStats;
    SoundManager soundManager;

    // Pega a refer�ncia dos dois scripts mais importantes para o bom funcionamento do bot�o
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        soundManager = SoundManager.instance;
    }

    // Se nenhuma das refer�ncias voltou nula, pode recarregar a energia do jogador
    public void MaxEnergy()
    {
        if (playerStats == null || soundManager == null) return;

        playerStats.Recharge();
        soundManager.BtnSound();
    }
}
