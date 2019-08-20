using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuViewController : MonoBehaviour
{
    public Text coinsCount;
    public Text xpCount;
    public Text levelCount;
    public Slider levelProgress;
    public Transform tapToStart;
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
        levelProgress.value = ((float)PlayerData.XPCount / (float)GameplayController.Instance.GetRequiredXPForLevelUpdate());

        tapToStart.DOKill();
        tapToStart.DOScale(0.925f, 0.85f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
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
