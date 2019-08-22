using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRefs : SingletonMono<UIRefs>
{
    //public List<Sprite> characters;


    public List<CharacterAnimList> characterAnimationSprites;



    public List<Sprite> tornado;
    public List<Sprite> waves;
    public List<Sprite> shark;
    public Sprite sharkUnderWater;

}

[System.Serializable]
public class StateAnim
{
    public List<Sprite> animSprites;
}
 
[System.Serializable]
public class CharacterAnimList
{
    //public List<StateAnim> animationStates;

    public List<Sprite> frozenFrames;
    public List<Sprite> cryingFrames;
    public List<Sprite> shiveringFrames;
    public List<Sprite> shockFrames;
    public List<Sprite> lightningFrames;
    public List<Sprite> gameWinFrames;
}

