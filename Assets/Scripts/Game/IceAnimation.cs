using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IceAnimation : MonoBehaviour
{
    public SpriteRenderer foam;
    public SpriteRenderer wave;
    public SpriteRenderer waveDroplets;
    // Start is called before the first frame update
    void Start()
    {
        foam.DOFade (0.5f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        wave.transform.DOLocalMoveY(-1.8f, 0.9f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

    }

    public void AnimateWater(Vector3 position)
    {
        waveDroplets.transform.localPosition = position;
        waveDroplets.transform.localScale = Vector3.one * 1.3f;
        waveDroplets.color = new Color(1, 1, 1, 0.6f);

        waveDroplets.gameObject.SetActive(true);
        waveDroplets.DOFade(0, 2f);
        waveDroplets.transform.DOScale(0.5f, 2f).OnComplete(()=> {
            waveDroplets.gameObject.SetActive(false);
        });
    }
    //// Update is called once per frame
    //void Update()
    //{

    //}
}
