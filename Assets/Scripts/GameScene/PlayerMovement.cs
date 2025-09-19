using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 250f;
    
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Аниматоры")]
    public Animator pantsAnimator;
    public Animator shirtAnimator;
    public Image accessorySlot;
    public Image hatSlot;

    private Animator[] allAnimators;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        allAnimators = new[]{shirtAnimator, pantsAnimator};
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if(movement.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        movement.y = Input.GetAxisRaw("Vertical");
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
        
        CosmeticItem itemData = CosmeticsLibraryManager.Instance.GetCosmeticItem(itemId);
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
        
        CosmeticItem itemData = CosmeticsLibraryManager.Instance.GetCosmeticItem(itemId);
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
        foreach (var anim in allAnimators)
        {
            if (anim != null && anim.gameObject.activeSelf && anim.runtimeAnimatorController != null)
            {
                anim.SetFloat("Speed", movement.magnitude);
            }
        }
        rb.MovePosition(rb.position + movement.normalized * (moveSpeed * Time.fixedDeltaTime));
    }
}
