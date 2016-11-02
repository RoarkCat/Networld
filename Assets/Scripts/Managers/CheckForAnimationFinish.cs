using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForAnimationFinish : MonoBehaviour {

    public bool moveIsFinished;

    public void animationFinished()
    {
        moveIsFinished = true;
    }
}
