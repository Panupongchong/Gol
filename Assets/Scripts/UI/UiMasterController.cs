using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class UiMasterController : MonoBehaviour {
	public static UiMasterController _instance;
	public static UiMasterController Instance
	{
		get {
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<UiMasterController>();
			}

			return _instance;
		}
	}

	public MainMenuViewController MainMenuPanel;
    public ResultViewController ResultPanel;
    public PvpLoginViewController PvpLoginPanel;
    public PvpMenuViewController PvpMenuPanel;
    public PvpResultViewController PvpResultPanel;

    public void ShowMainMenu(){
        MainMenuPanel.gameObject.SetActive(true);
    }

	public void ShowResult(int score, int combo, int bonus, int best, string matchId = ""){
		ResultPanel.SetScore (score);
		ResultPanel.SetCombo (combo);
		ResultPanel.SetBonus (bonus);
		ResultPanel.SetBestScore (best);
        ResultPanel.SetMatchId(matchId);
		ResultPanel.gameObject.SetActive (true);
	}

    public void LoadSpriteFromWww(string url, Action<Sprite> callback){
        string filePath = Application.persistentDataPath;
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(url);
        filePath += "/" + System.Convert.ToBase64String(plainTextBytes);
        string loadFilepath = filePath;
        bool web = false;
        WWW www;
        bool useCached = false;
        useCached = System.IO.File.Exists(filePath);
        if (useCached)
        {
            //check how old
            System.DateTime written = File.GetLastWriteTimeUtc(filePath);
            System.DateTime now = System.DateTime.UtcNow;
            double totalHours = now.Subtract(written).TotalHours;
            if (totalHours > 300)
                useCached = false;
        }
        if (useCached)
        {
            string pathforwww = "file://" + loadFilepath;
            Debug.Log("TRYING FROM CACHE " + url + "  file " + pathforwww);
            www = new WWW(pathforwww);
        }
        else
        {
            web = true;
            www = new WWW(url);
        }
        StartCoroutine(loadFromWww(www, filePath, web, callback));
    }

    private IEnumerator loadFromWww(WWW www, string filePath, bool web, Action<Sprite> callback){
        yield return www;
        if (www.error == null)
        {
            if (web)
            {
                //System.IO.Directory.GetFiles
                Debug.Log("SAVING DOWNLOAD  " + www.url + " to " + filePath);
                // string fullPath = filePath;
                File.WriteAllBytes(filePath, www.bytes);
                Debug.Log("SAVING DONE  " + www.url + " to " + filePath);
                //Debug.Log("FILE ATTRIBUTES  " + File.GetAttributes(filePath));
                //if (File.Exists(fullPath))
                // {
                //    Debug.Log("File.Exists " + fullPath);
                // }
            }
            else
            {
                Debug.Log("SUCCESS CACHE LOAD OF " + www.url);
            }
            callback(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0)));
        }
        else
        {
            if (!web)
            {
                File.Delete(filePath);
            }
            Debug.Log("WWW ERROR " + www.error);
        }
    }
}

