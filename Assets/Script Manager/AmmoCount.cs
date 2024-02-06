using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : GamiManagers
{
    public static AmmoCount occurrence;

    [Header("Update Text")]
    public Text ammunitionText;
    public Text magText;



    protected override void Awake()
    {
        base.Awake();
        occurrence = this;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadAmmunitionText();
        this.LoadMagText();
    }

    protected virtual void LoadMagText()
    {
        if (this.magText != null) return;
        GameObject magTextObject = GameObject.Find("MagText");

        if (magTextObject != null)
        {
            this.magText = magTextObject.GetComponent<Text>();

            if (this.magText == null)
            {
                Debug.LogError("Text component not found in MagText.");
            }
        }
        else
        {
            Debug.LogError("MagText object not found in the hierarchy.");
        }
    }

    protected virtual void LoadAmmunitionText()
    {
        if (this.ammunitionText != null) return;
        GameObject ammoTextObject = GameObject.Find("AmmoText");

        if (ammoTextObject != null)
        {
            this.ammunitionText = ammoTextObject.GetComponent<Text>();

            if (this.ammunitionText == null)
            {
                Debug.LogError("Text component not found in AmmoText.");
            }
        }
        else
        {
            Debug.LogError("AmmoText object not found in the hierarchy.");
        }
    }


    public void UpdateAmmoText(int presentAmmunition)
    {
        ammunitionText.text = "Ammo. " + presentAmmunition;
    }

    public void UpdateMagText(int mag)
    {
        magText.text = "Magazines. " + mag;
    }
}