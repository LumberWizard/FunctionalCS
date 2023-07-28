namespace FunctionalCS
{
    internal static class CoInductiveLinkedList
    {
        public delegate void LinkedList<T>(Action nil, Action<T, LinkedList<T>> cons);

        public static LinkedList<T> Nil<T>() => (nil, cons) => nil();

        public static LinkedList<T> Cons<T>(T h, LinkedList<T> t) => (nil, cons) => cons(h, t);

        public static LinkedList<T> Filter<T>(this LinkedList<T> list, Func<T, bool> filter) =>
            (nil, cons) => list(nil, (h, t) =>
            {
                if (filter(h)) cons(h, t.Filter(filter));
                else t.Filter(filter)(nil, cons);
            });

        public static LinkedList<S> Map<T, S>(this LinkedList<T> list, Func<T, S> mapping) =>
            (nil, cons) => list(nil, (h, t) => cons(mapping(h), t.Map(mapping)));

        public static Action<Action<S>> Reduce<T, S>(this LinkedList<T> list, Func<S, T, S> reducer, S accumulator) =>
            action => list(() => action(accumulator), (h, t) => t.Reduce(reducer, reducer(accumulator, h))(action));

        public static LinkedList<T> Concat<T>(this LinkedList<T> list1, LinkedList<T> list2) =>
            (nil, cons) => list1(() => list2(nil, cons), (h, t) => cons(h, t.Concat(list2)));

        public static void ForEach<T>(this LinkedList<T> list, Action<T> action) =>
            list(() => { }, (h, t) => { action(h); t.ForEach(action); });

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> enumerable) =>
            (nil, cons) =>
            {
                if (enumerable.Any()) cons(enumerable.First(), enumerable.Skip(1).ToLinkedList());
                else nil();
            };
    }
}
