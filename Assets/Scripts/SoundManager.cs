using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource jabSFX;
    [SerializeField] AudioSource straightSFX;
    [SerializeField] AudioSource hookSFX;
    [SerializeField] AudioSource kneeSFX;
    [SerializeField] AudioSource uppercutSFX;
    [SerializeField] AudioSource specialStartupSFX;
    [SerializeField] AudioSource grabSFX;
    [SerializeField] AudioSource missSFX;
    [SerializeField] AudioSource energyMaxSFX;
    [SerializeField] AudioSource btnSFX;

    private void Awake()
    {
        instance = this;
    }

    #region PlayerSFX
    public void Jab()
    {
        jabSFX.Play();
    }

    public void Straight()
    {
        straightSFX.Play();
    }

    public void Hook()
    {
        hookSFX.Play();
    }

    public void Knee()
    {
        kneeSFX.Play();
    }

    public void Uppercut()
    {
        uppercutSFX.Play();
    }

    public void SpecialStartup()
    {
        specialStartupSFX.Play();
    }

    public void Grab()
    {
        grabSFX.Play();
    }

    public void MissAttack()
    {
        missSFX.Play();
    }

    public void EnergyFull()
    {
        energyMaxSFX.Play();
    }

    #endregion

    #region UI

    public void BtnSound()
    {
        btnSFX.Play();
    }

    #endregion
}
