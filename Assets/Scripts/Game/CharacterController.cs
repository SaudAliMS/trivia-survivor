using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer sprite;

    public SpriteRenderer emoticon, otherEmoticon, glow;
    public TextMesh emoticonText, otherEmoticonText;
    public SpriteRenderer waterSplash;
    public Transform mask;

    float posX,posY;
    int chrId;

    public bool isDying;
    private bool myAnswer;
    private bool isOtherPlayer;
    public void SetupCharacter(int characterId, Vector3 newPos, bool answer, bool otherPlayer = true)
    {
        mask.gameObject.SetActive(false);
        isDying = false;
        isOtherPlayer = otherPlayer;
        //newPos += Vector3.down * 0.8f;
        chrId = characterId;
        myAnswer = answer;

        sprite.sprite = UIRefs.Instance.characterAnimationSprites[characterId].shiveringFrames[0];

        transform.position = newPos;//new Vector3(posX, posY, 0);
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

    private void HideEmoticons()
    {
        glow.gameObject.SetActive(false);
        emoticon.gameObject.SetActive(false);
        otherEmoticon.gameObject.SetActive(false);
    }

    public void Shuffle(Vector3 newPos, bool answer)
    {
        PlayWalkAnimation(); 

        //newPos += Vector3.down * 0.8f;
        myAnswer = answer;
       
        float time  = Vector3.Distance(transform.position, newPos) / 8f;
        float delay = Random.Range(1f, 1.5f);

        transform.DOKill();
        transform.DOMove(newPos, time).SetDelay(delay).SetEase(Ease.InOutSine).OnComplete(() => {
            PlayIdleAnimation();
        });


        int showText = Random.Range(0, 100);
        if (showText < 40)
        {
            otherEmoticonText.text = Utility.GetOtherPlayerText();
            otherEmoticon.transform.DOKill();

            float animDelay = Random.Range(0f, 1f);
            otherEmoticon.transform.DOScale(1, 0.01f).SetDelay(animDelay).OnComplete(() =>
            {
                otherEmoticon.gameObject.SetActive(true);

            });
            otherEmoticon.transform.DOScale(1, 4).SetDelay(animDelay).OnComplete(() =>
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

        transform.DOMove(newPos, time).SetEase(Ease.InOutSine).OnComplete(()=> {
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
        HideEmoticons();
        mask.transform.localPosition = Vector3.down * 1;
        mask.gameObject.SetActive(true);
        mask.transform.DOLocalMoveY(0.5f, 0.2f);
        mask.transform.DOLocalMoveY(-1, 0.4f).SetDelay(0.2f).OnComplete(() =>
        {
            mask.gameObject.SetActive(false);
        });

        waterSplash.DOKill();
        waterSplash.color = Color.white;
        waterSplash.transform.localScale = Vector3.zero;
        waterSplash.gameObject.SetActive(true);

        float posY = transform.localPosition.y;
        glow.gameObject.SetActive(false);

        transform.DOKill();
        //transform.localEulerAngles = Vector3.back * 5;
        animSequence = DOTween.Sequence();
        //animSequence.PrependInterval(Random.Range(0.2f, 0.3f));
        animSequence.Append(transform.DOScale(0.6f, 0.2f).OnComplete(delegate () {

            AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Death, false, 0.5f);
        }));
        animSequence.Join(transform.DOLocalMoveY(posY - 0.2f, 0.2f));
        //animSequence.Join(transform.DOLocalRotate(Vector3.forward * 0, 0.2f));

        animSequence.Append(transform.DOScale(0.7f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        //animSequence.Join(transform.DOLocalRotate(Vector3.forward * 5, 0.35f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.InOutSine));
        animSequence.Join(transform.DOLocalMoveY(posY, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        animSequence.Play();

        float finalScale = Random.Range(0.5f, 1.5f);
        waterSplash.transform.DOScale(finalScale, 0.5f).SetDelay(0.1f).OnComplete(() =>
        {
            waterSplash.gameObject.SetActive(false);
        });

        waterSplash.DOFade(0, 0.2f).SetDelay(0.4f);
    }

    public void PlayDeathAnimation() 
    {
        HideEmoticons();
        float initialDelay = Random.Range(0.2f, 0.3f);
        mask.transform.localPosition = Vector3.down * 1;
        mask.gameObject.SetActive(true);
        mask.transform.DOLocalMoveY(0.5f, 0.2f).SetDelay(initialDelay);
        mask.transform.DOLocalMoveY(-1, 0.4f).SetDelay(0.2f+ initialDelay).OnComplete(() =>
        {
            mask.gameObject.SetActive(false);
        });

        waterSplash.DOKill();
        waterSplash.color = Color.white;
        waterSplash.transform.localScale = Vector3.zero;
        waterSplash.gameObject.SetActive(true);
        float finalScale = Random.Range(0.5f, 1.5f);
        waterSplash.transform.DOScale(finalScale, 0.5f).SetDelay(0.1f).OnComplete(() =>
        {
            waterSplash.gameObject.SetActive(false);
        });

        waterSplash.DOFade(0, 0.2f).SetDelay(0.4f);

        float posY = transform.localPosition.y;
        glow.gameObject.SetActive(false);

        transform.DOKill();
        //transform.localEulerAngles = Vector3.back * 5;
        animSequence = DOTween.Sequence();
        animSequence.PrependInterval(initialDelay);
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
        HideEmoticons();
        //transform.localRotation = Quaternion.Euler(0, 0, -1);
        //transform.DOLocalRotate(new Vector3(0, 0, 1), Random.Range(0.03f, 0.05f)).SetLoops(-1, LoopType.Yoyo);
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Lightning, false, 1f);
    }

    public void PlayStunAnimation()
    {
        transform.DOKill();
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Stun, false, 0.4f);
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
