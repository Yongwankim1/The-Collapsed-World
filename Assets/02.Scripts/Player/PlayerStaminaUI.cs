using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField] PlayerStamina playerStamina;
    [SerializeField] Image staminaBar;
    [SerializeField] Color originColor;
    [SerializeField] Color staminaDepletedColor;
    [SerializeField] TextMeshProUGUI staminaText;
    private void Awake()
    {
        if(playerStamina == null)
            playerStamina = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStamina>();
    }
    private void OnEnable()
    {
        if (playerStamina != null)
            playerStamina.OnChangeStamina += UpdateSP;
        UpdateSP(100, 100);
    }

    private void OnDisable()
    {
        if (playerStamina != null)
            playerStamina.OnChangeStamina -= UpdateSP;
    }

    void UpdateSP(float maxValue, float currentValue)
    {
        staminaBar.fillAmount = currentValue / maxValue;
        if (playerStamina.IsStaminaDepleted)
        {
            staminaBar.color = staminaDepletedColor;
        }
        else
        {
            staminaBar.color = originColor;
        }
        staminaText.text = $"{currentValue:00}/{maxValue}";
    }


}
