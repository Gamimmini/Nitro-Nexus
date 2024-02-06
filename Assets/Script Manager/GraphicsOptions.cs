using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptions : MonoBehaviour
{
    public static GraphicsOptions Instance;

    [Header("Graphic")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    private float minSensitivity = 10f;
    private float maxSensitivity = 500f;

    public float cameraSensitivity = 100f;


    [Header("Key")]
    private string sensitivityKey = "CameraSensitivity";
    private string volumeKey = "MasterVolume";
    private string qualityKey = "GraphicsQuality";
    private string ambientIntensityKey = "AmbientIntensity";

    [Header("Change Color Button")]
    public GameObject lowBtn;
    public GameObject mediumBtn;
    public GameObject highBtn;
    public Color qualityColor = new Color(0.0f, 0.0f, 1.0f, 0.5f);
    private void Awake()
    {
        Instance = this;
        LoadSensitivity();
        LoadVolume();
        LoadQuality();
        LoadAmbientIntensity();
    }

    private void Start()
    {
        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;
        sensitivitySlider.value = cameraSensitivity;

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = AudioListener.volume;
    }

    private void Update()
    {
        UpdateSensitivity();

        Color lowBtnColor = (QualitySettings.GetQualityLevel() == 0) ? qualityColor : Color.white;
        Color mediumBtnColor = (QualitySettings.GetQualityLevel() == 2) ? qualityColor : Color.white;
        Color highBtnColor = (QualitySettings.GetQualityLevel() == 4) ? qualityColor : Color.white;

        lowBtn.GetComponent<Image>().color = lowBtnColor;
        mediumBtn.GetComponent<Image>().color = mediumBtnColor;
        highBtn.GetComponent<Image>().color = highBtnColor;
    }

    public void UpdateSensitivity()
    {
        cameraSensitivity = sensitivitySlider.value;
        SaveSensitivity();

        AudioListener.volume = volumeSlider.value;
        SaveVolume();

        SaveAmbientIntensity();
        //Debug.Log("Volume: " + AudioListener.volume);
    }

    public void SetHighQuality()
    {
        QualitySettings.SetQualityLevel(4, true);
        AdjustGraphicsForQualityLevel();
        SaveQuality(4);
    }

    public void SetMediumQuality()
    {
        QualitySettings.SetQualityLevel(2, true);
        AdjustGraphicsForQualityLevel();
        SaveQuality(2);
    }

    public void SetLowQuality()
    {
        QualitySettings.SetQualityLevel(0, true);
        AdjustGraphicsForQualityLevel();
        SaveQuality(0);
    }

    private void AdjustGraphicsForQualityLevel()
    {
        if (QualitySettings.GetQualityLevel() == 4)
        {
            RenderSettings.ambientIntensity = 1.5f;
        }
        else if (QualitySettings.GetQualityLevel() == 2)
        {
            RenderSettings.ambientIntensity = 1f;
        }
        else if (QualitySettings.GetQualityLevel() == 0)
        {
            RenderSettings.ambientIntensity = 0.5f;
        }
    }

    private void SaveSensitivity()
    {
        PlayerPrefs.SetFloat(sensitivityKey, cameraSensitivity);
        PlayerPrefs.Save();
    }

    private void LoadSensitivity()
    {
        if (PlayerPrefs.HasKey(sensitivityKey))
        {
            cameraSensitivity = PlayerPrefs.GetFloat(sensitivityKey);
        }
        else
        {
            cameraSensitivity = 100f;
        }
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(volumeKey, volumeSlider.value);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey(volumeKey))
        {
            AudioListener.volume = PlayerPrefs.GetFloat(volumeKey);
        }
        else
        {
            AudioListener.volume = 1f;
        }
    }

    private void SaveQuality(int level)
    {
        PlayerPrefs.SetInt(qualityKey, level);
        PlayerPrefs.Save();
    }

    private void LoadQuality()
    {
        if (PlayerPrefs.HasKey(qualityKey))
        {
            int qualityLevel = PlayerPrefs.GetInt(qualityKey);
            QualitySettings.SetQualityLevel(qualityLevel, true);
            AdjustGraphicsForQualityLevel();
        }
        else
        {
            QualitySettings.SetQualityLevel(2, true);
            AdjustGraphicsForQualityLevel();
        }
    }

    private void SaveAmbientIntensity()
    {
        PlayerPrefs.SetFloat(ambientIntensityKey, RenderSettings.ambientIntensity);
        PlayerPrefs.Save();
    }

    private void LoadAmbientIntensity()
    {
        if (PlayerPrefs.HasKey(ambientIntensityKey))
        {
            RenderSettings.ambientIntensity = PlayerPrefs.GetFloat(ambientIntensityKey);
        }
        else
        {
            // Giá trị mặc định nếu không có giá trị đã lưu
            RenderSettings.ambientIntensity = 1.0f;
        }
    }
}
