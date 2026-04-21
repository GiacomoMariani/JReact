using System;

namespace JReact
{
    public struct JCyclicState
    {
        public struct JECSEventRingState : IEquatable<JECSEventRingState>
        {
            public int NextWrite;
            public int Count;
            public int Capacity;
            public int Version;
        
            public JECSEventRingState Advance()
            {
                NextWrite = (NextWrite + 1) % Capacity;
                if (Count < Capacity) { Count++; }
                Version++;
                return this;
            }
            
            public JECSEventRingState Clear()
            {
                NextWrite = 0;
                Count     = 0;
                return this;
            }

            public bool IsEmpty()
                => Count == 0;

            public bool IsFull() => Count == Capacity;

            public bool Equals(JECSEventRingState other) => NextWrite == other.NextWrite && Count == other.Count && Capacity == other.Capacity && Version == other.Version;

            public override bool Equals(object obj) => obj is JECSEventRingState other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(NextWrite, Count, Capacity, Version);
        }
    }
}
