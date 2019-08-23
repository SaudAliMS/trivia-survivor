using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IceAnimation : MonoBehaviour
{
    public SpriteRenderer foam;
    public SpriteRenderer wave;
    // Start is called before the first frame update
    void Start()
    {
        foam.transform.DOLocalMoveY(-1.5f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        wave.transform.DOLocalMoveY(-1.9f, 1.25f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
