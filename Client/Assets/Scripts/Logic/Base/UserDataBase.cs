//----------------------------------------------------------------------------
//-- UserDataBase
//-- @author xiejie
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;


[Serializable]
public class UserDataBase : Data
{
    public string name;
    public string key;

    public int gold;

    //��ǰ�û�
    public int level;
    //��ǰ�ȼ����飨ÿ�������۳���
    public int lvexp;

    public int skipad;

    public int fish;

    public bool musicOpen;
    public bool soundOpen;

    public int curChallengeId;
    public int noviceId;

    public bool closeshake;

    public int onLineTime;
    public int onLineAwardTime; //�ѻ��۵Ľ���ʱ��
    public UserDataBase():base()
    {
            
    }
}
