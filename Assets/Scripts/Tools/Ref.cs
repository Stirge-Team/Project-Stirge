namespace Stirge.Tools
{
    public class Ref<T> where T : class
    {
        private T backing;
        public T Value { get { return backing; } }
        public Ref(T reference)
        {
            backing = reference;
        }

        public void SetNull()
        {
            backing = null;
        }
    }
}