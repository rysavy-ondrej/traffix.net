using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

namespace Traffix.Core.Observable
{
    internal static class TimeWindow<TSource>
    {
        internal sealed class Window : IObservable<IObservable<TSource>>
        {
            private readonly IObservable<TSource> _source;
            private readonly Func<TSource, long> _getTicks;
            private readonly TimeSpan _timeSpan;

            public Window(IObservable<TSource> source, Func<TSource, long> getTicks, TimeSpan timeSpan)
            {
                this._source = source;
                this._getTicks = getTicks;
                this._timeSpan = timeSpan;
            }
            public IDisposable Subscribe(IObserver<IObservable<TSource>> observer)
            {
                if (observer == null)
                {
                    throw new ArgumentNullException(nameof(observer));
                }
                var sink = CreateSink(observer);
                Run(sink);
                return sink;
            }

            private _ CreateSink(IObserver<IObservable<TSource>> observer) => new _(this, observer);

            private void Run(_ sink) => sink.Run(_source);

            private sealed class _ : IObserver<TSource>, IDisposable
            {
                private readonly TimeWindow<TSource>.Window _parent;
                private volatile IObserver<IObservable<TSource>> _observer;

                long? _windowEdgeTicks = null;
                private Subject<TSource>? _currentWindow = null;
                
                
                private RefCountDisposable _refCountDisposable;

                public _(Window parent, IObserver<IObservable<TSource>> observer)
                {
                    this._parent = parent;
                    this._observer = observer;
                }

                public void Run(IObservable<TSource> source)
                {
                    var firstWindow = CreateWindow();
                    ForwardOnNext(firstWindow);

                    var disposable = source.SubscribeSafe(this);
                    _refCountDisposable = new RefCountDisposable(disposable);
                }

                private IObservable<TSource> CreateWindow()
                {
                    var subject = new Subject<TSource>();
                    _currentWindow = subject;
                    return subject;
                }

                public void ForwardOnNext(IObservable<TSource> value)
                {
                    _observer.OnNext(value);
                }

                public void OnNext(TSource value)
                {
                    var ticks = _parent._getTicks(value);
                    _windowEdgeTicks ??= ticks + _parent._timeSpan.Ticks;
                    if (ticks < _windowEdgeTicks)
                    {
                        _currentWindow?.OnNext(value);
                    }
                    else 
                    {
                        _currentWindow?.OnCompleted();

                        _windowEdgeTicks += _parent._timeSpan.Ticks;
                        var newWindow = CreateWindow();
                        ForwardOnNext(newWindow);
                    }
                }

                public void OnError(Exception error)
                {
                    _currentWindow?.OnError(error);
                    _observer.OnError(error);
                }

                public void OnCompleted()
                {
                    _observer.OnCompleted();
                }

                public void Dispose()
                {
                    // When calling dispose, we have to:
                    // 1. stop producing the values to the observer
                    // 2. unsubscribe us from the source observable
                    if (Interlocked.Exchange(ref _observer, NopObserver<IObservable<TSource>>.Instance) != NopObserver<IObservable<TSource>>.Instance)
                    {
                        _refCountDisposable.Dispose();
                    }
                }
            }
        }
        internal sealed class NopObserver<T> : IObserver<T>
        {
            public static readonly IObserver<T> Instance = new NopObserver<T>();

            public void OnCompleted() { }

            public void OnError(Exception error) { }

            public void OnNext(T value) { }
        }
    }
}