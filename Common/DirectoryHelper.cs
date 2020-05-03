// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Aximo
{

    public delegate bool GenerateFileDelegate(string subPath, string cachePath);

    public static class DirectoryHelper
    {
        private static string _BinDir;
        public static string BinDir
        {
            get
            {
                if (_BinDir == null)
                    _BinDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
                return _BinDir;
            }
        }

        private static string _AppRootDir;
        public static string AppRootDir
        {
            get
            {
                if (_AppRootDir == null)
                    _AppRootDir = new DirectoryInfo(Path.Combine(BinDir, "..", "..", "..", "..")).FullName;
                return _AppRootDir;
            }
        }

        private static string _LibsDir;
        public static string LibsDir
        {
            get
            {
                if (_LibsDir == null)
                    _LibsDir = new DirectoryInfo(Path.Combine(AppRootDir, "..", "libs", "OpenToolkit")).FullName;
                return _LibsDir;
            }
        }

        private static string _AppSourceDir;
        public static string AppSourceDir
        {
            get
            {
                if (_AppSourceDir == null)
                {
                    _AppSourceDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..")).FullName;
                }
                return _AppSourceDir;
            }
        }

        private static string _AssemblyName;
        public static string AssemblyName
        {
            get
            {
                if (_AssemblyName == null)
                    _AssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                return _AssemblyName;
            }
        }

        private static string _AppCacheDir;
        public static string AppCacheDir
        {
            get
            {
                if (_AppCacheDir == null)
                {
                    _AppCacheDir = Path.Combine(EngineRootDir, ".cache", AssemblyName);
                    Directory.CreateDirectory(_AppCacheDir);
                }
                return _AppCacheDir;
            }
        }

        private static string _EngineRootDir;
        public static string EngineRootDir
        {
            get
            {
                if (_EngineRootDir == null)
                {
                    var dirName = Path.GetFileName(AppRootDir);
                    if (dirName == "AxEngine")
                    {
                        _EngineRootDir = AppRootDir;
                    }
                    else
                    {
                        _EngineRootDir = new DirectoryInfo(Path.Combine(AppRootDir, "..", "AxEngine")).FullName;
                    }
                }
                return _EngineRootDir;
            }
        }

        private static List<string> _SearchDirectories;
        public static List<string> SearchDirectories
        {
            get
            {
                if (_SearchDirectories == null)
                    _SearchDirectories = new List<string> { AppCacheDir, AppSourceDir, AppRootDir, EngineRootDir };
                return _SearchDirectories;
            }
        }

        public static string GetAssetsPath(string subPath)
        {
            foreach (var dir in SearchDirectories)
            {
                var path = Path.Combine(dir, "Assets", subPath);
                if (File.Exists(path))
                    return new FileInfo(path).FullName;
                if (Directory.Exists(path))
                    return new DirectoryInfo(path).FullName;
            }
            if (RequestFile(subPath))
                return GetAssetsPath(subPath);

            return "";
        }

        private static Dictionary<string, GenerateFileDelegate> FileGenerators = new Dictionary<string, GenerateFileDelegate>();

        public static void AddFileGenerator(string subPath, GenerateFileDelegate generator)
        {
            lock (FileGenerators)
                FileGenerators.Add(subPath, generator);
        }

        private static bool RequestFile(string subPath)
        {
            GenerateFileDelegate gen;
            lock (FileGenerators)
                FileGenerators.TryGetValue(subPath, out gen);
            if (gen != null)
            {
                var cachePath = Path.Combine(AppCacheDir, "Assets", subPath);
                var parent = Path.GetDirectoryName(cachePath);
                Directory.CreateDirectory(parent);
                if (gen(subPath, cachePath))
                    return File.Exists(cachePath) || Directory.Exists(cachePath);
            }

            return false;
        }

    }
}
