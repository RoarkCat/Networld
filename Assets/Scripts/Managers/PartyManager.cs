using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour {

    public List<PartyLayout> listOfAllies;
    public Transform allyTransform;
}

[System.Serializable]
public class PartyLayout
{
    public GameObject friendlyPrefab;
    public Vector3 friendlyPosition;
    public BaseCharacterClass characterScript;
}