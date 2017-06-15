using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.support.v4
{
    public class Pools
    {

        /**
         * Interface for managing a pool of objects.
         *
         * @param <T> The pooled type.
         */
        public interface Pool<T>
        {

            /**
             * @return An instance from the pool if such, null otherwise.
             */
            T acquire();

            /**
             * Release an instance to the pool.
             *
             * @param instance The instance to release.
             * @return Whether the instance was put in the pool.
             *
             * @throws IllegalStateException If the instance is already in the pool.
             */
            bool release(T instance);
        }

        private Pools()
        {
            /* do nothing - hiding constructor */
        }

        /**
         * Simple (non-synchronized) pool of objects.
         *
         * @param <T> The pooled type.
         */
        public class SimplePool<T> : Pool<T>
        {
            private object[] mPool;
            private int mPoolSize;

            /**
             * Creates a new instance.
             *
             * @param maxPoolSize The max pool size.
             *
             * @throws IllegalArgumentException If the max pool size is less than zero.
             */
            public SimplePool(int maxPoolSize)
            {
                if (maxPoolSize <= 0)
                {
                    throw new ArgumentException("The max pool size must be > 0");
                }
                mPool = new object[maxPoolSize];
            }

            public virtual T acquire()
            {
                if (mPoolSize > 0)
                {
                    int lastPooledIndex = mPoolSize - 1;
                    T instance = (T)mPool[lastPooledIndex];
                    mPool[lastPooledIndex] = null;
                    mPoolSize--;
                    return instance;
                }

                return default(T);
            }

            public virtual bool release(T instance)
            {
                if (isInPool(instance))
                {
                    throw new Exception("Already in the pool!");
                }
                if (mPoolSize < mPool.Length)
                {
                    mPool[mPoolSize] = instance;
                    mPoolSize++;
                    return true;
                }
                return false;
            }

            private bool isInPool(T instance)
            {
                for (int i = 0; i < mPoolSize; i++)
                {
                    if (mPool[i].Equals(instance))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /**
         * Synchronized) pool of objects.
         *
         * @param <T> The pooled type.
         */
        public class SynchronizedPool<T> : SimplePool<T>
        {
            private object mLock = new object();

            /**
             * Creates a new instance.
             *
             * @param maxPoolSize The max pool size.
             *
             * @throws IllegalArgumentException If the max pool size is less than zero.
             */
            public SynchronizedPool(int maxPoolSize) : base(maxPoolSize) { }

            public override T acquire()
            {
                lock(mLock)
                {
                    return base.acquire();
                }
            }


            public override bool release(T element)
            {
                lock(mLock)
                {
                    return base.release(element);
                }
            }
        }

    }
}
