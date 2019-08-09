using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveController : MonoBehaviour
{
    public GameObject wavePrefab;
    //// Start is called before the first frame update
    void Start()
    {
        for(int count =0; count < 5; count++)
        {
            GameObject waveGO = Instantiate(wavePrefab);
            waveGO.transform.SetParent(transform);
            float posX = Random.Range(-4f, 2f);
            float posY = Random.Range(-5f, 4f);
            Vector3 initialPos = new Vector3(posX, posY, 0);
            waveGO.transform.localPosition = initialPos;
            Vector3 finalPos = new Vector3(4, posY, 0);

            int index = Random.Range(0, UIRefs.Instance.waves.Count); 
            waveGO.GetComponent<SpriteRenderer>().sprite = UIRefs.Instance.waves[index];
            waveGO.SetActive(true);


            float speed = Random.Range(0.1f, 0.5f);
            float time = Vector3.Distance(finalPos, initialPos) / speed;
            waveGO.transform.DOLocalMove(finalPos, time).SetEase(Ease.Linear).OnComplete(()=> {
                MoveWaves(waveGO);
            });
            waveGO.transform.DOLocalRotate(Vector3.forward * 10, 2, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);

        }
    }


    void MoveWaves(GameObject waveGO)
    {
        float posX = -4; //Random.Range(-4f, 2f);
        float posY = Random.Range(-5f, 4f);
        Vector3 initialPos = new Vector3(posX, posY, 0);
        waveGO.transform.localPosition = initialPos;
        Vector3 finalPos = new Vector3(4, posY, 0);

        float speed = Random.Range(0.1f, 0.5f);
        float time = Vector3.Distance(finalPos, initialPos) / speed;

        waveGO.transform.DOLocalMove(finalPos, time).SetEase(Ease.Linear).OnComplete(() => {
            MoveWaves(waveGO);
        });
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
