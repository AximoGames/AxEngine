// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Aximo.Render
{
    public class SlotAllocator<T>
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<SlotAllocator<T>>();

        private HashSet<T> UsedNumbers = new HashSet<T>();
        private List<T> FreeNumbers;
        public string Name { get; set; }

        public SlotAllocator(IEnumerable<T> freeNumbers) : this(freeNumbers, "SlotAllocator")
        {
        }

        public SlotAllocator(IEnumerable<T> freeNumbers, string name)
        {
            Name = name;
            Init(freeNumbers);
        }

        public void Init(IEnumerable<T> freeNumbers)
        {
            FreeNumbers = new List<T>(freeNumbers);
            Log.ForContext(Name).Verbose("Initialized with {Count} elements", FreeNumbers.Count);
        }

        public T Alloc()
        {
            if (FreeNumbers.Count == 0)
                throw new Exception("No free slots available");

            var num = FreeNumbers[FreeNumbers.Count - 1];
            FreeNumbers.Remove(num);
            UsedNumbers.Add(num);
            Log.ForContext(Name).Verbose("Allocated Slot {Slot}. Remaining elements: {Remaining}", num, FreeNumbers.Count);
            return num;
        }

        public void Free(T num)
        {
            if (UsedNumbers.Remove(num))
            {
                FreeNumbers.Add(num);
                Log.ForContext(Name).Verbose("Free Slot {Slot}. Available elements: {Remaining}", num, FreeNumbers.Count);
            }
        }
    }
}
