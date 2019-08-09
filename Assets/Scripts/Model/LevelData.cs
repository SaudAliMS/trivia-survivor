using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LevelData 
{
    private int id;
    private int level;
    private string questionStatement;
    private string answerStatement;
    private bool answerIsTrue;

    public int Id                           { get { return id; } }
    public int Level                        { get { return level; } }
    public string Question                  { get { return questionStatement; } }
    public string AnswerStatement           { get { return answerStatement; } }
    public bool AnswerIsTrue                { get { return answerIsTrue; } }

    public static LevelData Create(string[] elements)
    {
        LevelData levelData              = new LevelData();
        levelData.id                     = Utility.ToInt(elements[0]);
        levelData.level                  = Utility.ToInt(elements[1]);
        levelData.answerIsTrue           = Utility.ToInt(elements[2]) == 1 ? true : false;
        levelData.questionStatement      = (elements[3]);
        levelData.answerStatement        = (elements[4]);

        return levelData;
    }
}