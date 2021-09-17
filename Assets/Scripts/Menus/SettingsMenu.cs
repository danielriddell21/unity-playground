using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class SettingsMenu : IMenu
{
    [Space]
    public Dropdown GraphicsDropdown;
    public Slider FovSlider;
    public Slider SensitivitySlider;

    private Text _fovValue;
    private Text _sensValue;

    void Start()
    {
        GraphicsDropdown = GetComponentInChildren<Dropdown>();
        QualitySettings.names.ToList().ForEach(q => GraphicsDropdown.options.Add(new Dropdown.OptionData() { text = q }));

        _fovValue = FovSlider.GetComponentInChildren<Text>();
        _sensValue = SensitivitySlider.GetComponentInChildren<Text>();

        LoadSettingsValues();
    }

    void Update()
    {
        _fovValue.text = $"{FovSlider.value}";

        SensitivitySlider.value = (float)Math.Round(SensitivitySlider.value, 1);
        _sensValue.text = $"{SensitivitySlider.value}";
    }

    void SaveSettings()
    {
        SettingsManager.SetSetting("GraphicsPreference", GraphicsDropdown.value);
        SettingsManager.SetSetting("FieldOfViewPreference", FovSlider.value);
        SettingsManager.SetSetting("SensitivityPreference", SensitivitySlider.value);
    }

    void LoadSettingsValues()
    {
        GraphicsDropdown.value = (int)SettingsManager.GetSetting("GraphicsPreference");
        FovSlider.value = SettingsManager.GetSetting("FieldOfViewPreference");
        SensitivitySlider.value = SettingsManager.GetSetting("SensitivityPreference");
    }

    public void LoadDefaultSettings()
    {
        GraphicsDropdown.value = (int)SettingsManager.defaultSettings["GraphicsPreference"];
        FovSlider.value = SettingsManager.defaultSettings["FieldOfViewPreference"];
        SensitivitySlider.value = SettingsManager.defaultSettings["SensitivityPreference"];
    }

    public void SetGraphics(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void Controls()
    {
        CurrentPage.SetActive(false);
        Page1.SetActive(true);
    }

    public void Back()
    {
        SaveSettings();

        CurrentPage.SetActive(false);
        BackPage.SetActive(true);
    }
}
