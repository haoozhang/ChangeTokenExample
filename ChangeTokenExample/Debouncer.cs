namespace ChangeTokenExample
{
    public class Debouncer : IDisposable
    {
        private readonly CancellationTokenSource cts;
        private readonly TimeSpan waitTime;
        private int counter;

        public Debouncer(TimeSpan? waitTime = null)
        {
            cts = new CancellationTokenSource();
            this.waitTime = waitTime ?? TimeSpan.FromSeconds(1);
        }

        public void Debouce(Action action)
        {
            var current = Interlocked.Increment(ref counter);

            Task.Delay(waitTime).ContinueWith(task =>
            {
                // Is this the last task that was queued?
                if (current == counter && !cts.IsCancellationRequested)
                    action();
                task.Dispose();
            }, cts.Token);
        }

        public void Dispose()
        {
            cts.Cancel();
        }
    }
}
