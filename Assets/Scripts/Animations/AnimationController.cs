using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnimationController : SingletonMono<AnimationController>
{
    private Dictionary<int, IEnumerator> coroutinesDictionary = new Dictionary<int, IEnumerator>();

    #region Animation for SpriteRenderer

    public void PlayAnimation(Action<bool> onComplete, SpriteRenderer spriteRenderer, int chrId, CharacterAnimtaionType type, bool loop, float time) 
    {
        int hash = spriteRenderer.GetHashCode();
        if (coroutinesDictionary.ContainsKey(hash))
        {
            IEnumerator routine = coroutinesDictionary.GetValue(hash);
            coroutinesDictionary.Remove(hash);
            StopCoroutine(routine);
        }
        IEnumerator coroutine = PlayAnimationCoroutine(onComplete, spriteRenderer,chrId, type, loop, time);
        coroutinesDictionary.Add(hash, coroutine);
        StartCoroutine(coroutine);
    }

    public void StopAnimation(SpriteRenderer spriteRenderer)
    {
        int hash = spriteRenderer.GetHashCode();
        if (coroutinesDictionary.ContainsKey(hash))
        {
            IEnumerator routine = coroutinesDictionary.GetValue(hash);
            coroutinesDictionary.Remove(hash);
            StopCoroutine(routine);
        }
    }

    private IEnumerator PlayAnimationCoroutine(Action<bool> onComplete, SpriteRenderer spriteRenderer, int chrId, CharacterAnimtaionType type,  bool loop, float time) 
    {
        List<Sprite> animationSprites = GetAnimationSprites(chrId, type);
        float frameDelay = (time / animationSprites.Count);

        if(loop && animationSprites.Count > 1)
        {
            while (true)
            {
                for (int i = 0; i < animationSprites.Count; i++)
                {
                    spriteRenderer.sprite = animationSprites[i];
                    yield return new WaitForSeconds(frameDelay);
                }
            }
        }
        else 
        {
            for (int i = 0; i < animationSprites.Count; i++)
            {
                spriteRenderer.sprite = animationSprites[i];
                yield return new WaitForSeconds(frameDelay);
            }
        }
        onComplete.Invoke(true);
    }

    #endregion Animation for SpriteRenderer

    #region Animation for Image

    public void PlayAnimation(Action<bool> onComplete, Image image, int chrId, CharacterAnimtaionType type, bool loop, float time)
    {
        int hash = image.GetHashCode();
        if (coroutinesDictionary.ContainsKey(hash))
        {
            IEnumerator routine = coroutinesDictionary.GetValue(hash);
            coroutinesDictionary.Remove(hash);
            StopCoroutine(routine);
        }
        IEnumerator coroutine = PlayAnimationCoroutine(onComplete, image, chrId, type, loop, time);
        coroutinesDictionary.Add(hash, coroutine);
        StartCoroutine(coroutine);
    }

    public void StopAnimation( Image image)
    {
        int hash = image.GetHashCode();
        if (coroutinesDictionary.ContainsKey(hash))
        {
            IEnumerator routine = coroutinesDictionary.GetValue(hash);
            coroutinesDictionary.Remove(hash);
            StopCoroutine(routine);
        }
    }

    private IEnumerator PlayAnimationCoroutine(Action<bool> onComplete, Image image, int chrId, CharacterAnimtaionType type, bool loop, float time)
    {
        List<Sprite> animationSprites = GetAnimationSprites(chrId, type);
        float frameDelay = (time / animationSprites.Count);

        if (loop && animationSprites.Count > 1)
        {
            while (true)
            {
                for (int i = 0; i < animationSprites.Count; i++)
                {
                    image.sprite = animationSprites[i];
                    yield return new WaitForSeconds(frameDelay);
                }
            }
        }
        else
        {
            for (int i = 0; i < animationSprites.Count; i++)
            {
                image.sprite = animationSprites[i];
                yield return new WaitForSeconds(frameDelay);
            }
        }
        onComplete.Invoke(true);
    }

    #endregion Animation for Image

    private List<Sprite> GetAnimationSprites(int CharacterID, CharacterAnimtaionType type) 
    {
        switch(type)
        {
            case CharacterAnimtaionType.Death:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].frozenFrames;

            case CharacterAnimtaionType.GameOver:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].cryingFrames;

            case CharacterAnimtaionType.Idle:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].shiveringFrames;

            case CharacterAnimtaionType.Lightning:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].lightningFrames;

            case CharacterAnimtaionType.Stun:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].shockFrames;

            case CharacterAnimtaionType.GameWin:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].gameWinFrames;

            case CharacterAnimtaionType.Tornado:
                return UIRefs.Instance.tornado;

            case CharacterAnimtaionType.SharkJump:
                return UIRefs.Instance.shark;
            default:
                return UIRefs.Instance.characterAnimationSprites[CharacterID].shiveringFrames;

        }

    }
}
