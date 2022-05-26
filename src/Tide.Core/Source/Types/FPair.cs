namespace Tide.Core
{
    public struct FPair<T>
    {
        public FPair(T a, T b)
        {
            A = a;
            B = b;
        }

        public T A { get; set; }
        public T B { get; set; }

        public FPair<T> Flipped()
        {
            return new FPair<T>(B, A);
        }
    }
}