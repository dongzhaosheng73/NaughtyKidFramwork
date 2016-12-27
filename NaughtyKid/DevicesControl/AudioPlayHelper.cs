using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NaughtyKid.MyTools;
using NaughtyKid.WinAPI;
using Timer = System.Windows.Forms.Timer;

namespace NaughtyKid.DevicesControl
{
    /// <summary>
    /// *************************************************
    /// 类名：MP3帮助类
    /// 修改日期：2016/06/25
    /// 作者：董兆生
    /// 联系方式：QQ490412323
    /// *************************************************
    /// </summary>
    public class AudioPlayHelper :IDisposable
    {
        /// <summary>
        /// 播放状态
        /// </summary>
        private PlayState _palystate = PlayState.Closed;
        public  PlayState PlayState
        {
            set
            {
                if (value == _palystate) return;
                OnPropertyChanged(value);
                _palystate = value;
            }
            get { return _palystate; }
        }
        /// <summary>
        /// 操作错误事件
        /// </summary>
        public event Errordelegate EventAudioplayerror;
        /// <summary>
        /// 播放完毕事件
        /// </summary>
        public event PlayEnd EventAudioPlayEnd;
        /// <summary>
        /// 播放状态发生变化事件
        /// </summary>
        public event DelegatePlayStateChange PlayStatePropertyChanged;
        /// <summary>
        /// 播放时间变化事件
        /// </summary>
        public event DelegatePlayNowTime EventPlayTimeChange;
        /// <summary>
        /// 时间长度毫秒
        /// </summary>
        public readonly StringBuilder PlayLenght = new StringBuilder(255);      
        /// <summary>
        /// 播放音量
        /// </summary>
        private  int _playvlome = 1000;
        public int PlayVolume{get { return _playvlome; }}
        /// <summary>
        /// 当前播放时间
        /// </summary>      
        public int NowPlayTime
        {
            get { return int.Parse(_nowplaytime.ToString()); }
        }
        private readonly StringBuilder _nowplaytime = new StringBuilder(255);

        private  AudioModel _nowPlayData;
        /// <summary>
        /// 当前播放歌曲
        /// </summary>
        public AudioModel NowPlayData { get { return _nowPlayData; } }
        /// <summary>
        /// 播放列表
        /// </summary>
        public List<AudioModel> PlayList = new List<AudioModel>();

        private int _playindex;
        /// <summary>
        /// 当前播放歌曲在列表中的序号
        /// </summary>
        public int PlayIndex {  get { return _playindex; } }
        /// <summary>
        /// 是否单曲循环播放
        /// </summary>
        public bool IsSingleLoop { set; get; }
        /// <summary>
        /// 是否列表循环播放
        /// </summary>
        public bool IsListLoop { set; get; }
        /// <summary>
        /// 随机循环播放
        /// </summary>
        public bool IsRandLoop { set; get; }

        private Random _random;

        public AudioPlayHelper()
        {
            _playindex = -1;

            PlayList = new List<AudioModel>();

            _nowPlayData = new AudioModel();

            var playTimer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };

            playTimer.Tick += playTimer_Tick;
        }

        public AudioPlayHelper(AudioModel playdata)
        {
            _nowPlayData = (AudioModel)playdata.Clone();
            PlayList .Add(_nowPlayData);       
            _playindex = 0;
  
            var playTimer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };

            playTimer.Tick += playTimer_Tick;
        }

        public AudioPlayHelper(List<AudioModel> playList)
        {
            PlayList = new List<AudioModel>(playList);
            _nowPlayData = PlayList[0];          
            _playindex = 0;

            var playTimer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };
            playTimer.Tick += playTimer_Tick;
        }

        public bool NextPlay()
        {
            if (PlayList==null) return false;

            if (_playindex + 1 >= PlayList.Count) return false;

            Closed();

            _nowPlayData = PlayList[_playindex + 1];

            Open();

            SetVolume(PlayVolume);

            Play();

            _playindex = _playindex + 1;

            return true;
        }

        public bool PreviousPlay()
        {
            if (PlayList==null) return false;

            if (_playindex - 1 <0 ) return false;

            Closed();

            _nowPlayData = PlayList[_playindex - 1];

            Open();

            SetVolume(PlayVolume);

            Play();

            _playindex = _playindex - 1;

            return true;
        }

        public bool JumpPlay(int index)
        {
            if (PlayList == null) return false;

            if (index < 0 && index >= PlayList.Count - 1) return false;

             Closed();

            _nowPlayData = PlayList[index];

            Open();

            SetVolume(PlayVolume);

            Play();

            _playindex = index;

            return true;
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            if (PlayState != PlayState.Playing) return;

            DoOrder(string.Format("status {0} position", _nowPlayData.AliasMovie), _nowplaytime, _nowplaytime.Capacity);

            var returntimeThread = new Thread(ThreadReturnTime) {IsBackground = true};

            returntimeThread.Start(_nowplaytime);

            if (!_nowplaytime.Equals(PlayLenght)) return;

             Closed();

            _palystate = PlayState.Closed;

            if (EventAudioPlayEnd !=null) EventAudioPlayEnd();

            if (IsRandLoop)
            {

                _random = new Random((int)DateTime.Now.Ticks);

                _playindex = _random.Next(0, PlayList.Count);

                _nowPlayData = PlayList[_playindex];

                JumpPlay(_playindex);

                return;
            }

            if (IsListLoop)
            {

                if (_playindex + 1 >= PlayList.Count)
                {
                    _playindex = 0;
                }
                else
                {
                    _playindex = _playindex + 1;
                }

                _nowPlayData = PlayList[_playindex];

                JumpPlay(_playindex);

                return;
            }

            if (!IsSingleLoop) return;

            JumpPlay(_playindex);
        }

        private void ThreadReturnTime(object time)
        {
            if(_palystate!=PlayState.Playing) return;

            if (EventPlayTimeChange != null) EventPlayTimeChange(int.Parse(time.ToString()));
        }

   
        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="order">指令</param>
        /// <param name="returnString">需要返回的数据</param>
        /// <param name="returnsize">返回数据大小</param>
        /// <returns></returns>
        private bool DoOrder(string order, StringBuilder returnString,int returnsize)
        {
            var error = WinApiHepler.mciSendString(order, returnString, returnsize, new IntPtr());

            if(IsDisplsed)return false;

            if (error == 0) return true;

            Errorlog(error);

            return false;
        }

        /// <summary>
        /// 添加播放文件
        /// </summary>
        /// <param name="playlist">播放列表</param>
        public void AddAudioFiles(List<AudioModel> playlist)
        {
            PlayList =  PlayList.Union(playlist).ToList();
        }

        public void AddAudioFile(AudioModel playfile)
        {
           if(PlayList.Contains(playfile)!=true)PlayList.Add(playfile);
        }

        public void RemoveAudioFile(AudioModel playaudio)
        {
            if(PlayList.Contains(playaudio)!=true)return;

            if (playaudio.Equals(_nowPlayData)) Closed();

            PlayList.Remove(playaudio);

            if (_playindex >= 0 && _playindex < PlayList.Count)
            {
                _nowPlayData = PlayList[_playindex];
            }
        }

        /// <summary>
        /// 事件格式化
        /// </summary>
        /// <param name="millisecond">毫秒</param>
        /// <returns>hh:mm:ss</returns>
        public static string TimeFormat(int millisecond)
        {
            var time = new TimeSpan(0, 0, 0, 0, millisecond);

            return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }
        /// <summary>
        /// 获得当前状态
        /// </summary>
        /// <returns></returns>
        public PlayState GetPlyaState()
        {
            var state = new StringBuilder(50);

            return DoOrder(string.Format("status {0} mode", _nowPlayData.AliasMovie), state, state.Capacity) != true ? PlayState.Error : (PlayState)Enum.Parse(typeof(PlayState), state.ToString());
        }
        /// <summary>
        /// 打开音乐文件
        /// </summary>
        public void Open()
        {
            PlayState = DoOrder(string.Format("open {0} alias {1}", _nowPlayData.ShortPath, _nowPlayData.AliasMovie), null, 0) != true ? PlayState.Error : PlayState.Opne;

            if (_palystate != PlayState.Opne) return;

            DoOrder(string.Format("status {0} length", _nowPlayData.AliasMovie), PlayLenght, PlayLenght.Capacity);

        }
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        public void Play(int starttime,int endtime)
        {
            PlayState = DoOrder(string.Format("play {0} from {1} to {2} notify", _nowPlayData.AliasMovie, starttime, endtime), null, 0) != true ? PlayState.Error : PlayState.Playing;         
        }
        /// <summary>
        /// 播放音乐
        /// </summary>
        public void Play()
        {
            PlayState = DoOrder(string.Format("play {0} notify", _nowPlayData.AliasMovie), null, 0) != true ? PlayState.Error : PlayState.Playing;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            PlayState = DoOrder(string.Format("pause {0}", _nowPlayData.AliasMovie), null, 0) != true ? PlayState.Error : PlayState.Paused;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            PlayState = DoOrder(string.Format("stop {0}", _nowPlayData.AliasMovie), null, 0) != true ? PlayState.Error : PlayState.Stopped;         
        }
        /// <summary>
        /// 关闭音乐
        /// </summary>
        public void Closed()
        {
            PlayState = DoOrder(string.Format("close {0}", _nowPlayData.AliasMovie), null, 0) != true ? PlayState.Error : PlayState.Closed;      
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="volume">0-1000</param>
        /// <returns></returns>
        public  bool SetVolume(int volume)
        {
            if (!DoOrder(string.Format("setaudio {0} volume to {1}", _nowPlayData.AliasMovie, volume), null, 0))
                return false;
            _playvlome = volume;
            return true;
        }

        private void Errorlog(int error)
        {
            var errorText = new StringBuilder(50);

            WinApiHepler.mciGetErrorString(error, errorText, errorText.Capacity);

            if (EventAudioplayerror == null) return;

            EventAudioplayerror(errorText.ToString());
        }

        private void OnPropertyChanged(PlayState state)
        {
            if (PlayStatePropertyChanged != null)
            {
                PlayStatePropertyChanged(state);
            }
        }

       protected bool IsDisplsed { get; set; }

       ~AudioPlayHelper()
        {
            Dispose();
        }
 
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(IsDisplsed);
        }
        public virtual void Dispose(bool isDisplsed)
        {
            if (isDisplsed)
                return;
               
            EventAudioplayerror = null;

            EventAudioPlayEnd = null;

            EventPlayTimeChange = null;

            DoOrder(string.Format("close {0}", _nowPlayData.AliasMovie), null, 0);        
        }
     
    }
    public delegate void Errordelegate(string error);

    public delegate void PlayEnd();

    public delegate void DelegateHockMesg(Message msg);

    public delegate void DelegatePlayStateChange(PlayState state);

    public delegate void DelegatePlayNowTime(int time);

    //public class WindosMessageInform :UserControl
    //{
    //    int[] HockMsg { set; get; }

    //    public event DelegateHockMesg EventHockmesg;

    //    public WindosMessageInform(int[] hockmsg)
    //    {
    //        HockMsg = hockmsg;
    //    }

    //    protected override void WndProc(ref Message m)
    //    {
    //        if (HockMsg.ToList().Contains(m.Msg))
    //        {
    //            if (EventHockmesg != null) EventHockmesg(m);
    //        }
    //        base.WndProc(ref m);
    //    }
    //}

    public class AudioModel : ICloneable
    {
        public AudioModel()
        {
          
        }

        public AudioModel(string file,string alias,object data)
        {
            PlayFile = file;

            AliasMovie = alias;

            SourceData = data;

            FileName = System.IO.Path.GetFileNameWithoutExtension(file);

            var shortpath = new StringBuilder(255);

            WinApiHepler.GetShortPathName(PlayFile, shortpath, shortpath.Capacity);

            ShortPath = shortpath.ToString();
        }  
        public string FileName { set; get; }
       
        public string PlayFile { set; get; }

        public string ShortPath { set; get; }

        public string AliasMovie { set; get; }

        public object SourceData { set; get; }

        public object Clone()
        {
            var clonedata = SerializationHelper.GetSerialization(this);

            return SerializationHelper.ScriptDeserialize<AudioModel>(clonedata);
        }
    }

    public enum PlayState
    {
        Opne,
        Playing,
        Paused,
        Stopped,
        Closed,
        None,
        Error
    }
}
