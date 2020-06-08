// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo.Engine.Audio
{
    public abstract class AudioModule
    {
        internal AudioRack Rack;
        public Port[] Outputs = Array.Empty<Port>();
        public Port[] Inputs = Array.Empty<Port>();
        public AudioParameter[] Parameters = Array.Empty<AudioParameter>();

        public Port GetOutput(string name) => Outputs.FirstOrDefault(p => p.Name == name);
        public Port GetInput(string name) => Inputs.FirstOrDefault(p => p.Name == name);

        public AudioParameter GetParameter(string name) => Parameters.FirstOrDefault(p => p.Name == name);

        public string Name;

        public abstract void Process();

        protected Port ConfigureOutput(string name, int i = 0)
        {
            Outputs = Outputs.EnsureSize(i + 1);
            if (Outputs[i] == null)
                Outputs[i] = new Port(this, PortDirection.Output, name);

            var port = Outputs[i];
            port.Name = name;
            return port;
        }

        protected Port ConfigureInput(string name, int i = 0)
        {
            Inputs = Inputs.EnsureSize(i + 1);
            if (Inputs[i] == null)
                Inputs[i] = new Port(this, PortDirection.Input, name);
            var port = Inputs[i];
            port.Name = name;
            return port;
        }

        protected AudioParameter ConfigureParameter(string name, int i, float min, float max, float? value)
        {
            Parameters = Parameters.EnsureSize(i + 1);
            if (Parameters[i] == null)
                Parameters[i] = new AudioParameter(this, name, min, max);
            var parameter = Parameters[i];
            if (value != null)
                parameter.Value = (float)value;
            return parameter;
        }
    }
}
