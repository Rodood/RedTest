using UnityEngine;

/// <summary>
/// Um script para controlar melhor todas as animações do jogador para permitir um melhor gerenciamento
/// delas, além de poder garantir que os eventos de animação toquem no momento correto fazendo com que
/// funcione melhor os efeitos sonoros e criação de colisor.
/// 
/// Esse script é um que poderia ser futuramente modificado para servir de referência para criar o mesmo
/// tratamento para os inimigos no jogo e animações deles.
///
/// Sempre que um áudio tiver um if (playerController.hit) é para mudar o tratamento caso o jogador acertou
/// ou não um inimigo
/// </summary>

public class PlayerAnimationController : MonoBehaviour
{
    SoundManager soundManager;

    PlayerController playerController;

    [Tooltip("Referência da posição em que o inimigo vai ser segurado durante a animação")]
    public Transform grabPosition; // Posição em que o inimigo será segurado no ataque especial
    private GameObject grabbedEnemy;

    // Pega a referência dos dois scripts mais importantes para garantir um bom funcionamento da lógica de animação
    void Start()
    {
        soundManager = SoundManager.instance;
        playerController = GetComponentInParent<PlayerController>();
    }

    #region SFX
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

    #endregion

    #region Colliders & Checks
    // Garante que o jogador saia do estado de ataque ao acabar as animações
    public void ExitAttackState()
    {
        playerController.currentState = States.Idle;
    }

    public void CreateCollider()
    {
        // Determina a direção que o ataque vai vir
        Vector3 attackPosition = transform.position + playerController.colPosition * playerController.attackRange;

        Collider[] hitEnemies = Physics.OverlapBox(attackPosition, playerController.attackBoxSize / 2, transform.rotation, playerController.enemyLayer);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<BaseEnemy>().TakeDamage(playerController.damage);

                // Caso seja o último golpe do combo, aplique o knockback no inimigo
                if(playerController.comboCount == 3)
                {
                    enemy.GetComponent<BaseEnemy>().Knockback((enemy.transform.position - transform.position).normalized);
                }  
            }

            playerController.hit = true;
        }
        else
        {
            playerController.hit = false;
        }
    }

    public void GrabEnemy()
    {
        if (grabbedEnemy != null) return; // Impede que múltiplos inimigos sejam pegos na animação

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // Garante que de fato é um inimigo e faz a tratativa para ficar correto a animação
            {
                grabbedEnemy = enemy.gameObject;
                grabbedEnemy.transform.SetParent(grabPosition);
                grabbedEnemy.transform.localPosition = Vector3.zero;
                grabbedEnemy.GetComponent<Rigidbody>().isKinematic = true;
                break;
            }
        }
    }

    public void ReleaseEnemy()
    {
        if (grabbedEnemy != null)
        {
            grabbedEnemy.transform.SetParent(null);
            Rigidbody rb = grabbedEnemy.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(transform.forward * 5f, ForceMode.Impulse); // Empurra o inimigo para frente
            grabbedEnemy = null;
        }
    }

    // Criei esse método para poder dar knockback no final do especial, podendo também modificar para fazer outras tratativas
    public void SpecialEnding()
    {
        Vector3 attackPosition = transform.position + playerController.colPosition * playerController.attackRange;

        Collider[] hitEnemies = Physics.OverlapBox(attackPosition, playerController.attackBoxSize / 2, transform.rotation, playerController.enemyLayer);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                enemy.GetComponent<BaseEnemy>().Knockback((enemy.transform.position - transform.position).normalized);
            }
        }
    }

    #endregion
}
