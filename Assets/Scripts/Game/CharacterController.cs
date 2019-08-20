﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer sprite;

    public SpriteRenderer emoticon, otherEmoticon, glow;
    public TextMesh emoticonText, otherEmoticonText;

    float posX,posY;
    int chrId;

    public bool isDying;
    private bool myAnswer;
    private bool isOtherPlayer;
    public void SetupCharacter(int characterId, Vector3 newPos, bool answer, bool otherPlayer = true)
    {
        isDying = false;
        isOtherPlayer = otherPlayer;
        newPos += Vector3.down * 0.8f;
        chrId = characterId;
        myAnswer = answer;

        sprite.sprite = UIRefs.Instance.characterAnimationSprites[characterId].shiveringFrames[0];

        transform.localPosition = newPos;//new Vector3(posX, posY, 0);
        transform.localScale = Vector3.one * 0.8f;
        emoticon.gameObject.SetActive(false);
        otherEmoticon.gameObject.SetActive(false);

        SetPlayerUIStauts();
        PlayIdleAnimation();
//        Invoke("PlayIdleAnimation", 2);
    }

    public void SetPlayerUIStauts() 
    {
        if (!isOtherPlayer)
        {
            glow.gameObject.SetActive(true);
            emoticonText.text = "ME";
            emoticon.gameObject.SetActive(true);
        }
        else
        {
            glow.gameObject.SetActive(false);
            emoticon.gameObject.SetActive(false);
        }
    }

    public void Shuffle(Vector3 newPos, bool answer)
    {
        PlayWalkAnimation(); 

        newPos += Vector3.down * 0.8f;
        myAnswer = answer;
       
        float time  = Vector3.Distance(transform.localPosition, newPos) / 8f;
        float delay = Random.Range(1f, 1.5f);

        transform.DOKill();
        transform.DOLocalMove(newPos, time).SetDelay(delay).SetEase(Ease.InOutSine).OnComplete(() => {
            PlayIdleAnimation();
        });


        int showText = Random.Range(0, 100);
        if (showText < 20)
        {
            otherEmoticonText.text = Utility.GetOtherPlayerText();
            otherEmoticon.transform.DOKill();
            otherEmoticon.gameObject.SetActive(true);
            otherEmoticon.transform.DOScale(1, 1).OnComplete(() =>
            {
                otherEmoticon.gameObject.SetActive(false);

            });
        }
    }

	public void ShuffleMyPlayer(Vector3 newPos, bool answer)
    {
        PlayWalkAnimation();

        newPos += Vector3.down * 0.8f;
        myAnswer = answer;
       
        float time = Vector3.Distance(transform.localPosition, newPos) / 8f;
        float delay = Random.Range(0.25f, 2f);

        transform.DOLocalMove(newPos, time).SetEase(Ease.InOutSine).OnComplete(()=> {
            PlayIdleAnimation();
        });
        emoticonText.text = Utility.GeMyPlayerText();
        emoticon.transform.DOKill();
        emoticon.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        localPos.z = localPos.y/10f;
        if (!isOtherPlayer)
        {
            glow.transform.Rotate(Vector3.back * 2);
            //localPos.z = -1f;
        }
        if (!isDying)
        {
            transform.localPosition = localPos;
        }
    }

    public bool UserAnswerYes()
    {
        return myAnswer;
    }

    #region Animations

    Sequence animSequence;

    public void PlayFreezeAnimation()
    {
    
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Death, false, 0.5f);

        ////transform.localEulerAngles = Vector3.back * 5;
        //animSequence = DOTween.Sequence();
        //animSequence.PrependInterval(Random.Range(0.2f, 0.3f));
        //animSequence.Append(transform.DOScale(0.6f, 0.2f).OnComplete(delegate () {

        //}));
        //animSequence.Join(transform.DOLocalMoveY(posY - 0.2f, 0.2f));
        ////animSequence.Join(transform.DOLocalRotate(Vector3.forward * 0, 0.2f));

        //animSequence.Append(transform.DOScale(0.7f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        ////animSequence.Join(transform.DOLocalRotate(Vector3.forward * 5, 0.35f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.InOutSine));
        //animSequence.Join(transform.DOLocalMoveY(posY, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        //animSequence.Play();
    }

    public void PlayDeathAnimation() 
    {
        float posY = transform.localPosition.y;
        glow.gameObject.SetActive(false);

        transform.DOKill();
        //transform.localEulerAngles = Vector3.back * 5;
        animSequence = DOTween.Sequence();
        animSequence.PrependInterval(Random.Range(0.2f, 0.3f));
        animSequence.Append(transform.DOScale(0.6f, 0.2f).OnComplete( delegate () {

            AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Death, false, 0.5f);
        }));
        animSequence.Join(transform.DOLocalMoveY(posY-0.2f, 0.2f));
        //animSequence.Join(transform.DOLocalRotate(Vector3.forward * 0, 0.2f));

        animSequence.Append(transform.DOScale(0.7f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        //animSequence.Join(transform.DOLocalRotate(Vector3.forward * 5, 0.35f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.InOutSine));
        animSequence.Join(transform.DOLocalMoveY(posY , 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        animSequence.Play();
    }

    public void PlayGameOverAnimation()
    {
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.GameOver, false, 0.5f);
    }

    public void PlayIdleAnimation()
    {
        transform.localRotation = Quaternion.Euler(0,0,-1);
        transform.DOLocalRotate(new Vector3(0, 0, 1), Random.Range(0.03f,0.05f)).SetLoops(-1, LoopType.Yoyo);
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Idle, false, 0.5f);
    }

    public void PlayLightningAnimation()
    {
        //transform.localRotation = Quaternion.Euler(0, 0, -1);
        //transform.DOLocalRotate(new Vector3(0, 0, 1), Random.Range(0.03f, 0.05f)).SetLoops(-1, LoopType.Yoyo);
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Lightning, false, 1f);
    }

    public void PlayStunAnimation()
    {
        transform.DOKill();
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Stun, true, 0.4f);
    }

    public void PlayWalkAnimation()
    {
        transform.DOKill();
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Idle, true, 0.4f);
    }

    private void OnAnimationComplete(bool status) 
    {
        //Debug.LogError("OnAnimationComplete: " + status);
    }

    #endregion Animations

}
