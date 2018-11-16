using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameStart : MonoBehaviour {

    private GameManager gameManager;

    // Use this for initialization
    void Start() {
        gameManager = GameManager.Instance;
        gameManager.StartRound();
    }
}
