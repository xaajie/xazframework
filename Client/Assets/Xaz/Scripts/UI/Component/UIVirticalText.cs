//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//  竖排多排文字
//  @author xiangzheng
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace Xaz
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System;

    [ExecuteInEditMode]
    public class UIVirticalText : BaseMeshEffect
    {

        [Tooltip("字和字之间的距离")]
        public float spacing = 1;
        private float lineSpacing = 1;
        private float textSpacing = 1;
        private float xOffset = 0;
        private float yOffset = 0;
        
        private readonly string strRegex = @"(\n)";
        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive())
                return;

            List<UIVertex> verts = new List<UIVertex>();
            helper.GetUIVertexStream(verts);

            Text text = GetComponent<Text>();
            string realText = GetStringNoHtml(text.text);

            TextGenerator tg = text.cachedTextGenerator;
            lineSpacing = text.fontSize * text.lineSpacing;
            textSpacing = text.fontSize * spacing;

            xOffset = text.rectTransform.sizeDelta.x / 2 - text.fontSize / 2;
            yOffset = text.rectTransform.sizeDelta.y / 2 - text.fontSize / 2;

            List<UILineInfo> lines = new List<UILineInfo>();
            tg.GetLines(lines);
            int needDelNum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                UILineInfo line = lines[i];

                int step = i;
                int current = 0;
                int endCharIdx = (i + 1 == lines.Count) ? tg.characterCountVisible : lines[i + 1].startCharIdx;
                for (int j = line.startCharIdx; j < endCharIdx; j++)
                {
                    bool isMatch = Regex.IsMatch(text.text[j].ToString(), strRegex);
                    bool isShow = (realText.Length > j - needDelNum) &&
                            text.text[j].ToString() == realText[j - needDelNum].ToString();
                    if (!isMatch && isShow)
                    {
                        modifyText(helper, j - needDelNum, current++, step);
                    }
                    else
                    {
                        needDelNum++;
                        if (isMatch)
                        {
                            break;
                        }
                    }

                }
            }
        }

        void modifyText(VertexHelper helper, int i, int charYPos, int charXPos)
        {
            UIVertex lb = new UIVertex();
            helper.PopulateUIVertex(ref lb, i * 4);

            UIVertex lt = new UIVertex();
            helper.PopulateUIVertex(ref lt, i * 4 + 1);

            UIVertex rt = new UIVertex();
            helper.PopulateUIVertex(ref rt, i * 4 + 2);

            UIVertex rb = new UIVertex();
            helper.PopulateUIVertex(ref rb, i * 4 + 3);

            Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
            Matrix4x4 move = Matrix4x4.TRS(-center, Quaternion.identity, Vector3.one);

            float x = -charXPos * lineSpacing + xOffset;
            float y = -charYPos * textSpacing + yOffset;

            Vector3 pos = new Vector3(x, y, 0);
            Matrix4x4 place = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            Matrix4x4 transform = place * move;

            lb.position = transform.MultiplyPoint(lb.position);
            lt.position = transform.MultiplyPoint(lt.position);
            rt.position = transform.MultiplyPoint(rt.position);
            rb.position = transform.MultiplyPoint(rb.position);

            helper.SetUIVertex(lb, i * 4);
            helper.SetUIVertex(lt, i * 4 + 1);
            helper.SetUIVertex(rt, i * 4 + 2);
            helper.SetUIVertex(rb, i * 4 + 3);
        }

        private string GetStringNoHtml(string strHtml)
        {
            if (String.IsNullOrEmpty(strHtml))
            {
                return strHtml;
            }
            else
            {
                string[] aryReg ={
@"<script[^>]*?>.*?</script>",
@"<!--.*\n(-->)?",
@"<(\/\s*)?(.|\n)*?(\/\s*)?>",
@"<(\w|\s|""|'| |=|\\|\.|\/|#)*",
@"([\r\n|\s])*",
@"&(quot|#34);",
@"&(amp|#38);",
@"&(lt|#60);",
@"&(gt|#62);",
@"&(nbsp|#160);",
@"&(iexcl|#161);",
@"&(cent|#162);",
@"&(pound|#163);",
@"&(copy|#169);",
@"&#(\d+);"};

                string newReg = aryReg[0];
                string strOutput = strHtml.Replace("&nbsp;", " ");
                for (int i = 0; i < aryReg.Length; i++)
                {
                    Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                    strOutput = regex.Replace(strOutput, "");
                }
                strOutput.Replace("<", "&gt;");
                strOutput.Replace(">", "&lt;");
                return strOutput.Replace(" ", "&nbsp;");
            }
        }
    }
}