using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameplayController : SingletonMono<GameplayController>
{
    [HideInInspector]public int sessionCoinsCount = 0;
    [HideInInspector] public int sessionXPCount = 0;
    [HideInInspector] public int questionNumber = 0;

    public SpriteRenderer snowDust;
    //public List<Transform> iceContainer;
    //public List<Transform> icePieces;
    public Transform leftIce,rightIce,wholeIce;

    public Transform stone,tornado;
    public Transform characterContainer;
    public GameObject characterPrefab;
    List<QuestionData> levelData;
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
        leftIce.gameObject.SetActive(false);
        rightIce.gameObject.SetActive(false);
        wholeIce.gameObject.SetActive(true);
        stone.gameObject.SetActive(false);

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



    void QuestionCompleted()
    {
        //bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        questionIndex++;
        if (questionIndex < levelData.Count)
        {
            LoadQuestion();
            //float posX = Camera.main.transform.position.x;
            //posX = answerIsTrue ? posX - 0.7f : posX + 0.7f;
            //Camera.main.transform.DOMoveX(posX, 1f).SetDelay(0.5f);
            //Camera.main.DOFieldOfView(Camera.main.fieldOfView - 5, 0.6f).SetDelay(0.5f);
        }
        else
        {
            timerOn = false;
            ViewController.Instance.OpenView(Views.LevelComplete);
        }
    }

    void AnalytizeUserAnswer()
    {
        canAnswer = false;
        RemoveEliminatedCharaceters();
        bool userAnswerYes = myCharacter.UserAnswerYes();
        if ((userAnswerYes && levelData[questionIndex].AnswerIsTrue) || 
            (!userAnswerYes && !levelData[questionIndex].AnswerIsTrue))
        {
            sessionCoinsCount += GameConstants.COIN_REWARD_ON_TRUE_ANSWER;
            sessionXPCount += GameConstants.XP_REWARD_ON_TRUE_ANSWER;
            ViewController.Instance.gameplayViewController.UpdateTopBar();
            QuestionCompleted();
        }
        else
        {
            QuestionFailed();
        }
    }

    void ShockWrongAnswerCharaceters()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            bool userAnswerYes = characterController.UserAnswerYes();
            if ((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
            {
                characterController.PlayStunAnimation();
            }
        }

        bool userAnswerYesOwn = myCharacter.UserAnswerYes();
        if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
        {
            myCharacter.PlayStunAnimation();
            TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Heavy);

        }
        else
        {
            TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Medium);
        }
    }



    void RemoveEliminatedCharaceters()
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
                SelectDeathAnimation();

            }
            else
            {
                ViewController.Instance.gameplayViewController.timer.text = "" + ((int)timerValue +1);
            }
        }
    }

    void PopulateCharacters()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;

        Utility.ResetPositionForCharacterRight();
        // right characters
        Vector3 rightCenterPos = GetRightSidePosition(answerIsTrue);
        for (int count= 0; count < 5; count++)
        {
            // new Vector3(1.25f, 0.5f, 0);
            Vector3 rightSidePos = Utility.GetPositionForCharacterRight(rightCenterPos);
            CharacterController characterController = GetCharacter();
            int characterId = Random.Range(0, UIRefs.Instance.characterAnimationSprites.Count);
            characterController.transform.SetParent(characterContainer);
            characterController.SetupCharacter(characterId, rightSidePos,false);
            characterList.Add(characterController);
        }

        Utility.ResetPositionForCharacterLeft();
        // left characters
        Vector3 leftCenterPos = GetLeftSidePosition(answerIsTrue);
        for (int count = 0; count < 5; count++)
        {
            //new Vector3(-1.25f,0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos);

            CharacterController characterController = GetCharacter();
            int characterId = Random.Range(0, UIRefs.Instance.characterAnimationSprites.Count);
            characterController.transform.SetParent(characterContainer);
            characterController.SetupCharacter(characterId, leftSidePos,true);
            characterList.Add(characterController);
        }

        Vector3 leftCenterPos1 = GetLeftSidePosition(answerIsTrue);//new Vector3(-1.25f, 0.5f, 0);
        Vector3 leftSidePos1 = Utility.GetPositionForCharacterLeft(leftCenterPos1);

        myCharacter = GetCharacter();
        int myCharacterId = 0;
        myCharacter.transform.SetParent(characterContainer);
        myCharacter.SetupCharacter(myCharacterId, leftSidePos1, true,false);
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
            //Debug.Log("Kill all other players");
            killAllOtherPlayers = true;
        }


        int halfCharacters = tempList.Count / 2;
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        if (killAllOtherPlayers && answerIsTrue)
        {
            //Debug.Log("Kill all other players");
            halfCharacters = tempList.Count;
        }
        else if (killAllOtherPlayers && !answerIsTrue)
        {
            //Debug.Log("Kill all other players");
            halfCharacters = 0;
        }

        // addd half moving right
        for (int count = 0; count < halfCharacters; count++)
        {
            int randomNo = Random.Range(0, tempList.Count);
            int characterIndex  = tempList[randomNo];
            tempList.RemoveAt(randomNo);
            characterMovingRight.Add(characterIndex);
        }

        // addd half moving left
        for (int count = 0; count < tempList.Count; count++)
        {
            int characterIndex = tempList[count];
            //tempList.RemoveAt(count);
            //count--;
            characterMovingLeft.Add(characterIndex);
        }
        //bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;

        Vector2 myCharacterPos = myCharacter.transform.position;
        // right characters
        Utility.ResetPositionForCharacterRight();
        Vector3 rightCenterPos = GetRightSidePosition(answerIsTrue);
        for (int count = 0; count < characterMovingRight.Count; count++)
        {
            //new Vector3(1.25f, 0.5f, 0);

            Vector3 rightSidePos = Utility.GetPositionForCharacterRight(rightCenterPos);
            while(Mathf.Approximately(rightSidePos.x,myCharacterPos.x) && Mathf.Approximately(rightSidePos.y, myCharacterPos.y))
            {
                rightSidePos = Utility.GetPositionForCharacterRight(rightCenterPos);
            }

            int index = characterMovingRight[count];
            //Debug.Log("Characters moving right" + index);
            CharacterController characterController = characterList[index];
            characterController.Shuffle(rightSidePos,false);
        }

        Utility.ResetPositionForCharacterLeft();
        Vector3 leftCenterPos = GetLeftSidePosition(answerIsTrue);
        // left characters
        for (int count = 0; count < characterMovingLeft.Count; count++)
        {
            //new Vector3(-1.25f, 0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos);
            while (Mathf.Approximately(leftSidePos.x, myCharacterPos.x) && Mathf.Approximately(leftSidePos.y, myCharacterPos.y))
            {
                leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos);
            }

            int index = characterMovingLeft[count];
            //Debug.Log("characterMovingLeft" + index);
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

        levelData = GameData.Instance.GetLevelData();
        PopulateCharacters();
    }

    public void LoadQuestion()
    {
        questionNumber += 1;
        myCharacter.SetPlayerUIStauts();
        timerValue = GameConstants.QUESTION_TIME;
        timerOn = true;
        ViewController.Instance.OpenView(Views.GamePlay);
        ViewController.Instance.gameplayViewController.ShowTimer();
        ViewController.Instance.gameplayViewController.ShowQuestion(levelData[questionIndex].Question);
        ViewController.Instance.gameplayViewController.UpdateTopBar();
        gamePaused = false;
        canAnswer = true;


        ShuffleCharacters();
    }

    public void OnPressYesBtn()
    {
        if (canAnswer)
        {
            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            Vector3 leftCenterPos = GetLeftSidePosition(answerIsTrue);//new Vector3(-1.25f, 0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos,false);

            //canAnswer = false;
            myCharacter.ShuffleMyPlayer(leftSidePos,true);
        }
    }

    public void OnPressNoBtn()
    {
        if (canAnswer)
        {
            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            Vector3 rightCenterPos = GetRightSidePosition(answerIsTrue);//new Vector3(1.25f, 0.5f, 0);
            Vector3 rightSidePos = Utility.GetPositionForCharacterRight(rightCenterPos,false);

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

    public void OnPressTapToStartBtn()
    {
        sessionCoinsCount = 0;
        sessionXPCount = 0;
        questionNumber = 0;
        LoadQuestion();
    }

    #endregion

    Vector3 GetRightSidePosition(bool correctLeftSide)
    {
        return rightIce.position + Vector3.down * 0f;// + Vector3.left * 0.1f;
    }

    Vector3 GetLeftSidePosition(bool correctLeftSide)
    {
        return leftIce.position + Vector3.down * 0f;// + Vector3.right * 0.1f;
    }

    #region Death Animations
    void SelectDeathAnimation()
    {
        canAnswer = false;
        timerOn = false;

        ShockWrongAnswerCharaceters();

        // last index
        if (questionIndex >= levelData.Count - 1)
        {
            Invoke("ThrowStone",0.5f);
        }
        else
        {
            int randomNo = Random.Range(0, 100);
            if(randomNo < 0)
            {
                Invoke("ThrowLightning", 0.5f);
            }
            else
            {
                Invoke("ShowTornado", 0.5f);
            }
        }
    }


    void ThrowStone()
    {
        Vector3 stonePosition;
        Transform icePieceSinking;

        if (levelData[questionIndex].AnswerIsTrue)
        {
            icePieceSinking = rightIce;
        }
        else
        {
            icePieceSinking = leftIce;
        }

        SpriteRenderer spriteRenderer = icePieceSinking.GetComponent<SpriteRenderer>();
        float posX = icePieceSinking.transform.position.x;
        stonePosition = new Vector3(posX, 7, 0);

        stone.localPosition = stonePosition;
        stone.gameObject.SetActive(true);

        stone.transform.DOLocalMoveY(0.5f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            leftIce.gameObject.SetActive(true);
            rightIce.gameObject.SetActive(true);
            wholeIce.gameObject.SetActive(false);


            snowDust.transform.localPosition = stone.transform.localPosition + Vector3.down * 0.4f;
            snowDust.transform.localScale = Vector3.one * 0.2f;
            snowDust.gameObject.SetActive(true);
            snowDust.DOFade(0.3f, 0.5f).SetEase(Ease.InSine);
            snowDust.transform.DOScale(1, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                snowDust.gameObject.SetActive(false);
                snowDust.color = Color.white;
            });

            icePieceSinking.DOMoveZ(1, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
            {
                icePieceSinking.gameObject.SetActive(false);
            });
            spriteRenderer.DOFade(0, 0.25f).SetEase(Ease.Linear);

            stone.gameObject.SetActive(false);

            SinkWrongAnswerCharaceters();
            Invoke("AnalytizeUserAnswer", 1.5f);
        });
    }


    void SinkWrongAnswerCharaceters()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            bool userAnswerYes = characterController.UserAnswerYes();
            if ((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
            {
                characterController.PlayDeathAnimation();
            }
        }

        bool userAnswerYesOwn = myCharacter.UserAnswerYes();
        if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
        {
            myCharacter.PlayDeathAnimation();
            //myCharacter.gameObject.SetActive(false);
            //characterCache.Add(myCharacter);
        }
    }


    void ThrowLightning()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            bool userAnswerYes = characterController.UserAnswerYes();
            if ((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
            {
                characterController.PlayLightningAnimation();
            }
        }

        bool userAnswerYesOwn = myCharacter.UserAnswerYes();
        if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
        {
            myCharacter.PlayLightningAnimation();
        }

        Invoke("AnalytizeUserAnswer", 1.5f);
    }

    void ShowTornado()
    {
        //        AnimationController.Instance.PlayAnimation(OnAnimationComplete, tornado, chrId, CharacterAnimtaionType.Stun, true, 0.4f);
        if (levelData[questionIndex].AnswerIsTrue)
        {
            tornado.position = new Vector3(1, -7, 0);
        }
        else
        {
            tornado.position = new Vector3(-1.5f, -7, 0);
        }

        tornado.gameObject.SetActive(true);
        tornado.DOMoveY(7, 1.2f).OnComplete(()=> {
            tornado.gameObject.SetActive(false);
        });

        transform.DOMove(Vector3.zero, 0.4f).OnComplete(() =>
        {
            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            for (int count = 0; count < characterList.Count; count++)
            {
                CharacterController characterController = characterList[count];
                bool userAnswerYes = characterController.UserAnswerYes();
                if ((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
                {
                    Vector3 startPos = characterController.transform.position;
                    Vector3 finalPos = startPos;
                    if (Random.Range(0,100) < 50)
                    {
                        finalPos.y = Random.Range(3f, 4f);
                    }
                    else
                    {
                        finalPos.y = Random.Range(-3f, -4f);
                    }
                    finalPos.x += Random.Range(-1f, 1f);

                    characterController.isDying = true;
                    characterController.transform.DOScale(1f, 0.3f);
                    characterController.transform.DOScale(0.75f, 0.2f).SetDelay(0.3f);
                    float time  = Random.Range(0.3f, 0.6f);
                    characterController.transform.DOMove(finalPos, time).SetEase(Ease.Linear).OnComplete(()=> {
                        characterController.PlayFreezeAnimation();

                    });

                }
            }

            bool userAnswerYesOwn = myCharacter.UserAnswerYes();
            if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
            {
                Vector3 startPos = myCharacter.transform.position;
                Vector3 finalPos = startPos;
                if (Random.Range(0, 100) < 50)
                {
                    finalPos.y = Random.Range(3f, 4f);
                }
                else
                {
                    finalPos.y = Random.Range(-3f, -4f);
                }
                finalPos.x += Random.Range(-1f, 1f);

                myCharacter.isDying = true;
                myCharacter.transform.DOScale(1f, 0.3f);
                myCharacter.transform.DOScale(0.75f, 0.2f).SetDelay(0.3f);
                myCharacter.transform.DOMove(finalPos, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                    myCharacter.PlayDeathAnimation();

                });
            }

            Invoke("AnalytizeUserAnswer", 1.5f);
        });

    }

    #endregion Death Animations

    public int GetRequiredXPForLevelUpdate()
    {
        if (PlayerData.Level > GameConstants.REQUIRED_XP.Length) 
        {
            return GameConstants.REQUIRED_XP[GameConstants.REQUIRED_XP.Length - 1];
        }
        else 
        {
            return GameConstants.REQUIRED_XP[PlayerData.Level - 1];
        }
    }
}
