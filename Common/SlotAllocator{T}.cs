// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Aximo.Render
{
    public class SlotAllocator<T>
    {
        private static HashSet<T> UsedNumbers = new HashSet<T>();
        private static List<T> FreeNumbers;

        public SlotAllocator(IEnumerable<T> freeNumbers)
        {
            Init(freeNumbers);
        }

        public void Init(IEnumerable<T> freeNumbers)
        {
            FreeNumbers = new List<T>(freeNumbers);
        }

        public T Alloc()
        {
            var num = FreeNumbers[FreeNumbers.Count - 1];
            FreeNumbers.Remove(num);
            UsedNumbers.Add(num);
            return num;
        }

        public void Free(T num)
        {
            UsedNumbers.Remove(num);
            FreeNumbers.Add(num);
        }
    }
}
