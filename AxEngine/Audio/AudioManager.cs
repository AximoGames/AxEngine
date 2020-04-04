// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
//using System.Media;
using NetCoreAudio;

namespace Aximo.Engine
{

    public class AudioManager
    {

        public bool Mute => CommandLineOptions.Current.Mute;

        public static AudioManager Default { get; private set; }

        //private SoundPlayer Player;
        private Player Player;

        static AudioManager()
        {
            Default = new AudioManager();
        }

        public AudioManager()
        {
            //  Player = new SoundPlayer();
            Player = new Player();
        }

        private string GetPath(string path)
        {
            return DirectoryHelper.GetAssetsPath(path);
        }

        public void PlayAsync(string path)
        {
            if (Mute) return;

            // Player.Stop();
            // Player.SoundLocation = GetPath(path);
            // Player.Play();

            Player.Stop();
            Player.Play(GetPath(path));
        }

        public void PlaySync(string path)
        {
            if (Mute) return;

            // Player.Stop();
            // Player.SoundLocation = GetPath(path);
            // Player.PlaySync();

            throw new NotSupportedException();
        }

    }

}