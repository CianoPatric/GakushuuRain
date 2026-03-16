using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 250f;
    public Rigidbody2D _rb;
    private Vector2 _movementInput;
    private PlayerAppearance _appearance;
    
    private CosmeticsLibraryManager _cosmeticsLibraryManager;

    public void Initialize(CosmeticsLibraryManager cosmeticsLibraryManager, PlayerAppearance appearance)
    {
        _cosmeticsLibraryManager = cosmeticsLibraryManager;
        _appearance = appearance;
    }

    void Update()
    {
        _movementInput.x = Input.GetAxisRaw("Horizontal");
        _movementInput.y = Input.GetAxisRaw("Vertical");
        
        bool isMoving = _movementInput.magnitude > 0.1f;
        _appearance._masterAnimator.SetBool("isMoving", isMoving);
        
        if (_movementInput.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            if(_movementInput.x > 0.1f)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    
    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movementInput.normalized * (moveSpeed * Time.fixedDeltaTime));
    }
}
