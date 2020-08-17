using System;

namespace J.H_D.Minions
{
    public struct FeatureRequest<T, U> : IEquatable<FeatureRequest<T, U>>
        where U : Enum
    {
        public FeatureRequest(T answer, U error)
        {
            this.Answer = answer;
            this.Error = error;
        }

        public T Answer { get; set; }
        public U Error { get; set; }

        public bool Equals(FeatureRequest<T, U> other)
        {
            return
                GetHashCode() == other.GetHashCode();
        }
    }
}
