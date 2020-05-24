// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Mathematics;

#pragma warning disable SA1649 // File name should match first type name

namespace Aximo
{
    public interface IPosition
    {
        Vector3 Position { get; set; }
    }

    public interface IData
    {
        T GetExtraData<T>(string name, T defaultValue = default);
        bool HasExtraData(string name);
        bool SetExraData<T>(string name, T value, T defaultValue = default);
    }

    internal static class IDataHelper
    {
        public static T GetData<T>(Dictionary<string, object> data, string name, T defaultValue = default)
        {
            if (data.TryGetValue(name, out object value))
                return (T)value;
            return default;
        }

        public static bool HasData(Dictionary<string, object> data, string name)
        {
            return data.ContainsKey(name);
        }

        public static bool SetData<T>(Dictionary<string, object> data, string name, T value, T defaultValue = default)
        {
            if (data.TryGetValue(name, out object currentValue))
            {
                if (Equals(value, defaultValue))
                {
                    data.Remove(name);
                    return true;
                }
                else
                {
                    if (Equals(currentValue, value))
                        return false;

                    data[name] = value;
                    return true;
                }
            }
            else
            {
                if (Equals(value, defaultValue))
                    return false;

                data.Add(name, value);
                return true;
            }
        }
    }
}
