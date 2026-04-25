using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour, IInteractable
{
    [SerializeField] float exitTimer;
    Coroutine exitZoneActive;
    public void Interact(PlayerInteract player)
    {
        if(exitZoneActive == null) exitZoneActive = StartCoroutine(Exit());
    }
    IEnumerator Exit()
    {
        yield return new WaitForSeconds(exitTimer);
        SceneManager.LoadScene("ResultScene");
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (exitZoneActive != null)
        {
            StopCoroutine(exitZoneActive);
        }
    }
}
