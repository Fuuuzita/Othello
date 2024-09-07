using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreDisp : MonoBehaviour
{
    public TextMeshProUGUI white_num;
    public TextMeshProUGUI black_num;
    public TextMeshProUGUI result;
    public Sprite whiteDiscSprite;
    public Sprite blackDiscSprite;
    public Sprite drowDiscSprite;
    public Image resultImg;

    GameObject boardObj;  //"Board"Object
    Boardtile boardtile;

    // Start is called before the first frame update
    void Start()
    {
        //"Board"ObjectŽæ“¾
        boardObj = GameObject.Find("Board");
        boardtile = boardObj.GetComponent<Boardtile>();
    }

    // Update is called once per frame
    void Update()
    {
        white_num.text = ""+boardtile.GettWhiteDiscCount();
        black_num.text = "" + boardtile.GettBlackDiscCount();
        if(boardtile.GetGameOverFlag() )
        {
            result.text = "WIN!!";
            if(boardtile.GettWhiteDiscCount()> boardtile.GettBlackDiscCount())
            {
                resultImg.sprite = whiteDiscSprite;
            }
            else if(boardtile.GettWhiteDiscCount() < boardtile.GettBlackDiscCount())
            {
                resultImg.sprite = blackDiscSprite;
            }
            else
            {
                result.text = "DROW";
                resultImg.sprite = drowDiscSprite;
            }
        }
        else
        {
            // NON:0 WHITE:1 BLACK:2
            if(boardtile.GetNowColor() == 2)
            {
                resultImg.sprite = blackDiscSprite;
            }
            else
            {
                resultImg.sprite = whiteDiscSprite;
            }
        }
    }
}
