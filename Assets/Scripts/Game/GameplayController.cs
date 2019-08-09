﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameplayController : SingletonMono<GameplayController>
{
    public SpriteRenderer snowDust;
    public List<Transform> iceContainer;
    public List<Transform> icePieces;

    public Transform stone;
    public Transform characterContainer;
    public GameObject characterPrefab;
    List<LevelData> levelData;
    int questionIndex;
    bool canAnswer = false;

    bool gamePaused = false;
    bool timerOn = false;
    float timerValue;

    private List<CharacterController> characterCache;
    private List<CharacterController> characterList;
    private CharacterController myCharacter;
    #region private methods

    void ResetCharacters()
    {
        if(characterList == null)
        {
            characterCache = new List<CharacterController>();
            characterList = new List<CharacterController>();
        }

        if (icePieces == null)
        {
            icePieces = new List<Transform>();
        }
        icePieces.Clear();

        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            characterController.gameObject.SetActive(false);
            characterCache.Add(characterController);

        }

        if(myCharacter != null)
        {
            myCharacter.gameObject.SetActive(false);
            characterCache.Add(myCharacter);
            myCharacter = null;
        }

        for (int count = 0; count < iceContainer.Count; count++)
        {
            Transform ice = iceContainer[count];
            ice.gameObject.SetActive(true);
            icePieces.Add(ice);
            ice.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        characterList.Clear();
    }


    CharacterController GetCharacter()
    {
        if (characterCache.Count > 0)
        {
            CharacterController characterController = characterCache[0];
            characterController.gameObject.SetActive(true);
            characterCache.RemoveAt(0);
            return characterController;
        }
        else
        {
            GameObject characterGO = Instantiate(characterPrefab);
            CharacterController characterController = characterGO.GetComponent<CharacterController>();
            characterGO.gameObject.SetActive(true);
            return characterController;
        }
    }

    void ThrowStone()
    {
        canAnswer = false;
        timerOn = false;
        //bool userAnswerYes = myCharacter.UserAnswerYes();
        Vector3 stonePosition;
        Transform icePiece;

        if (levelData[questionIndex].AnswerIsTrue) {
            icePiece = icePieces[icePieces.Count - 1];
            icePieces.RemoveAt(icePieces.Count - 1);
        }
        else{
            icePiece = icePieces[0];
            icePieces.RemoveAt(0);
        }

        SpriteRenderer spriteRenderer = icePiece.GetComponent<SpriteRenderer>();
        float posX = icePiece.transform.position.x;
        stonePosition = new Vector3(posX,7,0);

        stone.localPosition = stonePosition;
        stone.gameObject.SetActive(true);

        stone.transform.DOLocalMoveY(0.5f,0.4f).SetEase(Ease.Linear).OnComplete(() =>
        {
            snowDust.transform.localPosition = stone.transform.localPosition + Vector3.down * 0.4f; 
            snowDust.transform.localScale = Vector3.one * 0.2f;
            snowDust.gameObject.SetActive(true);
            snowDust.DOFade(0.3f, 0.5f).SetEase(Ease.InSine);
            snowDust.transform.DOScale(1, 0.5f).SetEase(Ease.Linear).OnComplete(()=> {
                snowDust.gameObject.SetActive(false);
                snowDust.color = Color.white;
            });

            stone.gameObject.SetActive(false);
            icePiece.DOMoveZ(1, 0.25f).SetEase(Ease.Linear);
            spriteRenderer.DOFade(0, 0.25f).SetEase(Ease.Linear);
            AnalytizeUserAnswer();
        });
    }

    void QuestionCompleted()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        questionIndex++;
        if (questionIndex < levelData.Count)
        {
            LoadQuestion();
            float posX = Camera.main.transform.position.x;
            posX = answerIsTrue ? posX - 0.7f : posX + 0.7f;
            Camera.main.transform.DOMoveX(posX, 1f).SetDelay(0.5f);
            Camera.main.DOFieldOfView(Camera.main.fieldOfView - 5, 0.6f).SetDelay(0.5f);
        }
        else
        {
            timerOn = false;
            GameController.Instance.LevelCompleted();
            ViewController.Instance.OpenView(Views.LevelComplete);
        }
    }

    void AnalytizeUserAnswer()
    {
        canAnswer = false;
        RemoveFallenCharaceters();
        bool userAnswerYes = myCharacter.UserAnswerYes();
        if ((userAnswerYes && levelData[questionIndex].AnswerIsTrue) || 
            (!userAnswerYes && !levelData[questionIndex].AnswerIsTrue))
        {
            QuestionCompleted();
        }
        else
        {
            QuestionFailed();
        }
    }

    void RemoveFallenCharaceters()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        for (int count = 0; count < characterList.Count; count ++)
        {
            CharacterController characterController =  characterList[count];
            bool userAnswerYes = characterController.UserAnswerYes();
            if((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
            {
                characterController.gameObject.SetActive(false);
                characterCache.Add(characterController);
                characterList.RemoveAt(count);
                count--;
            }
        }

        bool userAnswerYesOwn = myCharacter.UserAnswerYes();
        if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
        {
            myCharacter.gameObject.SetActive(false);
            characterCache.Add(myCharacter);
        }
    }



    void QuestionFailed()
    {
        canAnswer = false;
        timerOn = false;

        ViewController.Instance.gameOverViewController.ShowAnswer(levelData[questionIndex].AnswerStatement);
        ViewController.Instance.OpenView(Views.LevelFailed);
    }

    void Update()
    {
        if(timerOn && !gamePaused)
        {
            timerValue -= Time.deltaTime;
            if (timerValue <= 0)
            {
                ViewController.Instance.gameplayViewController.HideTimer();
//                AnalytizeUserAnswer();
                ThrowStone();
            }
            else
            {
                //float questionTime = GameConstants.QUESTION_TIME - timerValue;
                //questionTime = Mathf.Clamp(questionTime, 0, GameConstants.QUESTION_TIME);
                ViewController.Instance.gameplayViewController.timer.text = "" + ((int)timerValue +1);
            }
        }
    }

    void PopulateCharacters()
    {
        Utility.ResetPositionForCharacter();

        // right characters
        for(int count= 0; count < 5; count++)
        {
            Vector3 rightCenterPos = GetRightSidePosition(); // new Vector3(1.25f, 0.5f, 0);
            Vector3 rightSidePos = Utility.GetPositionForCharacter(rightCenterPos);
            CharacterController characterController = GetCharacter();
            int characterId = Random.Range(0, UIRefs.Instance.characters.Count);
            characterController.transform.SetParent(characterContainer);
            characterController.SetupCharacter(characterId, rightSidePos,false);
            characterList.Add(characterController);
        }

        Utility.ResetPositionForCharacter();
        // left characters
        for (int count = 0; count < 5; count++)
        {
            Vector3 leftCenterPos = GetLeftSidePosition();//new Vector3(-1.25f,0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacter(leftCenterPos);

            CharacterController characterController = GetCharacter();
            int characterId = Random.Range(0, UIRefs.Instance.characters.Count);
            characterController.transform.SetParent(characterContainer);
            characterController.SetupCharacter(characterId, leftSidePos,true);
            characterList.Add(characterController);
        }

        Vector3 leftCenterPos1 = GetLeftSidePosition();//new Vector3(-1.25f, 0.5f, 0);
        Vector3 leftSidePos1 = Utility.GetPositionForCharacter(leftCenterPos1);

        myCharacter = GetCharacter();
        int myCharacterId = 0;
        myCharacter.transform.SetParent(characterContainer);
        myCharacter.SetupCharacter(myCharacterId, leftSidePos1, true);
    }

    void ShuffleCharacters()
    {
        // create temp list
        List<int> tempList = new List<int>();
        for (int count = 0; count < characterList.Count; count++)
        {
            tempList.Add(count);
        }

        List<int> characterMovingRight = new List<int>();
        List<int> characterMovingLeft  = new List<int>();

        bool killAllOtherPlayers =  false;
        if( questionIndex >= levelData.Count-1)
        {
            Debug.Log("Kill all other players");
            killAllOtherPlayers = true;
        }


        int halfCharacters = tempList.Count / 2;
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        if (killAllOtherPlayers && answerIsTrue)
        {
            Debug.Log("Kill all other players");
            halfCharacters = tempList.Count;
        }
        else if (killAllOtherPlayers && !answerIsTrue)
        {
            Debug.Log("Kill all other players");
            halfCharacters = 0;
        }

        // addd half moving right
        for (int count = 0; count < halfCharacters; count++)
        {
            int randomNo = Random.Range(0, tempList.Count);
            tempList.RemoveAt(randomNo);
            characterMovingRight.Add(randomNo);
        }

        // addd half moving left
        for (int count = 0; count < tempList.Count; count++)
        {
            int index  = tempList[count];
            tempList.RemoveAt(count);
            characterMovingLeft.Add(index);
        }


        // right characters
        Utility.ResetPositionForCharacter();
        for (int count = 0; count < characterMovingRight.Count; count++)
        {
            Vector3 rightCenterPos = GetRightSidePosition();//new Vector3(1.25f, 0.5f, 0);
            Vector3 rightSidePos = Utility.GetPositionForCharacter(rightCenterPos);

            int index = characterMovingRight[count];
            CharacterController characterController = characterList[index];
            characterController.Shuffle(rightSidePos,false);
        }

        Utility.ResetPositionForCharacter();
        // left characters
        for (int count = 0; count < characterMovingLeft.Count; count++)
        {
            Vector3 leftCenterPos = GetLeftSidePosition();//new Vector3(-1.25f, 0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacter(leftCenterPos);

            int index = characterMovingLeft[count];
            CharacterController characterController = characterList[index];
            characterController.Shuffle(leftSidePos,true);
        }
    }

    #endregion

    #region public methods
    public void LoadLevel()
    {
        Camera.main.fieldOfView = 60;
        Camera.main.transform.position = Vector3.back * 10;
        ViewController.Instance.OpenView(Views.MainMenu);
        ResetCharacters();
        questionIndex = 0;

        levelData = GameData.Instance.GetLevelData(GameController.Instance.currentLevel);
        PopulateCharacters();
    }

    public void LoadQuestion()
    {
        timerValue = GameConstants.QUESTION_TIME;
        timerOn = true;
        ViewController.Instance.OpenView(Views.GamePlay);
        ViewController.Instance.gameplayViewController.ShowTimer();
        ViewController.Instance.gameplayViewController.ShowQuestion(levelData[questionIndex].Question);
        gamePaused = false;
        canAnswer = true;
        ShuffleCharacters();
    }

    public void OnPressYesBtn()
    {
        if (canAnswer)
        {
            Vector3 leftCenterPos = GetLeftSidePosition();//new Vector3(-1.25f, 0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacter(leftCenterPos);

            //canAnswer = false;
            myCharacter.ShuffleMyPlayer(leftSidePos,true);
        }
    }

    public void OnPressNoBtn()
    {
        if (canAnswer)
        {
            Vector3 rightCenterPos = GetRightSidePosition();//new Vector3(1.25f, 0.5f, 0);
            Vector3 rightSidePos = Utility.GetPositionForCharacter(rightCenterPos);

            //canAnswer = false;
            myCharacter.ShuffleMyPlayer(rightSidePos,false);
        }
    }

    public void OnPressPauseBtn()
    {
        gamePaused = !gamePaused;
    }

    public bool IsGamePaused()
    {
        return gamePaused;
    }

    #endregion

    Vector3 GetRightSidePosition()
    {
        Transform icePiece = icePieces[icePieces.Count - 1];
        if (icePieces.Count > 2)
        {
            return icePiece.position + Vector3.up * 0.5f + Vector3.left * 0.5f;
        }
        else
            return icePiece.position + Vector3.up * 0.5f + Vector3.right * 0.125f;

    }

    Vector3 GetLeftSidePosition()
    {
        Transform icePiece = icePieces[0];
        if (icePieces.Count > 2)
        {
            return icePiece.position + Vector3.up * 0.5f + Vector3.right * 0.5f;
        }
        else
            return icePiece.position + Vector3.up * 0.5f + Vector3.left * 0.125f;
    }

}
