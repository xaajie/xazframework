using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class UserRecordData : Data
{
    //�û�����
    public UserDataBase user;
    //�ؿ���¼
    public List<UserChallengeDataBase> challengeInfo;
    //�����б�����
    public List<UserBagDataBase> bagInfo;
    //ͼ��
    public List<UserBookDataBase> books;

    public List<UserAttrupDataBase> attrup;

    public List<UserBuffDataBase> buffs;

    public List<UserAchivementChallengeDataBase> achivements;
    //�����¼
    public List<UserShopBuyDataBase> shopbuyInfos;

    public UserRecordData() : base()
    {
        user = new UserDataBase();
        challengeInfo = new List<UserChallengeDataBase>() { };
        bagInfo = new List<UserBagDataBase>() { };
        books = new List<UserBookDataBase>() { };
        attrup = new List<UserAttrupDataBase> { };
        buffs = new List<UserBuffDataBase> { };
        achivements = new List<UserAchivementChallengeDataBase> { };
        shopbuyInfos = new List<UserShopBuyDataBase>() { };
    }

    public void SetUserData(string v1, string v2)
    {
        user.name = v1;
        user.key = v2;
        user.gold = Constant.INITGOID;
        user.level = 1;
        user.lvexp = 0;
        user.noviceId = NoviceConst.Novice_StartId;
    }
}
