using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterScript : MonoBehaviour {

    public List<EnemyLayout> listOfEnemies;
    public Transform battleEncounterTransform;
    public Transform playerPosition;

}

[System.Serializable]
public class EnemyLayout
{
    public GameObject enemyPrefab;
    public Vector3 enemyPosition;
    public Vector3 enemyColliderPosition;
    public Vector3 enemyColliderScale;
}
