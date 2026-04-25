using UnityEngine;

public class InventoryAction : MonoBehaviour
{
    [SerializeField] PlayerInputReader inputReader;
    [SerializeField] InventoryGUI inventoryGUI;

    private void Awake()
    {
        if (inputReader == null) inputReader = GetComponent<PlayerInputReader>();
        if (inventoryGUI == null) inventoryGUI = GameObject.Find("InventoryPanel").GetComponent<InventoryGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        if (inputReader == null) return;
        if (inputReader.IsInventoryPerformedThisFrame)
        {
            //inventoryGUI.gameObject.SetActive(!inventoryGUI.gameObject.activeSelf);
            if (!inventoryGUI.gameObject.activeSelf)
            {
                EscManager.Instance.PushPanel(inventoryGUI.gameObject);
            }
            else
            {
                EscManager.Instance.PopPanel();
            }
        }

    }
    public void OnInventory()
    {
        if (inventoryGUI == null) return;

        //inventoryGUI.gameObject.SetActive(true);
        EscManager.Instance.PushPanel(inventoryGUI.gameObject);
    }
}
