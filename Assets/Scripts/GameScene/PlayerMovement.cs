using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 250f;
    
    public Animator animator;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        if (movement.x < 0)
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            if(movement.x > 0)
            {
                this.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        animator.SetFloat("Speed", movement.magnitude);
        rb.MovePosition(rb.position + movement.normalized * (moveSpeed * Time.fixedDeltaTime));
    }
}
