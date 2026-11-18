using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Helpers
{
    public class History : IEnumerable<int>
    {
        private readonly LinkedList<int> _history;

        public History(int[] history) => _history = new(history);

        public int Add(int value)
        {
            int prev = _history.First.Value;
            
            _history.AddFirst(value);
            _history.RemoveLast();

            return value - prev;
        }
        
        public IEnumerator<int> GetEnumerator() => _history.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}