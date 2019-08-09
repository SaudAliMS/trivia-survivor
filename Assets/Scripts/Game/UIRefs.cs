using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRefs : SingletonMono<UIRefs>
{
    public List<Sprite> characters;


    public List<CharacterAnimList> characterAnimationSprites;


}

[System.Serializable]
public class StateAnim
{
    public List<Sprite> animSprites;
}
 
[System.Serializable]
public class CharacterAnimList
{
    public List<StateAnim> animationStates;
}