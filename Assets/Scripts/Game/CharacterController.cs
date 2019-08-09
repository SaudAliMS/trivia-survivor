using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer sprite;
    float posX,posY;

    public void SetupCharacter(int characterId)
    {
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

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        localPos.z = localPos.y/10f;
        transform.localPosition = localPos;
    }
    //public void SetupCharacter(int characterId)
    //{

    //}
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame

}
