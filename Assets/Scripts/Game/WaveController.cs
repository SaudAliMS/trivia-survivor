using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveController : MonoBehaviour
{
    public SpriteRenderer wave;
    public void Animate(Vector3 startingPos)
    {
        wave.transform.position = startingPos;
        wave.DOKill();

        wave.flipX = startingPos.x < 0 ? true : false; 
        wave.gameObject.SetActive(true);

        float posX = wave.transform.position.x;
        //List<Vector3> tornadoPath = new List<Vector3>();
        //tornadoPath.Add(new Vector3(posX + 0.2f, -2f, 0));
        //tornadoPath.Add(new Vector3(posX - 0.25f, 0f, 0));
        //tornadoPath.Add(new Vector3(posX + 0.2f, 2f, 0));
        //tornadoPath.Add(Vector3.up * 7);

        Invoke("PlayWaveSound", 0.5f);
        wave.transform.DOMoveY(0.5f, 1.2f).OnComplete(() => {
            //wave.gameObject.SetActive(false);
            //AnimationController.Instance.StopAnimation(wave);
        });
        AnimationController.Instance.PlayAnimation(OnAnimationComplete, wave, -1, CharacterAnimtaionType.WaveSplash, false, 1.5f);

        wave.transform.DOScale(1, 1.25f).OnComplete(() => {
            wave.gameObject.SetActive(false);
            AnimationController.Instance.StopAnimation(wave);
        });
    }

    private void OnAnimationComplete(bool status)
    {
    }

    private void PlayWaveSound() 
    {
        SoundController.Instance.PlaySfx(Sfx.WaveDeath, 0.5f);
    }
}
