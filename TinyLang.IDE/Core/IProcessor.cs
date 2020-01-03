using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace TinyLang.IDE.Core
{
    public interface IProcessor<in TIn, out TOut> : IObservable<TOut>, IObserver<TIn>
    {
        TOut Process(TIn val);
    }

    public abstract class BaseProcessor<TIn, TOut> : IProcessor<TIn, TOut>
    {
        protected Subject<TOut> subject = new Subject<TOut>();

        public virtual void OnCompleted() 
        {
            subject.OnCompleted();
        }

        public virtual void OnError(Exception error) 
        {
            subject.OnError(error);
        }

        public void OnNext(TIn value)
        {
            try
            {
                subject.OnNext(Process(value));
            }
            catch (Exception ex) 
            {
                OnError(ex);
            }
        }

        public abstract TOut Process(TIn val);

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
