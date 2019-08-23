using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SharkController : MonoBehaviour
{
    public SpriteRenderer shark;

    //public Sprite sharkUnderWater;
    //public Sprite sharkJump;
    public Sprite sharkInAir;

    public Transform splash;
    public void Animate(Vector3 startingPos)
    {
        shark.maskInteraction = SpriteMaskInteraction.None;
        splash.transform.localScale = Vector3.zero;

        shark.transform.position = startingPos;
        shark.transform.DOKill();
        shark.flipX = startingPos.x < 0 ? true : false;

        // set under water frame
        shark.sprite = UIRefs.Instance.sharkUnderWater;
        shark.gameObject.SetActive(true);

        float posX = shark.transform.position.x;

        List<Vector3> tornadoPath = new List<Vector3>();
        tornadoPath.Add(new Vector3(posX, -2f, 0));
        tornadoPath.Add(new Vector3(posX, 0f, 0));

        float speed = 10;
        //float speedInAir = 7;
        Vector3 posY1 = new Vector3(-1.7f, - 1.75f,0);
        Vector3 posY2 = new Vector3(-1.5f, 4f, 0);// 3f;
        Vector3 posY3 = new Vector3(-1f, 7f, 0);// 7f;
        if(startingPos.x > 0)
        {
             posY1 = new Vector3(1.25f, -1.75f, 0);
             posY2 = new Vector3(1f, 3.5f, 0);
             posY3 = new Vector3(0.75f, 7f, 0);
        }

        float time1 = Vector3.Distance( posY1,startingPos) / speed;
        float time2 = Vector3.Distance(posY2,posY1) / speed;
        float time3 = Vector3.Distance(posY3,posY2) / speed;

        float totalTime = time1 + time2 + time3;// + time4 + time5;
        //Debug.Log("total time " + totalTime);

        float animationTime = 0.2f;
        Invoke("PlaySharkSound", time1);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(shark.transform.DOMove(posY1, time1).SetEase(Ease.Linear));

        sequence.InsertCallback(time1,() => {
             AnimationController.Instance.PlayAnimation(OnAnimationComplete, shark, -1, CharacterAnimtaionType.SharkJump, false, animationTime);

            // set Jump frame
            //            shark.sprite = sharkInAir;
            //          shark.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            //shark.transform.DOLocalRotate(Vector3.left * 40, 0.1f).SetEase(Ease.Linear);
            //shark.transform.DOLocalRotate(Vector3.left * 0, time2 + time3).SetDelay(0.1f).SetEase(Ease.Linear);
        });
        sequence.Append(shark.transform.DOMove(posY1, animationTime).SetEase(Ease.Linear));

        sequence.Append(shark.transform.DOMove(posY2, time2).SetEase(Ease.Linear));
        sequence.InsertCallback(time2+time1 + animationTime, () =>
        {
            shark.maskInteraction = SpriteMaskInteraction.None;
            shark.sprite = UIRefs.Instance.sharkUnderWater;
            splash.transform.DOScale(1.1f, 0.25f).SetEase(Ease.OutBack);
            shark.flipX = !shark.flipX;

        });

        sequence.InsertCallback(time1 + time2 * 0.5f+ animationTime, () =>
        {
            shark.flipX = !shark.flipX;
        });

        sequence.Append(shark.transform.DOMove(posY3, time3).SetEase(Ease.Linear));
        sequence.InsertCallback( time1 + time2 + 0.25f + animationTime, () =>
        {
            splash.transform.DOScale(0, 0.01f);
        });

        sequence.AppendCallback(() =>
        {
            shark.gameObject.SetActive(false);
        });
        sequence.Play();
    }

    private void OnAnimationComplete(bool status)
    {
    }

    private void PlaySharkSound()
    {
        SoundController.Instance.PlaySfx(Sfx.SharkDeath, 0.5f);
    }

  
}
