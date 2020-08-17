using System;

namespace J.H_D.Minions
{
    public struct FeatureRequest<T, U>
        where U : Enum
    {
        public FeatureRequest(T answer, U error)
        {
            this.answer = answer;
            this.error = error;
        }

        private T answer;
        private U error;

        public T Answer { get => answer; set => answer = value; }
        public U Error { get => error; set => error = value; }
    }
}
