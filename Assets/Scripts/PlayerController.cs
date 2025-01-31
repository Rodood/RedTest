using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum States
{
    Idle,
    Walking,
    Attacking,
    Special,
    Hit
}

public class PlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction attackAction;
    InputAction specialAction;

    PlayerStats playerStats;
    public States currentState;

    public Animator anim;

    // Ground Movement
    private Rigidbody rb;
    public float MoveSpeed = 5f;
    Vector3 targetVelocity;
    bool facingRight = true;
    [SerializeField] Transform mesh;

    // Combo System
    private int comboCount = 0; // Tracks the current combo count
    private float lastAttackTime = 0f; // Tracks the time of the last attack
    public float comboWindow = 0.7f; // Time window to continue the combo
    public float damage;

    // Collider hit enemy
    public LayerMask enemyLayer; // Assign in the Inspector or initialize in Start
    public float attackRange = 2f; // Adjust based on your game
    public Vector3 attackBoxSize = new Vector3(1f, 2f, 0.5f); // Width, Height, Depth
    public Vector3 colPosition = Vector3.right;
    public bool hit = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        specialAction = InputSystem.actions.FindAction("Special");
    }

    // Update is called once per frame
    void Update()
    {
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

        if (attackAction.WasPressedThisFrame())
        {
            Attack();
        }

        if (specialAction.WasPressedThisFrame() && playerStats.CanUseSpecial() && currentState != States.Special) 
        {
            SpecialAttack();
            playerStats.UseEnergy();
        }

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
        // Check if the combo window is still open
        if (Time.time - lastAttackTime > comboWindow)
        {
            comboCount = 0; // Reset combo if the window has passed
            currentState = States.Idle;
        }

        if(comboCount != 3)
        {
            currentState = States.Attacking;

            comboCount++; // Increment combo count
            lastAttackTime = Time.time; // Update the last attack time

            // Trigger the appropriate attack animation based on combo count
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
        if (currentState == States.Special) return; // Prevent multiple activations

        // Enter Special Attack state and play full animation
        currentState = States.Special;
        anim.SetTrigger("special");

        // Start a coroutine to check for enemies at the right time
        StartCoroutine(AttemptGrab());
    }

    IEnumerator AttemptGrab()
    {
        // Wait for the moment in the animation when the grab should happen
        yield return new WaitForSeconds(0.9f); // Adjust this to match the animation timing

        Vector3 grabPosition = transform.position + colPosition * attackRange;
        Collider[] hitEnemies = Physics.OverlapBox(grabPosition, attackBoxSize / 2, transform.rotation, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            foreach (Collider enemy in hitEnemies)
            {
                //enemy.GetComponent<EnemyController>().GrabbedByPlayer();
            }

            hit = true;
        }
        else
        {
            // No enemy was grabbed, immediately stop the animation and return to idle
            anim.ResetTrigger("special"); // Reset animation to prevent weird transitions
            anim.SetTrigger("idle"); // Force reset to idle animation

            yield return new WaitForEndOfFrame();
            currentState = States.Idle;

            hit = false;
        }
    }

    private void Flip()
    {
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
