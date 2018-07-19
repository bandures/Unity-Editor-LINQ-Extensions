using System;

namespace Unity.Editor.LinqExt
{
    public static partial class LinqExt
    {
        public enum Condition
        {
            Is,
            IsNot,
            Contain,
            NotContain
        }

        internal static Func<T, bool> GetNamePredicate<T>(Condition condition, Func<T, string> predicate, string str)
        {
            switch (condition)
            {
                case Condition.Is:
                    return (x) => predicate(x) == str;
                case Condition.IsNot:
                    return (x) => predicate(x) != str;
                case Condition.Contain:
                    return (x) => predicate(x).Contains(str);
                case Condition.NotContain:
                    return (x) => !predicate(x).Contains(str);
            }
            return (x) => true;
        }

    }
}