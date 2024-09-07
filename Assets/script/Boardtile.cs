using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boardtile : MonoBehaviour
{
    // NON:0 WHITE:1 BLACK:2
    int[,] tileData = new int[8, 8];
    bool[,] isPutablePlace = new bool[8, 8];
    bool isGameOver = false;
    int nowColor = 2; //BLACK:2
    bool isPutDisc = false; //置かれたらtrue
    bool isEnablePlace = false; //置けるならtrue
    int disc_blackNum = 0;
    int disc_whiteNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        // all elements "NON"
        for (int x_loop = 0; x_loop < 8; x_loop++)
        {
            for (int y_loop = 0; y_loop < 8; y_loop++)
            {
                tileData[x_loop, y_loop] = 0;
            }
        }

        //初期配置
        SetDisconBoard(4, 4, 1);
        SetDisconBoard(4, 5, 2);
        SetDisconBoard(5, 4, 2);
        SetDisconBoard(5, 5, 1);

        // othello coroutine start
        StartCoroutine(OthelloGame());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //tileData ni data wo settei
    public void SetDisconBoard(int posx, int posy, int discColor)
    {
        string objName = "board_" + posx + "_" + posy;
        GameObject targetdisc = GameObject.Find(objName);

        // set disc color
        tileData[posx - 1, posy - 1] = discColor;

        // set disc
        targetdisc.GetComponent<DiscOnBoard>().discColorSet(discColor);

    }

    // touch sequence
    public void TouchSeqBoarf(int pos_x, int pos_y)
    {
        // 押せる場所で押せる状態の場合
        if((isPutablePlace[pos_x-1, pos_y-1] == true)&&(isEnablePlace==true))
        {
            Debug.Log("DiscSetTOU x:" + pos_x + "  y:" + pos_y + "  color:" + nowColor);
            SetDisconBoard(pos_x, pos_y, nowColor);
            for(int direction=1; direction<=8; direction++) 
            {
                //ひっくり返す
                TurnDisc(pos_x, pos_y, direction);
            }
            isPutDisc = true;
        }
    }

    //coroutine
    IEnumerator OthelloGame()
    {
        //        bool isPutDisc = true;
        int passCount = 0;


        while (isGameOver == false)
        {
            //置ける場所のチェック
            yield return PutDiscEnableChk();
            //数の更新
            CountDisc();
            //置ける場所がある場合
            if (isEnablePlace == true)
            {
                //置かれるまで待つ処理
                yield return new WaitUntil(() => isPutDisc);

                isPutDisc = false;
                passCount = 0;

            }
            else
            {
                //置けない場合はここ
                passCount++;
                //2連続で置けない場合はゲーム終了
                if(passCount>=2)
                {
                    isGameOver = true;
                }
            }

            //色変更
            if (nowColor == 1) { nowColor = 2; }
            else { nowColor = 1; }

            yield return null;
        }

        //勝敗の判定
    }

    //置ける場所のチェック
    IEnumerator PutDiscEnableChk()
    {
        bool isEnable = false;
        int flipColor = 0;
        isEnablePlace = false;
        //反転する色
        if (nowColor == 1) { flipColor = 2; }
        else { flipColor = 1; }

        for (int x_loop = 0; x_loop < 8; x_loop++)
        {
            for (int y_loop = 0; y_loop < 8; y_loop++)
            {
                isPutablePlace[x_loop,y_loop] = false;

                //Discなしの場合のみ処理する。
                if (tileData[x_loop,y_loop] == 0)
                {
                    //　　8    1   2
                    //  　    ↑
                    //    7 ←　→ 3
                    //　 　　 ↓
                    //    6    5   4
                    for (int direction = 1; direction <= 8; direction++) 
                    {
                        if(PutableChk(x_loop, y_loop, flipColor, direction))
                        {
                            isEnable = true;
                            break;
                        }
                    }
                }
            }
        }

        isEnablePlace = isEnable;
        yield return null;
    }

    //置けるかの判定が長いので無理やり関数化
    bool PutableChk(int x_loop, int y_loop, int flipColor, int direction)
    {
        int x_data = x_loop;
        int y_data = y_loop;
        int xmax = 8;
        int xmin = 0;
        int ymax = 8;
        int ymin = 0;
        int x_offset = 0;
        int y_offset = 0;
        bool isEnable = false;
        x_data = x_loop;
        y_data = y_loop;

        // 方向毎のデータ設定
        SetDirectionData(ref x_offset, ref y_offset, ref xmin, ref xmax, ref ymin, ref ymax, direction);

        if ((x_data>= xmin)&&(x_data <= xmax)&&(y_data >= ymin)&&(y_data<= ymax))
        {
            //一つ上がflipColorの場合のみ処理する
            if (tileData[x_data + x_offset, y_data + y_offset] == flipColor)
            {
                x_data += x_offset * 2;
                y_data += y_offset * 2;
                while ((x_data >= 0)&& (x_data <= 7)&& (y_data >= 0) && (y_data <= 7))
                {
                    //今の色なら置ける
                    if (tileData[x_data, y_data] == nowColor)
                    {
                        isEnable = true;
                        isPutablePlace[x_loop, y_loop] = true;
                        break;
                    }
                    //NONならぬける
                    else if (tileData[x_data, y_data] == 0)
                    {
                        break;
                    }
                    //flipColorなら継続
                    else
                    {
                    }

                    x_data += x_offset;
                    y_data += y_offset;
                }
            }
        }
        return isEnable;
    }

    //ひっくり返す処理
    bool TurnDisc(int x_loop, int y_loop, int direction)
    {
        int x_data = x_loop-1;
        int y_data = y_loop-1;
        int xmax = 8;
        int xmin = 0;
        int ymax = 8;
        int ymin = 0;
        int x_offset = 0;
        int y_offset = 0;
        bool isEnable = false;
        int flipColor = 0;
        int[] turnDiscX = { 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] turnDiscY = { 0, 0, 0, 0, 0, 0, 0, 0 };
        int turnCount = 0;
        //反転する色
        if (nowColor == 1) { flipColor = 2; }
        else { flipColor = 1; }

        // 方向毎のデータ設定
        SetDirectionData(ref x_offset, ref y_offset, ref xmin, ref xmax, ref ymin, ref ymax, direction);

        if ((x_data >= xmin) && (x_data <= xmax) && (y_data >= ymin) && (y_data <= ymax))
        {
            //一つ上がflipColorの場合のみ処理する
            if (tileData[x_data + x_offset, y_data + y_offset] == flipColor)
            {
                turnDiscX[turnCount] = x_data + x_offset + 1;
                turnDiscY[turnCount] = y_data + y_offset + 1;
                turnCount++;

                x_data += x_offset * 2;
                y_data += y_offset * 2;
                while ((x_data >= 0) && (x_data <= 7) && (y_data >= 0) && (y_data <= 7))
                {
                    //今の色なら置ける
                    if (tileData[x_data, y_data] == nowColor)
                    {
                        isEnable = true;
                        break;
                    }
                    //NONならぬける
                    else if (tileData[x_data, y_data] == 0)
                    {
                        break;
                    }
                    //flipColorなら継続
                    else
                    {
                        //ひっくり返すやつを保存
                        turnDiscX[turnCount] = x_data + 1;
                        turnDiscY[turnCount] = y_data + 1;
                        turnCount++;
                    }

                    x_data += x_offset;
                    y_data += y_offset;
                }

                //ひっくり返す
                if(isEnable)
                {
                    for(int loopCnt=0; loopCnt< turnCount; loopCnt++)
                    {
                        SetDisconBoard(turnDiscX[loopCnt], turnDiscY[loopCnt], nowColor);
                    }
                }


            }
        }
        return isEnable;
    }

    //方向毎のデータ設定
    void SetDirectionData(ref int x_offset, ref int y_offset, ref int xmin, ref int xmax, ref int ymin, ref int ymax ,int direction)
    {
        // direction
        //　　8    1   2
        //  　    ↑
        //    7 ←　→ 3
        //　 　　 ↓
        //    6    5   4
        switch (direction)
        {
            case 1:
                x_offset = 0;
                y_offset = -1;
                xmin = 0;
                xmax = 7;
                ymin = 2;
                ymax = 7;
                break;
            case 2:
                x_offset = 1;
                y_offset = -1;
                xmin = 0;
                xmax = 5;
                ymin = 2;
                ymax = 7;
                break;
            case 3:
                x_offset = 1;
                y_offset = 0;
                xmin = 0;
                xmax = 5;
                ymin = 0;
                ymax = 7;
                break;
            case 4:
                x_offset = 1;
                y_offset = 1;
                xmin = 0;
                xmax = 5;
                ymin = 0;
                ymax = 5;
                break;
            case 5:
                x_offset = 0;
                y_offset = 1;
                xmin = 0;
                xmax = 7;
                ymin = 0;
                ymax = 5;
                break;
            case 6:
                x_offset = -1;
                y_offset = 1;
                xmin = 2;
                xmax = 7;
                ymin = 0;
                ymax = 5;
                break;
            case 7:
                x_offset = -1;
                y_offset = 0;
                xmin = 2;
                xmax = 7;
                ymin = 0;
                ymax = 7;
                break;
            case 8:
                x_offset = -1;
                y_offset = -1;
                xmin = 2;
                xmax = 7;
                ymin = 2;
                ymax = 7;
                break;
            default:
                Debug.Log("ERROR  direction");
                break;
        }
    }

    //Disc数を数える処理
    void CountDisc()
    {
        int blackCnt = 0;
        int whiteCnt = 0;

        // 8*8 tile mapping
        for (int x_loop = 0; x_loop < 8; x_loop++)
        {
            for (int y_loop = 0; y_loop < 8; y_loop++)
            {
                // NON:0 WHITE:1 BLACK:2
                if (tileData[x_loop, y_loop]==1)
                {
                    whiteCnt++;
                }
                else if (tileData[x_loop, y_loop] == 2)
                {
                    blackCnt++;
                }
                else
                {

                }
            }
        }

        disc_whiteNum = whiteCnt;
        disc_blackNum = blackCnt;
    }

    public int GettBlackDiscCount()
    {
        return disc_blackNum;
    }
    public int GettWhiteDiscCount()
    {
        return disc_whiteNum;
    }

    public bool GetGameOverFlag()
    {
        return isGameOver;
    }

    public int GetNowColor()
    {
        return nowColor;
    }
}
