using System.Collections;
using UnityEngine;

public class ExitZone : MonoBehaviour, IInteractable
{
    [SerializeField] float exitTimer = 3f;
    [SerializeField] ExitTimerUI exitTimerUI;
    void Start()
    {
        if(exitTimerUI == null) exitTimerUI = FindFirstObjectByType<ExitTimerUI>(FindObjectsInactive.Include);    
    }
    public void Interact(PlayerInteract player)
    {
        exitTimerUI.OnStartExitTimer(exitTimer);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (exitTimerUI == null) return;

        exitTimerUI.OnStopExitTimer();
    }
}
