using UnityEngine;

/// <summary>
/// Um script para dar funcionalidade ao botão de recarregar energia.
/// </summary>

public class MaxEnergyBtn : MonoBehaviour
{
    PlayerStats playerStats;
    SoundManager soundManager;

    // Pega a referência dos dois scripts mais importantes para o bom funcionamento do botão
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        soundManager = SoundManager.instance;
    }

    // Se nenhuma das referências voltou nula, pode recarregar a energia do jogador
    public void MaxEnergy()
    {
        if (playerStats == null || soundManager == null) return;

        playerStats.Recharge();
        soundManager.BtnSound();
    }
}
