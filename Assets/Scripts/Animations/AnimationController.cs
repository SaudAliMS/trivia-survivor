using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationController : SingletonMono<AnimationController>
{

    public void PlayAnimation(Action<bool> onComplete, SpriteRenderer spriteRenderer, int chrId, CharacterAnimtaionType type, bool loop, float time) 
    {
        IEnumerator coroutine = PlayAnimationCoroutine(onComplete, spriteRenderer,chrId, type, loop, time);
        StartCoroutine(coroutine);
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

    private List<Sprite> GetAnimationSprites(int CharacterID, CharacterAnimtaionType type) 
    {
        return UIRefs.Instance.characterAnimationSprites[CharacterID].animationStates[(int)type].animSprites;
    }
}
