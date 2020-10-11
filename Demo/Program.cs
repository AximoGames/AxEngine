// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using Aximo.Engine;
using McMaster.NETCore.Plugins;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.AxDemo
{

    internal class Program
    {

        public static int Counter = 0;

        public static void Main(string[] args)
        {
            ////var obj = PluginManager.GetScriptBehaviour<TestClass>();
            ////obj.Invoke();

            //var random = new Random();
            ////var dic = new Dictionary<int, object>();
            ////var dic = new Hashtable();
            //var dic = new FastDictionary<int, object>();

            ////var keys = new string[] {
            ////    "abcdefg0",
            ////    "abcdefg1",
            ////    "abcdefg2",
            ////    "abcdefg3",
            ////    "abcdefg4",
            ////    //"abcdefg5",
            ////    //"abcdefg6",
            ////    //"abcdefg7",
            ////    //"abcdefg8",
            ////    //"abcdefg9",
            ////};

            //var keys = new int[] {
            //    0,1,2,3,4
            //};

            //foreach (var key in keys)
            //    dic.Add(key, null);

            //object result = null;
            //var array = new int[200000000];
            //for (var i = 0; i < array.Length - 1; i++)
            //{
            //    array[i] = random.Next(0, keys.Length - 1);
            //}
            //GC.Collect();
            //var watch = new Stopwatch();
            //watch.Start();
            //for (var i = 0; i < 200000000 - 1; i++)
            //{
            //    result = dic[keys[array[i]]];
            //}
            //watch.Stop();

            //Console.WriteLine($"Done {watch.ElapsedMilliseconds}");
            //Console.ReadLine();

            var config = new ApplicationConfig
            {
                WindowTitle = "AxEngineDemo",
                WindowSize = new Vector2i(800, 600),
                WindowBorder = WindowBorder.Resizable,
                IsMultiThreaded = true,
                RenderFrequency = 0,
                UpdateFrequency = 0,
                IdleRenderFrequency = 0,
                IdleUpdateFrequency = 0,
                VSync = VSyncMode.Off,
                // UseGtkUI = true,
                UseConsole = true,
                NormalizedUISize = new Vector2(800, 600),
                //UseFrameDebug = true,
            };
            new DemoApplication().Start(config);
        }
    }
}
