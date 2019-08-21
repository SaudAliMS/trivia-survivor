using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TornadoController : MonoBehaviour
{
    public SpriteRenderer tornado;
    public void Animate(Vector3 startingPos)
    {
        tornado.transform.position = startingPos;
        tornado.DOKill();

        tornado.color = new Color(1, 1, 1, 0f);
        tornado.gameObject.SetActive(true);

        float posX = tornado.transform.position.x;
        List<Vector3> tornadoPath = new List<Vector3>();
        tornadoPath.Add(new Vector3(posX + 0.2f, -2f, 0));
        tornadoPath.Add(new Vector3(posX - 0.25f, 0f, 0));
        tornadoPath.Add(new Vector3(posX + 0.2f, 2f, 0));
        tornadoPath.Add(Vector3.up * 7);

        tornado.transform.DOPath(tornadoPath.ToArray(), 2f, PathType.CatmullRom).OnComplete(() => {
            tornado.gameObject.SetActive(false);
            AnimationController.Instance.StopAnimation(tornado);
        });
        tornado.DOFade(1, 0.3f).SetDelay(0.25f);
        tornado.DOFade(0f, 0.3f).SetDelay(1.2f);

        AnimationController.Instance.PlayAnimation(OnAnimationComplete, tornado, -1, CharacterAnimtaionType.Tornado, true, 0.3f);

    }

    private void OnAnimationComplete(bool status)
    {
    }
}
