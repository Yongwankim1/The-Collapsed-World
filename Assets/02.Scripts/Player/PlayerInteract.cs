using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] PlayerInputReader inputReader;
    List<IInteractable> interactions = new List<IInteractable> ();

    public bool IsInteract { get; private set; } = false;

    private void Awake()
    {
        if(inputReader == null) inputReader = GetComponent<PlayerInputReader>();

    }
    private void Update()
    {
        IsInteract = inputReader != null ? inputReader.IsInteractPerformedThisFrame : false;
        if (IsInteract)
        {
            Interact();
        }
    }
    private void Interact()
    {
        if (interactions.Count <= 0)
        {
            Debug.LogWarning("상호작용 대상 없음");
            return;
        }
        interactions[interactions.Count - 1].Interact(this);
        if (interactions.Count > 0) interactions.RemoveAt(interactions.Count - 1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interaction = collision.GetComponent<IInteractable>();
        if (interaction == null) return;

        if (interactions.Contains(interaction)) return;
        interactions.Add(interaction);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interaction = collision.GetComponent<IInteractable> ();
        if (interaction == null) return;

        interactions.Remove(interaction);
    }
}
