using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer sprite;
    float posX,posY;
    int chrId;

    private bool myAnswer;
    public void SetupCharacter(int characterId, Vector3 position, bool answer)
    {
            chrId = characterId;
        myAnswer = answer;
        sprite.sprite = UIRefs.Instance.characters[characterId];

        transform.localPosition = position;//new Vector3(posX, posY, 0);
        transform.localScale = Vector3.one;
        emoticon.gameObject.SetActive(false);
                Invoke("PlayIdleAnimation", 2);
    }

    public void Shuffle(Vector3 newPos, bool answer)
    {
        myAnswer = answer;
       
        float time  = Vector3.Distance(transform.localPosition, newPos) / 8f;
        float delay = Random.Range(1f, 2.5f);

        transform.DOLocalMove(newPos, time).SetDelay(delay).SetEase(Ease.InOutSine);
    }

	public void ShuffleMyPlayer(Vector3 newPos, bool answer)
    {
        myAnswer = answer;
        //if (right)
        //{
        //    posX = Random.Range(1f, 2f);
        //    posY = Random.Range(-0.25f, 2f);
        //}
        //else
        //{
        //    posX = Random.Range(-2, -1);
        //    posY = Random.Range(-0.5f, 1.5f);
        //}

        //Vector3 newPos = new Vector3(posX, posY, 0);
        float time = Vector3.Distance(transform.localPosition, newPos) / 8f;
        float delay = Random.Range(0.25f, 2f);

        transform.DOLocalMove(newPos, time).SetEase(Ease.InOutSine);
        emoticon.transform.DOKill();
        emoticon.gameObject.SetActive(true);
        emoticon.transform.DOScale(1, 1).OnComplete(() =>
        {
            emoticon.gameObject.SetActive(false);

        });

    }
    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        localPos.z = localPos.y;
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
        transform.DOKill();
        animSequence = DOTween.Sequence();
        animSequence.Append(transform.DOScale(0.4f, 0.4f).OnComplete( delegate () {
            AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Death, false, 0.5f);
        }));
        animSequence.Append(transform.DOScale(1, 0.3f).OnComplete(delegate () {

        }));
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
