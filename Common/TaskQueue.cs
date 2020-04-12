// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Aximo.Engine
{
    public class TaskQueue
    {
        private Queue<Action> Tasks = new Queue<Action>();

        public void Dispatch(Action act)
        {
            lock (Tasks)
                Tasks.Enqueue(act);
        }

        public void ProcessTasks()
        {
            while (Tasks.Count > 0)
            {
                Action act;
                lock (Tasks)
                    act = Tasks.Dequeue();

                act?.Invoke();
            }
        }
    }
}
