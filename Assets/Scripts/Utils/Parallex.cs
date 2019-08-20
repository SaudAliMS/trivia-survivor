using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Parallex : MonoBehaviour
{
    public Transform ice;
    private Material material;
//    public SpriteRenderer sRenderer;
    private float speed = -0.05f;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(ice.DOLocalRotate(Vector3.right * 3,0.5f).SetEase(Ease.InOutSine));
        mySequence.Append(ice.DOLocalRotate(Vector3.left * 3, 1f).SetEase(Ease.InOutSine));
        mySequence.Append(ice.DOLocalRotate(Vector3.right * 3, 1f).SetEase(Ease.InOutSine));
        mySequence.SetLoops(-1,LoopType.Yoyo);
        mySequence.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(0, Time.time * speed);
        //        sRenderer.material.mainTextureOffset = offset;

        material.mainTextureOffset = offset;
    }
}
