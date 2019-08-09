namespace Utils
{
    public abstract class SingletonBase<T> where T : SingletonBase<T>, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }

        protected SingletonBase()
        {

        }
    }
}