// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using GLib;
using OpenToolkit.Audio;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio
{
    public class AudioDebugLogValuesModule : AudioModule
    {
        private Stream Stream;
        private StreamWriter StreamWriter;

        public void SetOutputStream(Stream stream)
        {
            Stream = stream;
            StreamWriter = new StreamWriter(stream);
        }

        public void SetOutputFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);

            SetOutputStream(File.OpenWrite(file));
        }

        public AudioDebugLogValuesModule()
        {
            Name = "Debug Values";
            ConfigureInput("Probe", 0);
            InputChannels = new Port[] { Inputs[0] };
        }

        private Port[] InputChannels;

        private short Value;
        public override void Process()
        {
            if (Stream != null)
                for (var i = 0; i < InputChannels.Length; i++)
                {
                    //var v = 4.8822002f;
                    var voltage = InputChannels[i].GetVoltage();
                    var value = PCMConversion.FloatToShort(voltage);
                    if (value == -3 && value == Value)
                    {
                        var s = "";
                    }
                    Value = value;
                    StreamWriter.WriteLine(Rack.Tick + ": " + value);
                    //if (AxMath.Approximately(voltage, v))
                    //{
                    //    var s = "";
                    //}
                }
        }
    }
}
