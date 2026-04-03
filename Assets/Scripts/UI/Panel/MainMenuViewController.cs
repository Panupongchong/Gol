using UnityEngine.UI;

public class MainMenuViewController : BaseViewController
{
    public Button StartButton;
    public Button PvpButton;

    protected override void OnEnable()
    {
        StartButton.onClick.AddListener(StartGame);
        PvpButton.onClick.AddListener(ShowPvpMenu);
    }

    protected override void OnDisable()
    {
        StartButton.onClick.RemoveListener(StartGame);
        PvpButton.onClick.RemoveListener(ShowPvpMenu);
    }

    public void StartGame()
    {
        SoundController.Instance.PlaySound("Press");
        GameMasterController.Instance.StartGame();
        //fade out
        gameObject.SetActive(false);
    }

    public void ShowPvpMenu()
    {
        var signedIn = FirebaseController.Instance.signedIn;
        if (signedIn)
        {
            //Show pvp menu
            UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(true);
        }
        else
        {
            //Show pvp login
            UiMasterController.Instance.PvpLoginPanel.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    protected override void OnBackButton()
    {
        SoundController.Instance.PlaySound("Back");
        //exit?
    }
}
