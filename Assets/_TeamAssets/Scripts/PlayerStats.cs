using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// O script que lida com os diversos valores do jogador, principalmente
/// aos relacionados a UI do jogo, como a vida e a barra de energia
/// </summary>

public class PlayerStats : HealthAndUI
{
    [Header("Vari�veis relacionadas a UI da barra de energia")]
    [SerializeField] GameObject energyReadyText;
    [SerializeField] GameObject maxEnergyBtn;
    [SerializeField] Slider energySlider;

    [Header("Vari�veis relacionadas a como funciona a barra de energia")]
    [SerializeField] float maxEnergy = 10f;

    [Tooltip("Tempo que leva em segundos para come�ar a recuperar a barra de energia")]
    [SerializeField] float energyRecoveryDelay = 3f;
    [SerializeField] float energyRecoveryRate = 2f;
    float currentEnergy;
    float recoveryTimer;
    bool isRecovering;

    [Tooltip("Clique aqui para ativar ou desativar a recupera��o autom�tica da energia")]
    public bool autoRecoverOn = true;

    protected override void Start()
    {
        base.Start();

        energySlider.maxValue = maxEnergy;
        currentEnergy = maxEnergy;
        energySlider.value = currentEnergy;
    }

    private void Update()
    {
        // Verifica se a energia atual � igual a m�xima e impede que seja maior que a m�xima
        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;

            // Faz o tratamento de quando chega na energia m�xima depois de gastar
            if (isRecovering) 
            {
                isRecovering = false;
                SoundManager.instance.EnergyFull();
                energyReadyText.SetActive(true);
                maxEnergyBtn.SetActive(false);
            }

            return;
        }

        // Caso a recupera��o autom�tica estiver desligada, n�o prosegue com o resto do c�digo
        if(!autoRecoverOn)
            return;

        if (!isRecovering)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= energyRecoveryDelay) 
            {
                isRecovering = true;
                recoveryTimer = 0;
            }
        }

        if (isRecovering) 
        {
            currentEnergy += energyRecoveryRate * Time.deltaTime;
            energySlider.value = currentEnergy;
        }
    }

    // Um m�todo de recuperar a vida do jogador para ser usado futuramente
    public void RecoverHealth(float _recoverValue)
    {
        currentHealth += _recoverValue;

        if (currentHealth >= maxHealth) 
        {
            currentHealth = maxHealth;
        }

        healthSlider.value = currentHealth;  
    }

    /* 
     * Usa toda a energia do jogador de uma vez, mas pode ser modificado para receber
     * o quanto vai diminuir de energia ao usar energia.
     */
    public void UseEnergy()
    {
        currentEnergy = 0;
        energySlider.value = currentEnergy;
        isRecovering = false;
        recoveryTimer = 0;

        maxEnergyBtn.SetActive(true);
    }

    // Usado pelo bot�o de recarregar energia para recarregar toda a energia de uma vez
    public void Recharge()
    {
        currentEnergy = maxEnergy;
        energySlider.value = currentEnergy;
        isRecovering = true;
    }

    // Diz ao script que cuida dos inputs do jogador se o especial pode ou n�o ser usado
    public bool CanUseSpecial()
    {
        if (currentEnergy == maxEnergy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Retorna o jogador ao ponto de spawn com tudo no m�ximo caso morra no teste
    protected override void Death()
    {
        transform.position = spawnPos.position;
        
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        healthSlider.value = currentHealth;
        energySlider.value = currentEnergy;
    }
}
