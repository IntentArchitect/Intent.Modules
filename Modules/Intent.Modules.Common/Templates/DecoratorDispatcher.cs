using Intent.SoftwareFactory.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
{
    public class DecoratorDispatcher<TDecorator> where TDecorator : ITemplateDecorator
    {
        private IEnumerable<TDecorator> _decorators;
        private Func<IEnumerable<TDecorator>> _loadDecorators;

        public DecoratorDispatcher(Func<IEnumerable<TDecorator>> loadDecorators)
        {
            _loadDecorators = loadDecorators;
        }

        public string Dispatch(Func<TDecorator, string> doIt)
        {
            return GetDecorators().Aggregate(doIt);
        }

        public TResult Dispatch<TResult>(Func<IEnumerable<TDecorator>, TResult> doIt)
        {
            return doIt(GetDecorators());
        }

        public void Dispatch(Action<TDecorator> doIt)
        {
            foreach (var decorator in GetDecorators())
            {
                doIt(decorator);
            }
        }

        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = _loadDecorators());
        }
    }
}
