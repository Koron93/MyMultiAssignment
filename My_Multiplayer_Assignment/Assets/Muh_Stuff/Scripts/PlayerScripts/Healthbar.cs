using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthSlider;

    // Update the health bar based on the health percentage
    public void UpdateHealthBar(float healthPercentage)
    {
        healthSlider.fillAmount = healthPercentage;
    }
}
