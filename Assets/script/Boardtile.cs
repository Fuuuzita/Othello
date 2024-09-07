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
    bool isPutDisc = false; //�u���ꂽ��true
    bool isEnablePlace = false; //�u����Ȃ�true
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

        //�����z�u
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
        // ������ꏊ�ŉ������Ԃ̏ꍇ
        if((isPutablePlace[pos_x-1, pos_y-1] == true)&&(isEnablePlace==true))
        {
            Debug.Log("DiscSetTOU x:" + pos_x + "  y:" + pos_y + "  color:" + nowColor);
            SetDisconBoard(pos_x, pos_y, nowColor);
            for(int direction=1; direction<=8; direction++) 
            {
                //�Ђ�����Ԃ�
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
            //�u����ꏊ�̃`�F�b�N
            yield return PutDiscEnableChk();
            //���̍X�V
            CountDisc();
            //�u����ꏊ������ꍇ
            if (isEnablePlace == true)
            {
                //�u�����܂ő҂���
                yield return new WaitUntil(() => isPutDisc);

                isPutDisc = false;
                passCount = 0;

            }
            else
            {
                //�u���Ȃ��ꍇ�͂���
                passCount++;
                //2�A���Œu���Ȃ��ꍇ�̓Q�[���I��
                if(passCount>=2)
                {
                    isGameOver = true;
                }
            }

            //�F�ύX
            if (nowColor == 1) { nowColor = 2; }
            else { nowColor = 1; }

            yield return null;
        }

        //���s�̔���
    }

    //�u����ꏊ�̃`�F�b�N
    IEnumerator PutDiscEnableChk()
    {
        bool isEnable = false;
        int flipColor = 0;
        isEnablePlace = false;
        //���]����F
        if (nowColor == 1) { flipColor = 2; }
        else { flipColor = 1; }

        for (int x_loop = 0; x_loop < 8; x_loop++)
        {
            for (int y_loop = 0; y_loop < 8; y_loop++)
            {
                isPutablePlace[x_loop,y_loop] = false;

                //Disc�Ȃ��̏ꍇ�̂ݏ�������B
                if (tileData[x_loop,y_loop] == 0)
                {
                    //�@�@8    1   2
                    //  �@    ��
                    //    7 ���@�� 3
                    //�@ �@�@ ��
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

    //�u���邩�̔��肪�����̂Ŗ������֐���
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

        // �������̃f�[�^�ݒ�
        SetDirectionData(ref x_offset, ref y_offset, ref xmin, ref xmax, ref ymin, ref ymax, direction);

        if ((x_data>= xmin)&&(x_data <= xmax)&&(y_data >= ymin)&&(y_data<= ymax))
        {
            //��オflipColor�̏ꍇ�̂ݏ�������
            if (tileData[x_data + x_offset, y_data + y_offset] == flipColor)
            {
                x_data += x_offset * 2;
                y_data += y_offset * 2;
                while ((x_data >= 0)&& (x_data <= 7)&& (y_data >= 0) && (y_data <= 7))
                {
                    //���̐F�Ȃ�u����
                    if (tileData[x_data, y_data] == nowColor)
                    {
                        isEnable = true;
                        isPutablePlace[x_loop, y_loop] = true;
                        break;
                    }
                    //NON�Ȃ�ʂ���
                    else if (tileData[x_data, y_data] == 0)
                    {
                        break;
                    }
                    //flipColor�Ȃ�p��
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

    //�Ђ�����Ԃ�����
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
        //���]����F
        if (nowColor == 1) { flipColor = 2; }
        else { flipColor = 1; }

        // �������̃f�[�^�ݒ�
        SetDirectionData(ref x_offset, ref y_offset, ref xmin, ref xmax, ref ymin, ref ymax, direction);

        if ((x_data >= xmin) && (x_data <= xmax) && (y_data >= ymin) && (y_data <= ymax))
        {
            //��オflipColor�̏ꍇ�̂ݏ�������
            if (tileData[x_data + x_offset, y_data + y_offset] == flipColor)
            {
                turnDiscX[turnCount] = x_data + x_offset + 1;
                turnDiscY[turnCount] = y_data + y_offset + 1;
                turnCount++;

                x_data += x_offset * 2;
                y_data += y_offset * 2;
                while ((x_data >= 0) && (x_data <= 7) && (y_data >= 0) && (y_data <= 7))
                {
                    //���̐F�Ȃ�u����
                    if (tileData[x_data, y_data] == nowColor)
                    {
                        isEnable = true;
                        break;
                    }
                    //NON�Ȃ�ʂ���
                    else if (tileData[x_data, y_data] == 0)
                    {
                        break;
                    }
                    //flipColor�Ȃ�p��
                    else
                    {
                        //�Ђ�����Ԃ����ۑ�
                        turnDiscX[turnCount] = x_data + 1;
                        turnDiscY[turnCount] = y_data + 1;
                        turnCount++;
                    }

                    x_data += x_offset;
                    y_data += y_offset;
                }

                //�Ђ�����Ԃ�
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

    //�������̃f�[�^�ݒ�
    void SetDirectionData(ref int x_offset, ref int y_offset, ref int xmin, ref int xmax, ref int ymin, ref int ymax ,int direction)
    {
        // direction
        //�@�@8    1   2
        //  �@    ��
        //    7 ���@�� 3
        //�@ �@�@ ��
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

    //Disc���𐔂��鏈��
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
