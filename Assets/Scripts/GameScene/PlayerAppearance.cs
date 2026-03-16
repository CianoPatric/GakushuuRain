using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerAppearance : MonoBehaviour
{
    [Header("Отображение")]
    [SerializeField] private SpriteResolver _bodyResolver;
    [SerializeField] private SpriteResolver _hatResolver;
    [SerializeField] private SpriteResolver _neckResolver;
    [SerializeField] private SpriteResolver _accessoryResolver;
    [SerializeField] private SpriteResolver _torsResolver;
    [SerializeField] private SpriteResolver _pantsResolver;
    [SerializeField] private SpriteResolver _bootsResolver;
    
    [Header("Индикаторы")]
    [SerializeField] private Animator _dialogueIndicator;
    [SerializeField] private Animator _dresingIndicator;
    
    public Animator _masterAnimator;
    
    public void UpdateVisuals(PlayerCustomization equipped)
    {
        SetSlot(_bodyResolver, "Body", "default");
        
        SetSlot(_hatResolver, "Hat", equipped.hatId);
        SetSlot(_neckResolver, "Neck", equipped.neckId);
        SetSlot(_accessoryResolver, "Accessory", equipped.accessoryId);
        SetSlot(_torsResolver, "Tors", equipped.torsId);
        SetSlot(_pantsResolver, "Pants", equipped.pantsId);
        SetSlot(_bootsResolver, "Boots", equipped.bootsId);
    }

    private void SetSlot(SpriteResolver resolver, string category, string label)
    {
        if(resolver == null) return;
        resolver.SetCategoryAndLabel(category, string.IsNullOrEmpty(label) ? "Empty" : label);
    }
    
    public void TriggerAnimation(string trigger) => _masterAnimator.SetTrigger(trigger);
    
    public void SetDialogueActive(bool active) => _dialogueIndicator.SetBool("isActive", active);
    
    public void SetDressingActive(bool active) => _dresingIndicator.SetBool("isDressing", active);
}