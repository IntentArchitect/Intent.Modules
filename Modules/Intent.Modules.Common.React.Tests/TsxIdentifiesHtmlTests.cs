using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.React.Editor;
using Intent.Modules.Common.React.Weaving;
using Xunit;

namespace Intent.Modules.Common.React.Tests
{
    public static class ParserFactory {
        public static string GetReturnType(string source)
        {
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(source)));
            var javaLexer = new JavaScriptLexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);
            tokens.Fill();
            var list = tokens.GetTokens();
            for (var index = 0; index < list.Count; index++)
            {
                var token = list[index];
                //Console.WriteLine($"{index}\t{token.Text}\t\t[{token.Type} - {javaLexer.RuleNames[Math.Min(Math.Max(token.Type - 1, 0), javaLexer.RuleNames.Length)]}]");
            }

            var _rewriter = new TokenStreamRewriter(tokens);
            var parser = new JavaScriptParser(tokens);
            var listener = new JavaScriptFileFactoryListener();
            //parser.Interpreter.PredictionMode = PredictionMode.SLL; // Performance enhancement
            ParseTreeWalker.Default.Walk(listener, parser.program());
            return listener.LastReturnString;
        }
    }

    public class TsxIdentifiesHtmlTests
    {
        [Fact]
        public void Test()
        {
            var merger = new TsxWeavingMerger();
            var result = merger.Merge(existingContent: @"
import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';

type CounterProps =
    CounterStore.CounterState &
    typeof CounterStore.actionCreators &
    RouteComponentProps<{}>;

class Counter extends React.PureComponent<CounterProps> {
    public render() {
        return (
            <React.Fragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count: <strong>{this.props.count}</strong></p>

                <button type=""button""
                    className=""btn btn-primary btn-lg""
                    onClick={() => { this.props.increment(); }}>
                    Increment
                </button>
            </React.Fragment>
        );
    }
};

export default connect(
    (state: ApplicationState) => state.counter,
    CounterStore.actionCreators
)(Counter);", outputContent: @"
import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';

type CounterProps =
    CounterStore.CounterState &
    typeof CounterStore.actionCreators &
    RouteComponentProps<{}>;

class Counter extends React.PureComponent<CounterProps> {
    public render() {
        return (
            <React.Fragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count: <strong>{this.props.count}</strong></p>

                <button type=""button""
                    className=""btn btn-primary btn-lg""
                    onClick={() => { this.props.increment(); }}>
                    Increment
                </button>
            </React.Fragment>
        );
    }
};

export default connect(
    (state: ApplicationState) => state.counter,
    CounterStore.actionCreators
)(Counter);");

            Assert.Equal(@"
import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';

type CounterProps =
    CounterStore.CounterState &
    typeof CounterStore.actionCreators &
    RouteComponentProps<{}>;

class Counter extends React.PureComponent<CounterProps> {
    public render() {
        return (
            <React.Fragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count: <strong>{this.props.count}</strong></p>

                <button type=""button""
                    className=""btn btn-primary btn-lg""
                    onClick={() => { this.props.increment(); }}>
                    Increment
                </button>
            </React.Fragment>
        );
    }
};

export default connect(
    (state: ApplicationState) => state.counter,
    CounterStore.actionCreators
)(Counter);", result);
        }
    }
}
