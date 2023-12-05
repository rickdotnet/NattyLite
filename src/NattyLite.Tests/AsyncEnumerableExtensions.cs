namespace NattyLite.Tests;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
    {
        return new AsyncEnumerable<T>(enumerable);
    }

    private class AsyncEnumerable<T>(IEnumerable<T> enumerable) : IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(enumerable.GetEnumerator());
        }

        private class AsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
        {
            public T Current => enumerator.Current;

            public ValueTask DisposeAsync()
            {
                enumerator.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(enumerator.MoveNext());
            }
        }
    }
}