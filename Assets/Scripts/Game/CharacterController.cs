using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer sprite;
    float posX,posY;
    public SpriteRenderer emoticon;
    private bool myAnswer;
    public void SetupCharacter(int characterId, Vector3 position, bool answer)
    {
        myAnswer = answer;
        sprite.sprite = UIRefs.Instance.characters[characterId];
        int randNo = Random.Range(0, 100);
        //// right
        //if(randNo <  50)
        //{
        //    posX = Random.Range(1f, 2f);
        //    posY = Random.Range(-0.25f, 2f);
        //}
        //else
        //{
        //    posX = Random.Range(-2, -1);
        //    posY = Random.Range(-0.5f, 1.5f);
        //}

        transform.localPosition = position;//new Vector3(posX, posY, 0);
        transform.localScale = Vector3.one;
        emoticon.gameObject.SetActive(false);
    }

    public void Shuffle(Vector3 newPos, bool answer)
    {
        myAnswer = answer;
        //int randNo = Random.Range(0, 100);
        //if (randNo < 50)
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
    //public void SetupCharacter(int characterId)
    //{

    //}
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame

}
