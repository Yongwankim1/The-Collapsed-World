using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscManager : MonoBehaviour
{
    public static EscManager Instance;
    private Stack<GameObject> openPanels = new Stack<GameObject>();
    [SerializeField] int count;
    [SerializeField] GameObject currentEscPanel;
    [SerializeField] AudioClip uiOpen;
    [SerializeField] AudioClip uiClose;
    public int Count => openPanels.Count;
    public GameObject CurrentEscPanel => currentEscPanel;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!PopPanel())
            {
                PushPanel(currentEscPanel);
            }
        }
    }
    public void SetCurrentEscPanel(GameObject panel)
    {
        currentEscPanel = panel;
    }

    public void PushPanel(GameObject panel)
    {
        if (panel == null) return;
        panel.SetActive(true);
        openPanels.Push(panel);
        count = openPanels.Count;
        if (SoundManager.Instance)
            SoundManager.Instance.PlaySfxOneShot(uiOpen,0.7f);
    }

    public bool PopPanel()
    {
        if(openPanels.Count == 0) return false;
        openPanels.Pop().SetActive(false);
        count = openPanels.Count;
        if (SoundManager.Instance)
            SoundManager.Instance.PlaySfxOneShot(uiClose,0.7f);
        return true;
    }
}
