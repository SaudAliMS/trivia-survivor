using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameplayController : SingletonMono<GameplayController>
{
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

    #region private methods
//    private void UpdateZorder()
//    {
//        for(int count = 0; count < characterList.Count; count++)
//        {
//            CharacterController characterController = characterList[count];
//            float zorder = characterController.transform.position.y;
////            zorder = zorder * 100;
    //        characterController.transform.SetSiblingIndex(count);
    //    }
    //}

    void ResetCharacters()
    {
        if(characterList == null)
        {
            characterCache = new List<CharacterController>();
            characterList = new List<CharacterController>();
        }

        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            characterController.gameObject.SetActive(false);
            characterCache.Add(characterController);
        }
        characterList.Clear();
    }

    void QuestionCompleted()
    {
        questionIndex++;
        if (questionIndex < levelData.Count)
        {
            LoadQuestion();
        }
        else
        {
            timerOn = false;
            GameController.Instance.LevelCompleted();
            ViewController.Instance.OpenView(Views.LevelComplete);
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
                QuestionFailed();
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
        for(int count= 0; count < 10; count++)
        {
            CharacterController characterController = GetCharacter();
            int characterId = Random.Range(0, UIRefs.Instance.characters.Count);
            characterController.transform.SetParent(characterContainer);
            characterController.SetupCharacter(characterId);
            characterList.Add(characterController);
        }
    }

    void ShuffleCharacters()
    {
        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            characterController.Shuffle();
        }
    }

    CharacterController GetCharacter()
    {
        if(characterCache.Count > 0)
        {
            CharacterController characterController = characterCache[0];
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
    #endregion

    #region public methods
    public void LoadLevel()
    {
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
            canAnswer = false;
            if (levelData[questionIndex].AnswerIsTrue)
            {
                QuestionCompleted();
            }
            else
            {
                QuestionFailed();
            }
        }
    }

    public void OnPressNoBtn()
    {
        if (canAnswer)
        {
            canAnswer = false;
            if (!levelData[questionIndex].AnswerIsTrue)
            {
                QuestionCompleted();
            }
            else
            {
                QuestionFailed();
            }
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
}
