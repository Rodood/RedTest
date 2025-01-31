using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : HealthAndUI
{
    [SerializeField] TextMeshProUGUI energyReadyText;
    [SerializeField] Slider energySlider;
    [SerializeField] float maxEnergy = 10f;
    [SerializeField] float energyRecoveryDelay = 3f;
    [SerializeField] float energyRecoveryRate = 2f;
    float currentEnergy;
    float recoveryTimer;
    bool isRecovering;

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
        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;

            if (isRecovering) 
            {
                isRecovering = false;
                SoundManager.instance.EnergyFull();
                energyReadyText.gameObject.SetActive(true);
            }

            return;
        }

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

    public void RecoverHealth(float _recoverValue)
    {
        currentHealth += _recoverValue;

        if (currentHealth >= maxHealth) 
        {
            currentHealth = maxHealth;
        }

        healthSlider.value = currentHealth;  
    }

    public void UseEnergy()
    {
        currentEnergy = 0;
        energySlider.value = currentEnergy;
        isRecovering = false;
        recoveryTimer = 0;
    }

    public void Recharge()
    {
        currentEnergy = maxEnergy;
        energySlider.value = currentEnergy;
        isRecovering = true;
    }

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

    protected override void Death()
    {
        throw new System.NotImplementedException();
    }
}
