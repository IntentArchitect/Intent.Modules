using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.React.Weaving;
using Xunit;

namespace Intent.Modules.Common.React.Tests
{
    public class JavaScriptFileFactoryListener : JavaScriptParserBaseListener
    {
        public override void EnterHtmlElements(JavaScriptParser.HtmlElementsContext context)
        {
            base.EnterHtmlElements(context);
        }
    }

    public class TsxIdentifiesHtmlTests
    {
        [Fact]
        public void TestParsing()
        {
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(@"import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(forecasts) {
    return (
      <table className='table table-striped' aria-labelledby=""tabelLabel"">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <p aria-live=""polite"">Current count 
            <span>ppp</span>
        </p>
        <tbody>
          {forecasts.map(forecast =>
            <tr key={forecast.date}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

    render2() {
        return (
            <React.Fragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count:
                    <strong>{props.count}</strong>
                </p>

                <button type=""button""
                    className=""btn btn-primary btn-lg""
                    onClick={() => { props.increment(); }}>
                    Increment
              </button>
            </React.Fragment>
        );
    }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.forecasts);

    return (
      <div>
        <h1 id=""tabelLabel"" >Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
    const response = await fetch('weatherforecast');
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
  }
}")));
            var javaLexer = new JavaScriptLexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);
            var _rewriter = new TokenStreamRewriter(tokens);
            var parser = new JavaScriptParser(tokens);
            var listener = new JavaScriptFileFactoryListener();
            ParseTreeWalker.Default.Walk(listener, parser.program());
        }

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
