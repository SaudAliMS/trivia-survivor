using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuViewController : MonoBehaviour
{
    public Text coinsCount;
    public Text xpCount;
    public Text levelCount;
    public Slider levelProgress;

    public void Open()
    {
        UpdateUI();
        gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        coinsCount.text = PlayerData.CoinsCount.ToString();
        xpCount.text = PlayerData.XPCount.ToString() + "/" + GameplayController.Instance.GetRequiredXPForLevelUpdate();
        levelCount.text = "Lv " + PlayerData.Level.ToString();
        levelProgress.value = (PlayerData.XPCount / GameplayController.Instance.GetRequiredXPForLevelUpdate());
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnPressStartGame()
    {
        GameplayController.Instance.OnPressTapToStartBtn();
    }
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


}
