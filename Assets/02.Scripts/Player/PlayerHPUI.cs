using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] PlayerHP playerHP;
    [SerializeField] Image hpBar;
    [SerializeField] TextMeshProUGUI hpText;
    private void Start()
    {
        if (playerHP == null)
            playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        UpdateHP(playerHP.MaxHP, playerHP.CurrentHP);
    }
    private void OnEnable()
    {
        if (playerHP != null)
            playerHP.OnChangeHP += UpdateHP;
        UpdateHP(playerHP.MaxHP, playerHP.CurrentHP);
    }

    private void OnDisable()
    {
        if (playerHP != null)
            playerHP.OnChangeHP -= UpdateHP;
    }

    void UpdateHP(float maxValue, float currentValue)
    {
        hpBar.fillAmount = currentValue / maxValue;
        hpText.text = $"{currentValue}/{maxValue}";
    }
}
