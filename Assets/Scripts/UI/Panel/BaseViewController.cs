using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class BaseViewController : MonoBehaviour
{

    public Button BackButton;
    public Button SettingButton;

    protected virtual void OnBackButton()
    {
        Debug.Log("Going back");
    }

    protected virtual void OnSettingButton()
    {
        Debug.Log("Showing settings");
    }

    protected virtual void OnEnable()
    {
        if (BackButton) BackButton.onClick.AddListener(OnBackButton);
        if (SettingButton) SettingButton.onClick.AddListener(OnSettingButton);
    }

    protected virtual void OnDisable()
    {
        if (BackButton) BackButton.onClick.RemoveListener(OnBackButton);
        if (SettingButton) SettingButton.onClick.RemoveListener(OnSettingButton);
    }
}