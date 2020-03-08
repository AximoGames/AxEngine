using System;
using System.Media;

namespace AxEngine
{

    public class AudioManager
    {

        public static AudioManager Default { get; private set; }

        private SoundPlayer Player;

        static AudioManager()
        {
            Default = new AudioManager();
        }

        public AudioManager()
        {
            Player = new SoundPlayer();
        }

        private string GetPath(string path)
        {
            return DirectoryHelper.GetPath(path);
        }

        public void PlayAsync(string path)
        {
            Player.Stop();
            Player.SoundLocation = GetPath(path);
            Player.Play();
        }

        public void PlaySync(string path)
        {
            Player.Stop();
            Player.SoundLocation = GetPath(path);
            Player.PlaySync();
        }

    }

}