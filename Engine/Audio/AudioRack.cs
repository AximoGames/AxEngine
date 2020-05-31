// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using System.Media;
using NetCoreAudio;

namespace Aximo.Engine.Audio
{
    public class AudioCable
    {
        public Port Input;
        public Port Output;

        public AudioCable(Port input, Port output)
        {
            Input = input;
            Output = output;
        }

        public void Process()
        {
            Output.SetVoltage(Input.GetVoltage());
        }
    }

    public class AudioRack
    {
        public AudioModule[] Modules = Array.Empty<AudioModule>();
        public AudioCable[] Cables = Array.Empty<AudioCable>();

        public void AddModule(AudioModule module)
        {
            Modules = Modules.AppendElement(module);
        }

        public void AddCable(AudioCable cable)
        {
            Cables = Cables.AppendElement(cable);
        }

        public long Tick;

        public void Process()
        {
            for (var i = 0; i < Modules.Length; i++)
                Modules[i].Process();
            for (var i = 0; i < Cables.Length; i++)
                Cables[i].Process();
            Tick++;
        }

        private bool Running;
        public void MainLoop()
        {
            Running = true;
            while (Running)
            {
                Process();
            }
        }

        public void Stop()
        {
            Running = false;
        }

        public AudioModule GetModule(string name)
        {
            return Modules.FirstOrDefault(m => m.Name == name);
        }
    }
}
