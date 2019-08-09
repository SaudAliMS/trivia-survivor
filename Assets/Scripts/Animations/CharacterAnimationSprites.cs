using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterAnimationSprites", order = 1)]
public class CharacterAnimationSprites : ScriptableObject
{
    [Header("Character 0")]
    public List<Sprite> character0Death;
    public List<Sprite> character0GameOver;
    public List<Sprite> character0Idle;
    public List<Sprite> character0Stun;
    public List<Sprite> character0Walk;

    [Header("Character 1")]
    public List<Sprite> character1Death;
    public List<Sprite> character1GameOver;
    public List<Sprite> character1Idle;
    public List<Sprite> character1Stun;
    public List<Sprite> character1Walk;

    [Header("Character 2")]
    public List<Sprite> character2Death;
    public List<Sprite> character2GameOver;
    public List<Sprite> character2Idle;
    public List<Sprite> character2Stun;
    public List<Sprite> character2Walk;

    [Header("Character 3")]
    public List<Sprite> character3Death;
    public List<Sprite> character3GameOver;
    public List<Sprite> character3Idle;
    public List<Sprite> character3Stun;
    public List<Sprite> character3Walk;

    [Header("Character 4")]
    public List<Sprite> character4Death;
    public List<Sprite> character4GameOver;
    public List<Sprite> character4Idle;
    public List<Sprite> character4Stun;
    public List<Sprite> character4Walk;

    [Header("Character 5")]
    public List<Sprite> character5Death;
    public List<Sprite> character5GameOver;
    public List<Sprite> character5Idle;
    public List<Sprite> character5Stun;
    public List<Sprite> character5Walk;
}
