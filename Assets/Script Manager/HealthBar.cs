using UnityEngine;
using UnityEngine.UI;

public class HealthBar : GamiManagers
{
    public Slider healthBarSlider;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadHealthBarSlider();

    }
    protected virtual void LoadHealthBarSlider()
    {
        if (this.healthBarSlider != null) return;

        this.healthBarSlider = GetComponentInChildren<Slider>();

        if (this.healthBarSlider == null)
        {
            Debug.LogError("Slider component not found in HealthBar or its children.");
        }
    }
    public void GiveFullHealth(float health)
    {
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
    }    
    public void SetHealth(float health)
    {
        healthBarSlider.value = health;
    }    
}
