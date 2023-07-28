namespace FunctionalCS
{
    internal static class CoInductiveLinkedList
    {
        public delegate void LinkedList<T>(Action nilCallback, Action<T, LinkedList<T>> consCallback);

        public static LinkedList<T> Nil<T>() => (nilCallback, consCallback) => nilCallback();

        public static LinkedList<T> Cons<T>(T h, LinkedList<T> t) => (nilCallback, consCallback) => consCallback(h, t);

        public static LinkedList<T> Filter<T>(this LinkedList<T> list, Func<T, bool> filter) =>
            (nilCallback, consCallback) => list(nilCallback, (h, t) =>
            {
                if (filter(h)) consCallback(h, t.Filter(filter));
                else t.Filter(filter)(nilCallback, consCallback);
            });

        public static LinkedList<S> Map<T, S>(this LinkedList<T> list, Func<T, S> mapping) =>
            (nilCallback, consCallback) => list(nilCallback, (h, t) => consCallback(mapping(h), t.Map(mapping)));

        public static Action<Action<S>> Reduce<T, S>(this LinkedList<T> list, Func<S, T, S> reducer, S accumulator) =>
            action => list(() => action(accumulator), (h, t) => t.Reduce(reducer, reducer(accumulator, h))(action));

        public static LinkedList<T> Concat<T>(this LinkedList<T> list1, LinkedList<T> list2) =>
            (nilCallback, consCallback) => list1(() => list2(nilCallback, consCallback), (h, t) => consCallback(h, t.Concat(list2)));

        public static void ForEach<T>(this LinkedList<T> list, Action<T> action) =>
            list(() => { }, (h, t) => { action(h); t.ForEach(action); });

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> enumerable) =>
            (nilCallback, consCallback) =>
            {
                if (enumerable.Any()) consCallback(enumerable.First(), enumerable.Skip(1).ToLinkedList());
                else nilCallback();
            };
    }
}
