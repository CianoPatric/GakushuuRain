using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 250f;
    
    private Rigidbody2D _rb;
    private Vector2 _movement;

    [Header("Аниматоры")]
    public Animator pantsAnimator;
    public Animator shirtAnimator;
    public Image accessorySlot;
    public Image hatSlot;

    private Animator[] _allAnimators;

    private CosmeticsLibraryManager _cosmeticsLibraryManager;

    public void Initialize(CosmeticsLibraryManager cosmeticsLibraryManager)
    {
        _cosmeticsLibraryManager = cosmeticsLibraryManager;
        _rb = GetComponent<Rigidbody2D>();
        _allAnimators = new[]{shirtAnimator, pantsAnimator};
    }

    void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        if (_movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if(_movement.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        _movement.y = Input.GetAxisRaw("Vertical");
    }

    public void UpdateAppearance(PlayerCustomization equippedItems)
    {
        UpdateAnimatedSlot(pantsAnimator, equippedItems.pantsId);
        UpdateAnimatedSlot(shirtAnimator, equippedItems.shirtId);
        UpdateStaticSlot(hatSlot, equippedItems.hatId);
        UpdateStaticSlot(accessorySlot, equippedItems.accessoryId);
    }

    private void UpdateAnimatedSlot(Animator animator, string itemId)
    {
        if(animator == null) return;
        
        CosmeticItem itemData = _cosmeticsLibraryManager.GetCosmeticItem(itemId);
        if (itemData != null && itemData.animatorController != null)
        {
            animator.gameObject.SetActive(true);
            animator.runtimeAnimatorController = itemData.animatorController;
        }
        else
        {
            animator.gameObject.SetActive(false);
        }
    }

    private void UpdateStaticSlot(Image imageSlot, string itemId)
    {
        if(imageSlot == null) return;
        
        CosmeticItem itemData = _cosmeticsLibraryManager.GetCosmeticItem(itemId);
        if (itemData != null && itemData.sprite != null && itemData.animatorController == null)
        {
            imageSlot.enabled = true;
            imageSlot.sprite = itemData.sprite;
        }
        else
        {
            imageSlot.enabled = false;
        }
    }

    void FixedUpdate()
    {
        foreach (var anim in _allAnimators)
        {
            if (anim != null && anim.gameObject.activeSelf && anim.runtimeAnimatorController != null)
            {
                anim.SetFloat("Speed", _movement.magnitude);
            }
        }
        _rb.MovePosition(_rb.position + _movement.normalized * (moveSpeed * Time.fixedDeltaTime));
    }
}
