using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview : MonoBehaviour
{

    //This script exists on the players in the character creator screen
    //It just handles visuals for the players

    [SerializeField] int playerID;
    private Transform modelTransform;
    private Renderer bodyRenderer;
    public Color litColor;

    // Start is called before the first frame update
    void Start()
    {
        modelTransform = transform;
        bodyRenderer = modelTransform.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

        //Get Color
        switch (playerID)
        {
            case 0:
                litColor = PlayerStats.player1Color;
                break;
            case 1:
                litColor = PlayerStats.player2Color;
                break;
            case 2:
                litColor = PlayerStats.player3Color;
                break;
            case 3:
                litColor = PlayerStats.player4Color;
                break;
        }

        //Set Color
        bodyRenderer.material.SetColor("_BaseColor", litColor); //Light Color
    }
}
