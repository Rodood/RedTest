using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    SoundManager soundManager;

    PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundManager = SoundManager.instance;
        playerController = GetComponentInParent<PlayerController>();
    }

    public void JabSFX()
    {
        if (playerController.hit) 
        {
            soundManager.Jab();
        }
        else
        {
            soundManager.MissAttack();
        }
    }

    public void StraightSFX()
    {
        if (playerController.hit)
        {
            soundManager.Straight();
        }
        else
        {
            soundManager.MissAttack();
        }
    }

    public void HookSFX()
    {
        if (playerController.hit)
        {
            soundManager.Hook();
        }
        else
        {
            soundManager.MissAttack();
        }
    }

    public void KneeSFX()
    {
        soundManager.Knee();
    }

    public void UppercutSFX() 
    { 
        soundManager.Uppercut();
    }

    public void SpecialStartupSFX()
    {
        soundManager.SpecialStartup();
    }

    public void GrabSFX()
    {
        if (playerController.hit)
        {
            soundManager.Grab();
        }
        else
        {
            soundManager.MissAttack();
        }
    }

    public void ExitAttackState()
    {
        playerController.currentState = States.Idle;
    }

    public void CreateCollider()
    {
        // Determine the attack direction
        Vector3 attackPosition = transform.position + playerController.colPosition * playerController.attackRange;

        Collider[] hitEnemies = Physics.OverlapBox(attackPosition, playerController.attackBoxSize / 2, transform.rotation, playerController.enemyLayer);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                // Apply damage or interaction
                enemy.GetComponent<BaseEnemy>().TakeDamage(playerController.damage);
            }

            playerController.hit = true;
        }
        else
        {
            playerController.hit = false;
        }
    }
}
