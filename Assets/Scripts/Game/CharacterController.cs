﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer sprite;

    public SpriteRenderer emoticon, otherEmoticon;
    public TextMesh emoticonText, otherEmoticonText;

    float posX,posY;
    int chrId;

    private bool myAnswer;
    private bool isOtherPlayer;
    public void SetupCharacter(int characterId, Vector3 newPos, bool answer, bool otherPlayer = true)
    {
        isOtherPlayer = otherPlayer;
        newPos += Vector3.down * 0.8f;
        chrId = characterId;
        myAnswer = answer;

        sprite.sprite = UIRefs.Instance.characterAnimationSprites[characterId].animationStates[(int)CharacterAnimtaionType.Idle].animSprites[0];

        transform.localPosition = newPos;//new Vector3(posX, posY, 0);
        transform.localScale = Vector3.one;
        emoticon.gameObject.SetActive(false);
        otherEmoticon.gameObject.SetActive(false);

        PlayIdleAnimation();
//        Invoke("PlayIdleAnimation", 2);
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
        emoticon.transform.DOScale(1, 3).OnComplete(() =>
        {
            emoticon.gameObject.SetActive(false);
        });
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        localPos.z = localPos.y;
        if (!isOtherPlayer)
        {
            localPos.z -= 0.1f;
        }
        transform.localPosition = localPos;
    }

    public bool UserAnswerYes()
    {
        return myAnswer;
    }

    #region Animations

    Sequence animSequence;
    public void PlayDeathAnimation() 
    {
        float posY = transform.localPosition.y;
        transform.DOKill();
        //transform.localEulerAngles = Vector3.back * 5;
        animSequence = DOTween.Sequence();
        animSequence.PrependInterval(0.3f);
        animSequence.Append(transform.DOScale(0.65f, 0.2f).OnComplete( delegate () {

            AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Death, false, 0.5f);
        }));
        animSequence.Join(transform.DOLocalMoveY(posY-0.2f, 0.2f));
        //animSequence.Join(transform.DOLocalRotate(Vector3.forward * 0, 0.2f));

        animSequence.Append(transform.DOScale(0.75f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
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
