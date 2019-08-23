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
        foam.DOFade (0.5f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        wave.transform.DOLocalMoveY(-1.9f, 1.25f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

    }

    public void AnimateWater(Vector3 position)
    {
        waveDroplets.transform.localPosition = position;
        waveDroplets.transform.localScale = Vector3.one * 1.3f;
        waveDroplets.color = new Color(1, 1, 1, 0.6f);

        waveDroplets.gameObject.SetActive(true);
        waveDroplets.DOFade(0, 1.5f);
        waveDroplets.transform.DOScale(0.5f, 1.5f).OnComplete(()=> {
            waveDroplets.gameObject.SetActive(false);
        });
    }
    //// Update is called once per frame
    //void Update()
    //{

    //}
}
