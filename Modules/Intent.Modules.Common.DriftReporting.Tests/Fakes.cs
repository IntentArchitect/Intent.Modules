using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Eventing;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.DriftReporting.Api;
using NSubstitute;

namespace Intent.Modules.Common.DriftReporting.Tests
{
    /// <summary>Records what is logged, one line per call, as the Software Factory's output would show it.</summary>
    internal sealed class RecordingTracing : ITracing
    {
        public List<string> Lines { get; } = new();
        public List<Exception> Failures { get; } = new();

        public void Debug(string message) => Lines.Add($"Debug: {message}");

        public void Failure(Exception exception)
        {
            Failures.Add(exception);
            Lines.Add(exception switch
            {
                ElementException e => $"ElementException on {e.ElementId}: {e.Message}",
                FriendlyException e => $"FriendlyException: {e.Message}",
                _ => $"{exception.GetType().Name}: {exception.Message}"
            });
        }

        public void Failure(string exceptionMessage) => Lines.Add($"Failure: {exceptionMessage}");
        public void Info(string message) => Lines.Add($"Info: {message}");
        public void Warning(string message) => Lines.Add($"Warning: {message}");
    }

    /// <summary>The Software Factory's event dispatcher, as it behaves: synchronous, and delivered only to existing subscribers.</summary>
    internal sealed class FakeEventDispatcher : IApplicationEventDispatcher
    {
        private readonly Dictionary<string, List<Action<object>>> _subscriptions = new();

        public void Subscribe<TEvent>(string eventIdentifier, Action<TEvent> handler)
        {
            if (!_subscriptions.TryGetValue(eventIdentifier, out var handlers))
            {
                _subscriptions[eventIdentifier] = handlers = new List<Action<object>>();
            }

            handlers.Add(o => handler((TEvent)o));
        }

        public void Subscribe(string eventIdentifier, Action<ApplicationEvent> handler) => Subscribe<ApplicationEvent>(eventIdentifier, handler);
        public void Subscribe<TEvent>(Action<TEvent> handler) => Subscribe(typeof(TEvent).FullName, handler);

        public void Publish<TEvent>(string eventIdentifier, TEvent payload)
        {
            foreach (var handler in _subscriptions.GetValueOrDefault(eventIdentifier)?.ToList() ?? new List<Action<object>>())
            {
                handler(payload);
            }
        }

        public void Publish<TEvent>(TEvent payload) => Publish(typeof(TEvent).FullName, payload);
        public void Publish(ApplicationEvent @event) => Publish(@event.EventIdentifier, @event);
        public void Publish(string eventIdentifier, IDictionary<string, string> @event) => Publish(new ApplicationEvent(eventIdentifier, @event));
        public void Publish(string eventIdentifier, Dictionary<string, string> @event) => Publish(new ApplicationEvent(eventIdentifier, @event));
    }

    internal static class Fakes
    {
        public static IApplication Application(string solutionRoot = "D:/Repos/Acme/intent", string outputRoot = "D:/Repos/Acme", string name = "Acme")
        {
            var application = Substitute.For<IApplication>();
            application.Name.Returns(name);
            application.OutputRootDirectory.Returns(outputRoot);
            application.EventDispatcher.Returns(new FakeEventDispatcher());
            var solution = Substitute.For<ISolutionConfig>();
            solution.SolutionRootLocation.Returns(solutionRoot);
            application.GetSolutionConfig().Returns(solution);
            var config = Substitute.For<IApplicationConfig>();
            config.Name.Returns(name);
            application.GetApplicationConfig().Returns(config);
            application.ChangeManager.Returns(Substitute.For<IChanges>());
            return application;
        }

        public static IElement Element(string id, string name)
        {
            var element = Substitute.For<IElement>();
            element.Id.Returns(id);
            element.Name.Returns(name);
            return element;
        }
    }

    /// <summary>A reporter whose checks are a delegate.</summary>
    internal sealed class FakeReporter : DriftReporterBase
    {
        private readonly Action<IDriftContext> _report;
        private readonly int _order;

        public FakeReporter(string id, int order, Action<IDriftContext> report)
        {
            Id = id;
            _order = order;
            _report = report;
        }

        public override string Id { get; }
        public override int Order => _order;
        public int Calls { get; private set; }

        protected override void Report(IDriftContext context)
        {
            Calls++;
            _report(context);
        }
    }
}
