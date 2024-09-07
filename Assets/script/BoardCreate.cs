using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreate : MonoBehaviour
{
    public GameObject boardTile;      // 1tile

    void Awake()
    {
        Vector3 newPos;
        float board_x = -1.6f;
        float board_y = 1.6f;
        int x_loop;
        int y_loop;
        GameObject panelGameObject;

        // 8*8 tile mapping
        for (x_loop = 0; x_loop < 8; x_loop++)
        {
            for (y_loop = 0; y_loop < 8; y_loop++)
            {
                newPos = this.transform.position;
                newPos.x = board_x + 0.4f * x_loop;
                newPos.y = board_y + (-0.4f) * y_loop;
                newPos.z = -5;
                panelGameObject = Instantiate(boardTile) as GameObject;
                panelGameObject.transform.parent = this.transform;
                panelGameObject.transform.position = newPos;
                panelGameObject.name = "board_"+(x_loop + 1)+"_" + (y_loop + 1);
                panelGameObject.GetComponent<DiscOnBoard>().discCreate(x_loop+1, y_loop + 1);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

}
