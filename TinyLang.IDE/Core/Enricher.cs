using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.IDE.Core
{
    public interface IEnricher<T>
    {
        void Enrich(T value);
    }

    public abstract class Enricher<T, TEnrichment> : IEnricher<T>
    {
        protected abstract void Enrich(T value, TEnrichment enrichment);

        protected abstract TEnrichment SelectEnrichment(T value);

        public virtual void Enrich(T value) 
        {
            var enrichment = SelectEnrichment(value);
            if (enrichment != null) 
            {
                Enrich(value, enrichment);
            }
        }
    }
}
