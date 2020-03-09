using System.Media;

namespace Aximo.Engine
{

    public class AudioManager
    {

        public bool Mute => CommandLineOptions.Current.Mute;

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
            return DirectoryHelper.GetAssetsPath(path);
        }

        public void PlayAsync(string path)
        {
            if (Mute) return;

            Player.Stop();
            Player.SoundLocation = GetPath(path);
            Player.Play();
        }

        public void PlaySync(string path)
        {
            if (Mute) return;

            Player.Stop();
            Player.SoundLocation = GetPath(path);
            Player.PlaySync();
        }

    }

}