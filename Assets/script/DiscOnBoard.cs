using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscOnBoard : MonoBehaviour
{
    public GameObject boardDisc;      // 1disc
    // NON:0 WHITE:1 BLACK:2
    public Sprite disc_White;
    public Sprite disc_Black;
    SpriteRenderer MainSpriteRenderer;
    GameObject discGameObject;
    int myposX = 0;
    int myposY = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void discCreate(int x_pos,int y_pos)
    {
        Vector3 newPos2;

        newPos2 = this.transform.position;
        newPos2.z = -5;
        discGameObject = Instantiate(boardDisc) as GameObject;
        discGameObject.transform.parent = this.transform;
        discGameObject.transform.position = newPos2;
        discGameObject.name = "disc_" + x_pos + "_" + y_pos;
        discGameObject.SetActive(false); //hihyouji
        MainSpriteRenderer = discGameObject.GetComponent<SpriteRenderer>();
        myposX = x_pos;
        myposY = y_pos;
    }

    // NON:0 WHITE:1 BLACK:2
    public void discColorSet(int color)
    {
        discGameObject.SetActive(true); //hihyouji
        if (color==1){
            MainSpriteRenderer.sprite = disc_White;
        }else{
            MainSpriteRenderer.sprite = disc_Black;

        }
    }

    public void TouchDisc()
    {
        // touch parent sequence
        gameObject.GetComponentInParent<Boardtile>().TouchSeqBoarf(myposX, myposY);
    }
}
