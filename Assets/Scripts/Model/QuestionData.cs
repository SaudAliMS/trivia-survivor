using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class QuestionData 
{
    private int id;
    private string questionStatement;
    private string answerStatement;
    private bool answerIsTrue;

    public int Id                           { get { return id; } }
    public string Question                  { get { return questionStatement; } }
    public string AnswerStatement           { get { return answerStatement; } }
    public bool AnswerIsTrue                { get { return answerIsTrue; } }

    public static QuestionData Create(string[] elements)
    {
        QuestionData levelData              = new QuestionData();
        levelData.id                     = Utility.ToInt(elements[0]);
        levelData.answerIsTrue           = Utility.ToInt(elements[1]) == 1 ? true : false;
        levelData.questionStatement      = (elements[2]);
        levelData.answerStatement        = (elements[3]);
        return levelData;
    }
}