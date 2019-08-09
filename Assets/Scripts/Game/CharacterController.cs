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

    public void SetupCharacter(int characterId)
    {
        chrId = characterId;
        sprite.sprite = UIRefs.Instance.characters[characterId];
        int randNo = Random.Range(0, 100);
        // right
        if(randNo <  50)
        {
            posX = Random.Range(1f, 2f);
            posY = Random.Range(-0.25f, 2f);
        }
        else
        {
            posX = Random.Range(-2, -1);
            posY = Random.Range(-0.5f, 1.5f);
        }

        transform.localPosition = new Vector3(posX, posY, 0);
        transform.localScale = Vector3.one;
        //PlayIdleAnimation();
        Invoke("PlayDeathAnimation", 2);
    }

    public void Shuffle()
    {
        int randNo = Random.Range(0, 100);
        if (randNo < 50)
        {
            posX = Random.Range(1f, 2f);
            posY = Random.Range(-0.25f, 2f);
        }
        else
        {
            posX = Random.Range(-2, -1);
            posY = Random.Range(-0.5f, 1.5f);
        }
        Vector3 newPos = new Vector3(posX, posY, 0);
        float time  = Vector3.Distance(transform.position, newPos) / 3f;
        float delay = Random.Range(0.25f, 2f);

        transform.DOLocalMove(newPos, time).SetDelay(delay).SetEase(Ease.InOutSine);
    }

    Sequence deathSequence;
    public void PlayDeathAnimation() 
    {
        deathSequence = DOTween.Sequence();
        deathSequence.Append(transform.DOScale(0.4f, 0.4f).OnComplete( delegate () {
            AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Death, false, 0.5f);
        }));
        deathSequence.Append(transform.DOScale(1, 0.3f).OnComplete(delegate () {

        }));
        deathSequence.Play();

    }

    public void PlayGameOverAnimation()
    {
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.GameOver, false, 0.5f);
    }

    public void PlayIdleAnimation()
    {
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Idle, false, 0.5f);
    }

    public void PlayStunAnimation()
    {
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, sprite, chrId, CharacterAnimtaionType.Stun, true, 0.4f);
    }

    private void OnAnimationComplete(bool status) 
    {
        Debug.LogError("OnAnimationComplete: " + status);
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        localPos.z = localPos.y/10f;
        transform.localPosition = localPos;
    }

}
