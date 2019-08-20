using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System;

public class GameData : SingletonBase<GameData>
{
    #region variables
    int questionIndex = 0;
    const int QUESTION_PER_MISSION = 5;
    private List<QuestionData> questionList;
    #endregion

    #region initialization methods
    public GameData()
    {
        questionList = new List<QuestionData>();
        questionIndex = DatabaseManager.GetInt("question_index", 0);
    }
    #endregion

    #region private methods
    #endregion

    #region public setter methods
    public void AddQuestionData(QuestionData levelData)
    {
        questionList.Add(levelData);
    }
    #endregion

    #region public getter methods

    public List<QuestionData> GetLevelData()
    {
        List<QuestionData> qList = new List<QuestionData>();
        int count = 0;
        while(count < QUESTION_PER_MISSION)
        {
            QuestionData qData = questionList[questionIndex];
            qList.Add(qData);
            count++;
            questionIndex++;
            if(questionIndex >= questionList.Count)
            {
                questionIndex = 0;
            }
        }
        DatabaseManager.SetInt("question_index", questionIndex);
        return qList;
    }

    //public bool LevelDataExists(int level)
    //{

    //    return questionsList.ContainsKey(level);
    //}
    #endregion
}