using UnityEngine;

public class ChestAnimatorController : MonoBehaviour
{
    [SerializeField] RandomDropInventory randomDropInventory;
    [SerializeField] Animator targetAnimator;
    [SerializeField] string isOpenParameterName = "IsOpen";
    int isOpenHash;

    private void Awake()
    {
        if(targetAnimator == null) targetAnimator = GetComponent<Animator>();
        if(randomDropInventory == null) randomDropInventory = GetComponent<RandomDropInventory>();
        isOpenHash = StringToHash(isOpenParameterName);
    }
    private void OnEnable()
    {
        if (randomDropInventory == null) return;
        randomDropInventory.OnOpenInventroy += SetIsOpen;
    }

    private void OnDisable()
    {
        if (randomDropInventory == null) return;
        randomDropInventory.OnOpenInventroy -= SetIsOpen;
    }
    public void SetIsOpen(bool value)
    {
        if (isOpenHash == 0) return;
        targetAnimator.SetBool(isOpenHash, value);
    }
    public int StringToHash(string parameterName)
    {
        if (string.IsNullOrEmpty(parameterName)) return 0;
        if (!targetAnimator) return 0;
        foreach (var param in targetAnimator.parameters)
        {
            if (param.name == parameterName)
                return param.nameHash;
        }

        Debug.LogWarning($"파라미터 없음: {parameterName}");
        return 0;
    }
}
