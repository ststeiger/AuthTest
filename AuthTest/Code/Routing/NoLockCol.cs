
namespace CoreCms
{


    // https://stackoverflow.com/questions/10675400/threadsafe-collection-without-lock
    public class NonLockingThreadSafeHashSet<T>
    {

        private System.Collections.Generic.IEqualityComparer<T> m_comparer;
        private System.Collections.Generic.HashSet<T> collection;


        public NonLockingThreadSafeHashSet(System.Collections.Generic.IEqualityComparer<T> comp)
        {
            this.m_comparer = comp;
            System.Threading.Interlocked.CompareExchange(
                  ref collection
                , new System.Collections.Generic.HashSet<T>(m_comparer)
                , null
            );

        } // End Constructor 


        public NonLockingThreadSafeHashSet() : this(null)
        { } // End Constructor 


        public void Add(T s)
        {
            while (true)
            {
                // Volatile read of global collection.
                System.Collections.Generic.HashSet<T> original = 
                    System.Threading.Interlocked.CompareExchange(ref collection, null, null);

                // Add new string to a local copy.
                System.Collections.Generic.HashSet<T> copy = 
                    new System.Collections.Generic.HashSet<T>(original, this.m_comparer);
                copy.Add(s);

                // Swap local copy with global collection, unless outraced by another thread.
                System.Collections.Generic.HashSet<T> result = 
                    System.Threading.Interlocked.CompareExchange(ref collection, copy, original);

                if (result == original)
                    break;
            } // Whend 

        } // End Sub Add 


        public bool Contains(T item)
        {
            return this.collection.Contains(item);
        } // End Function Contains 


        public bool this[T index]
        {
            get
            {
                return this.collection.Contains(index);
            }

        } // End Property this 


    } // End Class NonLockingHashSet



    public class NonLockingCollection
    {
        private System.Collections.Generic.List<string> collection;


        public NonLockingCollection()
        {
            // Initialize global collection through a volatile write.
            System.Threading.Interlocked.CompareExchange(
                  ref collection
                , new System.Collections.Generic.List<string>()
                , null
            );
        } // End Constructor 


        public void AddString(string s)
        {
            while (true)
            {
                // Volatile read of global collection.
                System.Collections.Generic.List<string> original = 
                    System.Threading.Interlocked.CompareExchange(ref collection, null, null);

                // Add new string to a local copy.
                System.Collections.Generic.List<string> copy = new System.Collections.Generic.List<string>(original);
                copy.Add(s);

                // Swap local copy with global collection,
                // unless outraced by another thread.
                System.Collections.Generic.List<string> result = 
                    System.Threading.Interlocked.CompareExchange(ref collection, copy, original);

                if (result == original)
                    break;
            } // Whend 

        } // End Sub AddString 


        public override string ToString()
        {
            // Volatile read of global collection.
            System.Collections.Generic.List<string> original = 
                System.Threading.Interlocked.CompareExchange(ref collection, null, null);

            // Since content of global collection will never be modified,
            // we may read it directly.
            return string.Join(",", original);
        } // End Function ToString 


    } // End Class NonLockingThreadSafeHashSet<T> 


} // End Namespace CoreCms 
