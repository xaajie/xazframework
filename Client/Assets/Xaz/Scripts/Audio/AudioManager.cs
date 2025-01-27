//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
// 音效管理器
// @author xiangzheng
//-------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
namespace Xaz
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public enum soundtrack
        {
            music = 1,
            effect = 2,
            //voice = 2,
            all = 3,
        }
        // (1) 声音根节点的物体;
        // (2) 保证这个节点在场景切换的时候不会删除，这样就不用再初始化一次;
        // (3) 所有播放声音的生源节点，都是在这个节点下
        bool _IsMusicOpen = false;//存放当前全局背景音乐是否静音的变量
        bool _IsSoundOpen = false;//存放当前音效是否静音的变量

        // url --> AudioSource 映射, 区分音乐，音效
        Dictionary<string, AudioSource> musics = null;//音乐表
        Dictionary<string, List<AudioSource>> effecs = null;//音效表
        Dictionary<soundtrack, float> volumeDic = new Dictionary<soundtrack, float>();
        private int checkInv = 900;
        private int cot = 1;
        public bool IsMusicOpen
        {
            get { return _IsMusicOpen; }
            set
            {
                _IsMusicOpen = value;
            }
        }
        public bool IsSoundOpen
        {
            get { return _IsSoundOpen; }
            set
            {
                _IsSoundOpen = value;
            }
        }

        //初始化
        public void init()
        {
            //this.gameObject.AddComponent<AudioScan>();//把声音检测组件挂载到根节点下
            //初始化音乐表和音效表
            musics = new Dictionary<string, AudioSource>();
            effecs = new Dictionary<string, List<AudioSource>>();
            volumeDic.Add(soundtrack.music, 1f);
            volumeDic.Add(soundtrack.effect, 1f);
        }

        void Update()
        {
            cot++;
            if(cot> checkInv)
            {
                cot = 0;
                CheckAudioList();
            }
        }

        public void Play(string url, int trackCompare, bool is_loop)
        {
            switch (trackCompare)
            {
                case (int)soundtrack.effect:
                    PlayEffect(url, is_loop);
                    break;
                case (int)soundtrack.music:
                    PlayMusic(url, is_loop);
                    break;
            }
        }

        public void Stop(string url, int trackCompare)
        {
            switch (trackCompare)
            {
                case (int)soundtrack.effect:
                    StopEffect(url);
                    break;
                case (int)soundtrack.music:
                    StopMusic(url);
                    break;
            }
        }
        public void SetVolume(float volume, soundtrack trackCompare)
        {
            volume = Mathf.Clamp01(volume);
            if (trackCompare == soundtrack.all)
            {
                SetVolume(volume, soundtrack.music);
                //SetVolume(volume, soundtrack.voice);
                SetVolume(volume, soundtrack.effect);
            }
            else
            {
                if (volumeDic.ContainsKey(trackCompare))
                {
                    volumeDic[trackCompare] = volume;
                }
                else
                {
                    volumeDic.Add(trackCompare, volume);
                }
            }
            InternalVolume(volume, trackCompare);
        }

        private void InternalVolume(float volume, soundtrack trackCompare)
        {
            volume = Mathf.Clamp01(volume);
            switch (trackCompare)
            {
                case soundtrack.effect:
                    foreach (List<AudioSource> cha in effecs.Values)
                    {
                        for (int i = 0; i < cha.Count; i++)
                        {
                            cha[i].volume = volume;
                        }
                    }
                    break;
                case soundtrack.music:
                    foreach (AudioSource cha in musics.Values)
                    {
                        cha.volume = volume;
                    }
                    break;
            }
        }

        private void LoadAudio(string path, System.Action<AudioClip> callback)
        {
            Action<UnityEngine.Object> on_cfg = (asset) =>
            {
                if (asset != null)
                {
                    callback(asset as AudioClip);
                }
            };
            ResMgr.LoadAssetAsync(path, typeof(AudioClip), on_cfg);
        }
        //播放指定背景音乐的接口
        public void PlayMusic(string url, bool is_loop = true)
        {
            GameObject s = new GameObject(url);//创建一个空节点
            s.transform.parent = this.transform;//加入节点到场景中
            AudioSource audio_source = s.AddComponent<AudioSource>();//空节点添加组件AudioSource
            LoadAudio(url, (clip) =>
            {
                audio_source.clip = clip;//设置组件的clip属性为clip
                audio_source.loop = is_loop;//设置组件循环播放
                audio_source.playOnAwake = true;//再次唤醒时播放声音
                audio_source.spatialBlend = 0.0f;
                audio_source.mute = IsMusicOpen;
                audio_source.enabled = true;
                audio_source.volume = volumeDic[soundtrack.music];
                audio_source.Play();
            });
            musics.Add(url, audio_source);
        }

        //停止播放指定背景音乐的接口
        public void StopMusic(string url)
        {
            AudioSource audio_source = null;
            if (!musics.ContainsKey(url))
            {
                return;
            }
            audio_source = musics[url];
            audio_source.Stop();
        }

        //停止播放所有背景音乐的接口
        public void StopAllMusic()
        {
            foreach (AudioSource s in musics.Values)
            {
                s.Stop();
            }
        }

        //删除指定背景音乐和它的节点
        public void ClearMusic(string url)
        {
            AudioSource audio_source = null;
            if (!musics.ContainsKey(url))//判断是否已经在背景音乐表里面了
            {
                return;//没有这个背景音乐就直接返回
            }
            audio_source = musics[url];//有就把audio_source直接赋值过去
            musics[url] = null;//指定audio_source组件清空
            GameObject.Destroy(audio_source.gameObject);//删除掉挂载指定audio_source组件的节点
        }

        //切换背景音乐静音开关
        public void SwitchMusic()
        {
            // 切换静音和有声音的状态
            IsMusicOpen = !IsMusicOpen;

            // 遍历所有背景音乐的AudioSource元素
            foreach (AudioSource s in musics.Values)
            {
                s.mute = IsMusicOpen;//设置为当前的状态
            }
        }


        //接下来开始是音效的接口
        //播放指定音效的接口
        private void PlayEffect(string url, bool is_loop = false)
        {
            AudioSource audio_source = null;
            if (effecs.ContainsKey(url))
            {
                for (int i = 0; i < effecs[url].Count; i++)
                {
                    if (!effecs[url][i].isPlaying)
                    {
                        audio_source = effecs[url][i];
                        break;
                    }
                }
            }
            if (audio_source != null)
            {
                audio_source.mute = _IsSoundOpen;
                audio_source.enabled = true;
                audio_source.volume = volumeDic[soundtrack.effect];
                audio_source.Play();
            }
            else
            {
                GameObject s = new GameObject(url);//创建一个空节点
                s.transform.parent = this.transform;//加入节点到场景中

                audio_source = s.AddComponent<AudioSource>();//空节点添加组件AudioSource
                //AudioClip clip = Resources.Load<AudioClip>(url);//代码加载一个AudioClip资源文件
                LoadAudio(url, (clipass) =>
                {
                    if(clipass == null || clipass as AudioClip == null)
                    {
                        return;
                    }
                    audio_source.clip = clipass as AudioClip;//设置组件的clip属性为clip
                    audio_source.loop = is_loop;//设置组件循环播放
                    audio_source.playOnAwake = true;//再次唤醒时播放声音
                    audio_source.spatialBlend = 0.0f;//设置为2D声音
                    audio_source.mute = _IsSoundOpen;
                    audio_source.enabled = true;
                    audio_source.volume = volumeDic[soundtrack.effect];
                    audio_source.Play();//开始播放
                });
                if (effecs.ContainsKey(url))
                {
                    effecs[url].Add(audio_source);
                }
                else
                {
                    effecs.Add(url, new List<AudioSource>() { audio_source });
                }
            }
        }


        //停止播放指定音效的接口
        public void StopEffect(string url)
        {
            if (!effecs.ContainsKey(url))
            {
                return;
            }
            List<AudioSource> audio_source = effecs[url];
            for (int i = 0; i < audio_source.Count; i++)
            {
                audio_source[i].Stop();//停止播放
            }
        }

        //停止播放所有音效的接口
        public void StopAllEffect()
        {
            foreach (List<AudioSource> audio_source in effecs.Values)
            {
                for (int i = 0; i < audio_source.Count; i++)
                {
                    audio_source[i].Stop();//停止播放
                }
            }
        }

        //删除指定音效和它的节点
        public void ClearEffect(string url)
        {
            if (!effecs.ContainsKey(url))//判断是否已经在音效表里面了
            {
                return;//没有这个音效就直接返回
            }
            List<AudioSource> audio_source = effecs[url];
            for (int i = 0; i < audio_source.Count; i++)
            {
                audio_source[i].Stop();//停止播放
                GameObject.Destroy(audio_source[i].gameObject);
            }
            effecs.Remove(url);
        }

        //切换音效静音开关
        public void SwitchEffect()
        {
            // 切换静音和有声音的状态
            _IsSoundOpen = !_IsSoundOpen;

            // 遍历所有音效的AudioSource元素
            foreach (List<AudioSource> audio_source in effecs.Values)
            {
                for (int i = 0; i < audio_source.Count; i++)
                {
                    audio_source[i].mute = _IsSoundOpen;
                }
            }
        }

        //播放3D的音效
        public void PlayEffect3D(string url, Vector3 pos, bool is_loop = false)
        {
            AudioSource audio_source = null;
            GameObject s = new GameObject(url);
            s.transform.parent = this.transform;
            s.transform.position = pos;//3D音效的位置

            audio_source = s.AddComponent<AudioSource>();
            AudioClip clip = Resources.Load<AudioClip>(url);
            audio_source.clip = clip;
            audio_source.loop = is_loop;
            audio_source.playOnAwake = true;
            audio_source.spatialBlend = 1.0f; // 3D音效
            if (effecs.ContainsKey(url))
            {
                effecs[url].Add(audio_source);
            }
            else
            {
                effecs.Add(url, new List<AudioSource>() { audio_source });
            }
            audio_source.mute = _IsSoundOpen;
            audio_source.enabled = true;
            audio_source.Play();
        }




        //优化策略接口
        public void CheckAudioList()
        {
            //遍历背景音乐表
            foreach (AudioSource s in musics.Values)
            {
                if (!s.isPlaying)//判断是否在播放
                {
                    s.enabled = false;//不在播放就直接隐藏
                }
            }

            //遍历音效表
            List<string> det = new List<string>();
            List<AudioSource> nt;
            Dictionary<string, List<AudioSource>> _dict = new Dictionary<string, List<AudioSource>>(effecs);
            if ((_dict != null) && (_dict.Count != 0))
            {
                foreach (KeyValuePair<string, List<AudioSource>> item in _dict)
                {
                    nt = new List<AudioSource>();
                    List<AudioSource> audio_source = item.Value;
                    for (int i = 0; i < audio_source.Count; i++)
                    {
                        if (!audio_source[i].isPlaying)
                        {
                            GameObject.Destroy(audio_source[i].gameObject);
                        }
                        else
                        {
                            nt.Add(audio_source[i]);
                        }
                    }
                    effecs[item.Key] = nt;
                }
            }
        }
    }
}
