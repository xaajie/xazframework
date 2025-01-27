//----------------------------------------------------------------------------
//-- sdk静态常量定义
//-- @author xz
//----------------------------------------------------------------------------

public class SDKConst
{
    //TT 分享
    //https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/operation1/gain-user/retweet
    //1 通用的不传 不定义了
    //2 链接分享​ 以默认链接分享形式为例 没有channel字段
    //代码指定 > 模板指定 > 平台默认
    public readonly string TT_Share_Channel_Video = "video"; //视频分享 
    public readonly string TT_Share_Channel_Token = "token"; //口令分享 不定义了
    public readonly string TT_Share_Channel_Article = "article"; //口令分享 ps:抖音不支持图文分享。
}