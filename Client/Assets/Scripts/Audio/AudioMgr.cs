//------------------------------------------------------------
// 游戏数据和框架音频管理器的桥接器
//------------------------------------------------------------
using Table;
using Xaz;

public class AudioMgr : Singleton<AudioMgr>
{
    static string saveMusicKey = "musicValue";
    static string saveSoundKey = "soundValue";
    public float soundVal=1;
    public float musicVal=1;

    public void Init()
    {
        if (PlayerPrefs.HasKey(saveMusicKey))
        {
            musicVal = PlayerPrefs.GetFloat(saveMusicKey);
        }
        if (PlayerPrefs.HasKey(saveSoundKey))
        {
            soundVal  = PlayerPrefs.GetFloat(saveSoundKey);
        }
        AudioManager.Instance.init();
        SetMusicVolume(musicVal);
        SetSoundVolume(soundVal);
    }

    public audio CheckGetAudio(int id)
    {
        if (StaticDataMgr.Instance.audioInfo.ContainsKey(id))
        {
            return StaticDataMgr.Instance.audioInfo[id];
        }
        Logger.Print("audioid未查到配置", id);
        return null;
    }

    public void Play(int id)
    {
        if (id > 0)
        {
            audio info = CheckGetAudio(id);
            if (info != null)
            {
                string path = string.Format("{0}{1}{2}", XazConfig.AudioPath, info.filename, XazConfig.AudioSuffix);
                AudioManager.Instance.Play(path, info.aType, info.aType == (int)AudioManager.soundtrack.music);
            }
        }
    }

    public void Save()
    {
        PlayerPrefs.SetFloat(saveMusicKey, musicVal);
        PlayerPrefs.SetFloat(saveSoundKey, soundVal);
    }
    public void SetMusicVolume(float volume)
    {
        musicVal = volume;
        AudioManager.Instance.SetVolume(volume, AudioManager.soundtrack.music);
    }

    public void SetSoundVolume(float volume)
    {
        soundVal = volume;
        AudioManager.Instance.SetVolume(volume, AudioManager.soundtrack.effect);
    }

    public void Shake()
    {
        if (Profile.Instance.user.CanShake())
        {
            SDKMgr.Instance.DoVibrate();
        }
    }
}
