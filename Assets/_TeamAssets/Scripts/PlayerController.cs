using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Cria um enum de estados possiveis que o jogador pode estar para ter um melhor controle
/// durante o jogo, podendo expandir mais a lista futuramente
/// </summary>

public enum States
{
    Idle,
    Walking,
    Attacking,
    Special,
    Hit
}

/// <summary>
/// Um script que gerencia todos os inputs do jogador e chama animações dele
/// </summary>

public class PlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction attackAction;
    InputAction specialAction;

    PlayerStats playerStats;
    public States currentState;

    public Animator anim;

    [Header("Valores relacionados ao movimento do personagem")]
    [SerializeField] Rigidbody rb;
    public float MoveSpeed = 5f;
    Vector3 targetVelocity;
    bool facingRight = true;
    [SerializeField] Transform mesh;

    [Header("Valores relacionados ao sistema de combo")]
    public int comboCount = 0; 
    private float lastAttackTime = 0f;
    public float comboWindow = 0.7f; 
    public float damage = 1f;

    [Header("Valores para determinar que o colisor foi até o inimigo")]
    public LayerMask enemyLayer;
    public float attackRange = 2f;
    public Vector3 attackBoxSize = new Vector3(1f, 2f, 0.5f);
    public Vector3 colPosition = Vector3.right;
    public bool hit = false;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        if(rb == null)
            rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        specialAction = InputSystem.actions.FindAction("Special");
    }

    void Update()
    {
        // Modifica o que o jogador pode fazer e as animações de acordo com o estado atual dele
        switch (currentState)
        {
            case States.Idle:
                moveAction.Enable();
                attackAction.Enable();
                anim.SetFloat("speed", 0);
                
                anim.ResetTrigger("jab");
                anim.ResetTrigger("direto");
                anim.ResetTrigger("hook");
                break;
            case States.Walking:
                anim.SetFloat("speed", 1);
                break;
            case States.Attacking:
                moveAction.Disable();
                break;
            case States.Special:
                moveAction.Disable();
                attackAction.Disable();
                break;
            case States.Hit:
                break;
        }

        Inputs();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void Inputs()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        Vector3 movement = (transform.right * moveValue.x + transform.forward * moveValue.y).normalized;
        targetVelocity = movement * MoveSpeed;

        if (attackAction.WasPressedThisFrame() && currentState != States.Special)
        {
            Attack();
        }

        if (specialAction.WasPressedThisFrame() && playerStats.CanUseSpecial() && currentState != States.Special) 
        {
            SpecialAttack();
            playerStats.UseEnergy();
        }

        // Impede que o jogador se mova caso esteja atacando ou usando especial
        if (currentState == States.Special || currentState == States.Attacking)
            return;

        if (moveValue.x == 0 && moveValue.y == 0)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            currentState = States.Idle;
        }
        else
        {
            currentState = States.Walking;
        }

        if (moveValue.x < 0 && facingRight)
        {
            Flip();
        }
        else if (moveValue.x > 0 && !facingRight)
        {
            Flip();
        }
    }

    void MovePlayer()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;
    }

    void Attack()
    {
        // Verifica se a janela de combo ainda está aberta
        if (Time.time - lastAttackTime > comboWindow)
        {
            comboCount = 0; // Reseta o combo se passou do tempo da janela
            currentState = States.Idle;
        }

        if(comboCount != 3)
        {
            currentState = States.Attacking;

            comboCount++; 
            lastAttackTime = Time.time; // Atualiza o tempo do último ataque vez que atacou

            // Chama a animação correta de acordo com o combo
            if (comboCount == 1)
            {
                anim.SetTrigger("jab");
            }
            else if (comboCount == 2)
            {
                anim.SetTrigger("direto");
            }
            else if (comboCount == 3)
            {
                anim.SetTrigger("hook");
            }
        }
    }

    void SpecialAttack()
    {
        // Entra no ataque e chama a animação do especial
        currentState = States.Special;
        anim.SetTrigger("special");

        // Começa uma coroutina para verificar se os inimigos apareceram no momento certo
        StartCoroutine(AttemptGrab());
    }

    /* 
     * Grande parte dessa lógica seria melhor no scrip de controle de animação, mas fiquei com pouco tempo
     * para reescrever o script e testar para verificar se está funcionando corretamente
     */
    IEnumerator AttemptGrab()
    {
        // Espera o momento certo na animação para verificar se acertou
        yield return new WaitForSeconds(0.9f);

        Vector3 grabPosition = transform.position + colPosition * attackRange;
        Collider[] hitEnemies = Physics.OverlapBox(grabPosition, attackBoxSize / 2, transform.rotation, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            hit = true;
        }
        else
        {
            // Não agarrou nenhum inimigo, então volte imediatamente para a animação de idle
            anim.ResetTrigger("special"); // Reseta a animação pra impedir que aconteça uma transição esquisita
            anim.SetTrigger("idle"); // Vai pra idle imediatamente

            yield return new WaitForEndOfFrame();
            currentState = States.Idle;

            hit = false;
        }
    }

    // Flipa o jogador se ele estiver andando para o lado oposto de onde ele está olhando
    private void Flip()
    {
        /* 
         * Infelizmente tive que rotacionar, pois se fizesse o jogador ficar olhando a camêra através de
         * flipar a escala dele, ocorria um bug com os bones da malha 3D que quebrava algumas animações
         * do jogador.
         */
        Quaternion newRotation = Quaternion.Euler(0, facingRight ? -90 : 90, 0);
        mesh.rotation = newRotation;

        colPosition *= -1;

        facingRight = !facingRight;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 grabPosition = transform.position + colPosition * attackRange;
        Gizmos.DrawWireCube(grabPosition, attackBoxSize);
    }
}
