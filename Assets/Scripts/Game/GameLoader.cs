using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //PlayerData.LoadState();
        //GameController.Instance.LoadGameData();

        GameplayController.Instance.LoadLevel();

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
