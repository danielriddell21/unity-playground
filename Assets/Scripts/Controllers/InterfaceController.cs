using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    public static CooldownTimer SwitchCharacterCooldown;

    public Slider CooldownSlider1;

    void Start()
    {
        SwitchCharacterCooldown.TimerCompleteEvent += HideCooldownSlider;
    }

    void Update()
    {
        SwitchCharacterCooldown.Update(Time.deltaTime);

        if (SwitchCharacterCooldown.IsActive)
        {
            CooldownSlider1.value = SwitchCharacterCooldown.PercentElapsed;
        }
    }
    private void HideCooldownSlider()
    {
        Debug.Log("Timer Completed");
    }
}
