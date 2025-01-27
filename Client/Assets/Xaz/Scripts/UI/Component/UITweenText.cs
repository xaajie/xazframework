//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
namespace Xaz
{
    public class UITweenText : Text, IControl
    {
        private float changeSpeed = 0.5f;
        double initNum = 0.0;
        double showNum = 0.0;
        double targetNum = 0;
        private bool IsChange = false;
        private bool isAdd = true;
        private int numpercent = 0;
        void Update()
        {
            if (IsChange)
            {
                showNum += (targetNum - initNum) * Time.deltaTime / changeSpeed;
                if (isAdd)
                {
                    if (showNum >= targetNum)
                    {
                        showNum = targetNum;
                        IsChange = false;
                    }
                }
                else
                {
                    if (showNum <= targetNum)
                    {
                        showNum = targetNum;
                        IsChange = false;
                    }
                }
                // 更新 UI 显示
                text = Math.Round(showNum, numpercent).ToString();
            }
        }

        //numpercent小数单位
        public void SetNum(double targetNumt, int numpercentt = 0)
        {
            numpercent = numpercentt;
            IsChange = true;
            initNum = showNum > 0 ? showNum : initNum;
            targetNum = targetNumt;
            isAdd = targetNum > initNum;
        }
    }
}
