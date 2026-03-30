using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private List<IInteractable> _nearbyInteractables = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _nearbyInteractables.Count > 0)
        {
            _nearbyInteractables.First().Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            if (_nearbyInteractables.Count != 0)
            {
                interactable.ShowIndicator(true);
            }
            _nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && _nearbyInteractables.Contains(interactable))
        {
            foreach (var item in _nearbyInteractables)
            {
                item.ShowIndicator(false);
            }
            _nearbyInteractables.Remove(interactable);

            if (_nearbyInteractables.Count > 0)
            {
                _nearbyInteractables.First().ShowIndicator(true);
            }
        }
    }
}