﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Aximo
{
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
                    _SearchDirectories = new List<string> { AppSourceDir, AppRootDir, EngineRootDir };
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
            return "";
        }

    }

}