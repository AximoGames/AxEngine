﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using System.Media;
using NetCoreAudio;

namespace Aximo.Engine.Audio
{

    public class AudioRack
    {
        public AudioModule[] Modules = Array.Empty<AudioModule>();
        public AudioCable[] Cables = Array.Empty<AudioCable>();

        public IEnumerable<TModule> GetModules<TModule>()
            where TModule : AudioModule
        {
            // SLOW: To not call this method from processing

            var modules = Modules;
            var len = modules.Length;
            for (var i = 0; i < len; i++)
            {
                var mod = modules[i] as TModule;
                if (mod != null)
                    yield return mod;
            }
        }

        public void AddModule(AudioModule module)
        {
            Modules = Modules.AppendElement(module);
            module.Rack = this;
        }

        public void AddCable(AudioCable cable)
        {
            Cables = Cables.AppendElement(cable);
            cable.CableInput.AddCable(cable);
            cable.CableOutput.AddCable(cable);
        }

        public void AddCable(Port moduleOutput, Port moduleInput)
        {
            AddCable(new AudioCable(moduleOutput, moduleInput));
        }

        public void RemoveCable(AudioCable cable)
        {
            Cables = Cables.RemoveElement(cable);
            cable.CableInput.RemoveCable(cable);
            cable.CableOutput.RemoveCable(cable);
        }

        public long Tick;

        public void Process()
        {
            var tasks = Tasks;
            lock (tasks)
            {
                while (tasks.Count > 0)
                {
                    var task = tasks.Dequeue();
                    task();
                }
            }

            var modules = Modules;
            var len = modules.Length;
            for (var i = 0; i < len; i++)
                modules[i].Process();

            var cables = Cables;
            len = cables.Length;
            for (var i = 0; i < len; i++)
                cables[i].Process();

            Tick++;
        }

        private bool Running;
        public void MainLoop()
        {
            Running = true;
            while (Running)
                Process();
        }

        private Thread Thread;
        public void StartThread()
        {
            Thread = new Thread(MainLoop);
            Thread.Priority = ThreadPriority.Highest;
            Thread.IsBackground = false;
            Thread.Start();
            Thread.Sleep(1000); // TODO: use waiter!
        }

        private Queue<Action> Tasks = new Queue<Action>();
        public void Dispatch(Action task)
        {
            lock (Tasks)
                Tasks.Enqueue(task);
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
