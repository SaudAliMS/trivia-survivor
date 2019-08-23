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
    public SpriteRenderer leftIce,rightIce,wholeIce;

    public IceAnimation iceAnimation;
    public Transform stone;
    public TornadoController tornado;
    public SharkController sharkController;
    public WaveController waveController;
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
        leftIce.color = Color.white;
        rightIce.color = Color.white;
        wholeIce.color = Color.white;

        leftIce.transform.localScale = Vector3.one;
        rightIce.transform.localScale = Vector3.one;

        wholeIce.transform.position = Vector3.up * 0.2f; 
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
            ViewController.Instance.OpenView(Views.GameComplete);
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
            Vibration.Vibrate(TapticPlugin.ImpactFeedback.Heavy);

        }
        else
        {
            Vibration.Vibrate(TapticPlugin.ImpactFeedback.Medium);
        }
    }

    void SmileCorrectAnswerCharaceters()
    {
        bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
        for (int count = 0; count < characterList.Count; count++)
        {
            CharacterController characterController = characterList[count];
            bool userAnswerYes = characterController.UserAnswerYes();
            if ((answerIsTrue == userAnswerYes))
            {
                characterController.PlayCorrectAnswerAnimation();
                characterController.PlayIdleHappyAnim(1.2f);
            }
        }

        bool userAnswerYesOwn = myCharacter.UserAnswerYes();
        if ((answerIsTrue == userAnswerYesOwn))
        {
            myCharacter.PlayCorrectAnswerAnimation();
            myCharacter.PlayIdleHappyAnim(1.2f);
            SoundController.Instance.PlaySfx(Sfx.Correct, 0.5f);
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
            //characterCache.Add(myCharacter);
            //myCharacter = null;
        }
    }



    void QuestionFailed()
    {
        canAnswer = false;
        timerOn = false;

        ViewController.Instance.gameCompleteViewController.ShowAnswer(levelData[questionIndex].AnswerStatement);
        ViewController.Instance.OpenView(Views.GameComplete);
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
                //SoundController.Instance.PlaySfx(Sfx.TimeUp, 0.5f);
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
        Vector3 rightCenterPos = GetRightSidePosition();
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
        Vector3 leftCenterPos = GetLeftSidePosition();
        for (int count = 0; count < 4; count++)
        {
            //new Vector3(-1.25f,0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos);

            CharacterController characterController = GetCharacter();
            int characterId = Random.Range(0, UIRefs.Instance.characterAnimationSprites.Count);
            characterController.transform.SetParent(characterContainer);
            characterController.SetupCharacter(characterId, leftSidePos,true);
            characterList.Add(characterController);
        }

        Vector3 leftCenterPos1 = GetLeftSidePosition();//new Vector3(-1.25f, 0.5f, 0);
        Vector3 leftSidePos1 = Utility.GetPositionForCharacterLeft(leftCenterPos1);

        myCharacter = GetCharacter();
        int myCharacterId = GameConstants.MY_CHARACTER_ID;
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
            Debug.Log("Kill all other players");
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
        Vector3 rightCenterPos = GetRightSidePosition();
        for (int count = 0; count < characterMovingRight.Count; count++)
        {
            //new Vector3(1.25f, 0.5f, 0);

            Vector3 rightSidePos = Utility.GetPositionForCharacterRight(rightCenterPos);
            while(Vector2.Distance(rightSidePos, myCharacterPos) < 0.1f)
            {
                Debug.Log("Position Overlap Changing to new pos");
                rightSidePos = Utility.GetPositionForCharacterRight(rightCenterPos);
            }

            int index = characterMovingRight[count];
            //Debug.Log("Characters moving right" + index);
            CharacterController characterController = characterList[index];
            characterController.Shuffle(rightSidePos,false);
        }

        Utility.ResetPositionForCharacterLeft();
        Vector3 leftCenterPos = GetLeftSidePosition();
        // left characters
        for (int count = 0; count < characterMovingLeft.Count; count++)
        {
            //new Vector3(-1.25f, 0.5f, 0);
            Vector3 leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos);
            while (Vector2.Distance(leftSidePos, myCharacterPos) < 0.1f)
            {
                Debug.Log("Position Overlap Changing to new pos");
                leftSidePos = Utility.GetPositionForCharacterLeft(leftCenterPos);
            }

            int index = characterMovingLeft[count];
            //Debug.Log("characterMovingLeft" + index);
            CharacterController characterController = characterList[index];
            characterController.Shuffle(leftSidePos,true);
        }
    }

    public int GetMyPosition() 
    {
        int position = 1;
        int lastQuestionIndex = Mathf.Clamp(questionIndex, 0, levelData.Count - 1);
        bool answerIsTrue = levelData[lastQuestionIndex].AnswerIsTrue;
       
       

        foreach (CharacterController character in characterList) 
        {
            if (answerIsTrue == character.UserAnswerYes()) 
            {
                position++;
            }
        }

        // if i'm wrong at last question
        if (answerIsTrue != myCharacter.UserAnswerYes() && position == 1)
        {
            position =2;
        }
        return position;
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

    public void AnimateReadyGo()
    {
        timerOn = false;
        canAnswer = false;
        ViewController.Instance.OpenView(Views.GamePlay);
        ViewController.Instance.gameplayViewController.UpdateTopBar();
        ViewController.Instance.gameplayViewController.AnimateReadGO();
        Invoke("LoadQuestion", 2.5f);
    }


    public void LoadQuestion()
    {
        SoundController.Instance.PlaySfx(Sfx.NewQuestion, 0.05f);
        questionNumber += 1;
        myCharacter.SetPlayerUIStauts();
        timerValue = GameConstants.QUESTION_TIME;
        timerOn = true;
        ViewController.Instance.OpenView(Views.GamePlay);
        ViewController.Instance.gameplayViewController.UpdateTopBar();
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
            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            Vector3 leftCenterPos = GetLeftSidePosition();//new Vector3(-1.25f, 0.5f, 0);
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
            Vector3 rightCenterPos = GetRightSidePosition();//new Vector3(1.25f, 0.5f, 0);
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
        AnimateReadyGo();
        //        LoadQuestion();
    }

    #endregion

    Vector3 GetRightSidePosition()
    {
        return rightIce.transform.position + Vector3.down * 0f;// + Vector3.left * 0.1f;
    }

    Vector3 GetLeftSidePosition()
    {
        return leftIce.transform.position + Vector3.down * 0.25f;// + Vector3.right * 0.1f;
    }

    #region Death Animations
    void SelectDeathAnimation()
    {
        canAnswer = false;
        timerOn = false;

        ShockWrongAnswerCharaceters();
        SmileCorrectAnswerCharaceters();

        // last index
        if (questionIndex >= levelData.Count - 1)
        {
            Invoke("ThrowStone",0.3f);
        }
        else
        {

            //Invoke("ShowWave", 0.3f);
//            Invoke("ShowShark", 0.5f);
            int randomNo = Random.Range(0, 100);
            if(randomNo < 25)
            {
                Invoke("ThrowLightning", 0.3f);
            }
            else if(randomNo < 50)
            {
                Invoke("ShowTornado", 0.3f);
            }
            else if (randomNo < 75)
            {
                Invoke("ShowWave", 0.3f);
            }
            else
            {
                Invoke("ShowShark", 0.3f);
            }
        }
    }


    void ThrowStone()
    {
        Vector3 stonePosition;
        SpriteRenderer icePieceSinking;

        if (levelData[questionIndex].AnswerIsTrue)
        {
            icePieceSinking = rightIce;
        }
        else
        {
            icePieceSinking = leftIce;
        }

        float posX = icePieceSinking.transform.position.x;
        stonePosition = new Vector3(posX, 7, 0);

        stone.localPosition = stonePosition;
        stone.gameObject.SetActive(true);

        stone.transform.DOLocalMoveY(0.5f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            leftIce.gameObject.SetActive(true);
            rightIce.gameObject.SetActive(true);
            wholeIce.gameObject.SetActive(false);

            PlayGlacierSound();
//            Invoke("PlayGlacierSound", 0.3f);
            snowDust.transform.localPosition = stone.transform.localPosition + Vector3.down * 0.4f;
            snowDust.transform.localScale = Vector3.one * 0.2f;
            snowDust.gameObject.SetActive(true);
            snowDust.DOFade(0.3f, 0.5f).SetEase(Ease.InSine);
            snowDust.transform.DOScale(1, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                snowDust.gameObject.SetActive(false);
                snowDust.color = Color.white;
            });

            icePieceSinking.transform.DOScale(0, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
            {
                icePieceSinking.gameObject.SetActive(false);
            });
            icePieceSinking.DOFade(0, 0.15f).SetDelay(0.1f).SetEase(Ease.Linear);
            stone.gameObject.SetActive(false);

            SinkWrongAnswerCharaceters();
            Invoke("AnalytizeUserAnswer", 1.5f);
        });
    }

    private void PlayGlacierSound() 
    {
        SoundController.Instance.PlaySfx(Sfx.GlacierBreak, 0.5f);
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

        SoundController.Instance.PlaySfx(Sfx.Lightning, 0.5f);
        Invoke("AnalytizeUserAnswer", 1.5f);
    }

    void ShowTornado()
    {
        Vector3 tornadoPos = new Vector3(-1.5f, -7, 0);
        if (levelData[questionIndex].AnswerIsTrue)
        {
            tornadoPos = new Vector3(1, -7, 0);
        }
        tornado.Animate(tornadoPos);

        transform.DOMove(Vector3.zero, 0.6f).OnComplete(() =>
        {
            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            SoundController.Instance.PlaySfx(Sfx.Scream, 0.3f);

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
                        finalPos.y = Random.Range(2.25f, 3f);
                    }
                    else
                    {
                        finalPos.y = Random.Range(-2.5f, -3.5f);
                    }
                    finalPos.x += Random.Range(-1.2f, 1.2f);

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
                    //SoundController.Instance.PlaySfx(Sfx.Scream, 0.5f);
                    myCharacter.PlayDeathAnimation();

                });
            }

            Invoke("AnalytizeUserAnswer", 1.5f);
        });

    }

    void ShowShark()
    {
        Vector3 sharkPos = new Vector3(-2.5f, -7, 0);
        if (levelData[questionIndex].AnswerIsTrue)
        {
            sharkPos = new Vector3(2f, -7, 0);
        }
        sharkController.Animate(sharkPos);

        transform.DOMove(Vector3.zero, 0.85f).OnComplete(() =>
        {
            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            for (int count = 0; count < characterList.Count; count++)
            {
                CharacterController characterController = characterList[count];
                SoundController.Instance.PlaySfx(Sfx.Scream, 0.3f);

                bool userAnswerYes = characterController.UserAnswerYes();
                if ((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
                {
                    Vector3 startPos = characterController.transform.position;
                    Vector3 finalPos = startPos;
  //                  if (Random.Range(0, 100) < 50)
//                    {
                        finalPos.y = Random.Range(2.5f, 4.5f);
//                    }
                    //else
                    //{
                    //    finalPos.y = Random.Range(-2.5f, -3.5f);
                    //}
                    finalPos.x += Random.Range(-2f, 2f);

                    characterController.isDying = true;
                    characterController.ShowBlood();
                    //SoundController.Instance.PlaySfx(Sfx.Scream, 0.5f);
                    characterController.transform.DOScale(1f, 0.3f);
                    characterController.transform.DOScale(0.75f, 0.2f).SetDelay(0.3f);
                    float time = Random.Range(0.3f, 0.6f);
                    characterController.transform.DOMove(finalPos, time).SetEase(Ease.Linear).OnComplete(() => {
                        characterController.PlayFreezeAnimation();

                    });
                }
            }

            bool userAnswerYesOwn = myCharacter.UserAnswerYes();
            if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
            {
                Vector3 startPos = myCharacter.transform.position;
                Vector3 finalPos = startPos;
//                if (Random.Range(0, 100) < 50)
  //              {
                    finalPos.y = Random.Range(2.5f, 4.5f);
                //}
                //else
                //{
                //    finalPos.y = Random.Range(-3f, -4f);
                //}
                finalPos.x += Random.Range(-2f, 2f);

                myCharacter.isDying = true;
                myCharacter.ShowBlood();
                //SoundController.Instance.PlaySfx(Sfx.Scream, 0.5f);
                myCharacter.transform.DOScale(1f, 0.3f);
                myCharacter.transform.DOScale(0.75f, 0.2f).SetDelay(0.3f);
                float time = Random.Range(0.3f, 0.6f);
                myCharacter.transform.DOMove(finalPos, time).SetEase(Ease.Linear).OnComplete(() => {
                    myCharacter.PlayFreezeAnimation();
                });
            }

            Invoke("AnalytizeUserAnswer", 2.5f);
        });

    }

    void ShowWave()
    {
        Vector3 wavePos = new Vector3(-1f, -7, 0);
        Vector3 waveDropletPos = new Vector3(-1.5f, 1, 0);
        if (levelData[questionIndex].AnswerIsTrue)
        {
            wavePos = new Vector3(0.5f, -7, 0);
            waveDropletPos = new Vector3(1.25f, 1, 0);
        }
        waveController.Animate(wavePos);

        transform.DOMove(Vector3.zero, 0.5f).OnComplete(() =>
        {
            iceAnimation.AnimateWater(waveDropletPos);
            SoundController.Instance.PlaySfx(Sfx.Scream, 0.3f);

            bool answerIsTrue = levelData[questionIndex].AnswerIsTrue;
            for (int count = 0; count < characterList.Count; count++)
            {
                CharacterController characterController = characterList[count];
                bool userAnswerYes = characterController.UserAnswerYes();
                if ((answerIsTrue && !userAnswerYes) || (!answerIsTrue && userAnswerYes))
                {
                    Vector3 startPos = characterController.transform.position;
                    Vector3 finalPos = startPos;
                    finalPos.y = Random.Range(2.25f, 3f);
                    finalPos.x += Random.Range(-2f, 2f);

                    characterController.isDying = true;
                    //characterController.ShowWater();
                    //SoundController.Instance.PlaySfx(Sfx.Scream, 0.5f);
                    characterController.transform.DOScale(1f, 0.3f);
                    characterController.transform.DOScale(0.75f, 0.2f).SetDelay(0.3f);
                    float time = Random.Range(0.3f, 0.6f);
                    characterController.transform.DOMove(finalPos, time).SetEase(Ease.Linear).OnComplete(() => {
                        characterController.PlayFreezeAnimation();

                    });
                }
            }

            bool userAnswerYesOwn = myCharacter.UserAnswerYes();
            if ((answerIsTrue && !userAnswerYesOwn) || (!answerIsTrue && userAnswerYesOwn))
            {
                Vector3 startPos = myCharacter.transform.position;
                Vector3 finalPos = startPos;
                finalPos.y = Random.Range(2.25f, 3f);
                finalPos.x += Random.Range(-2f, 2f);

                myCharacter.isDying = true;
                //myCharacter.ShowWater();
                //SoundController.Instance.PlaySfx(Sfx.Scream, 0.5f);
                myCharacter.transform.DOScale(1f, 0.3f);
                myCharacter.transform.DOScale(0.75f, 0.2f).SetDelay(0.3f);
                float time = Random.Range(0.3f, 0.6f);
                myCharacter.transform.DOMove(finalPos, time).SetEase(Ease.Linear).OnComplete(() => {
                    myCharacter.PlayFreezeAnimation();
                });
            }

            Invoke("AnalytizeUserAnswer", 2.5f);
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
