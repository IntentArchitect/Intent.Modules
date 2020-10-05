using System;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.React.Editor;

namespace Intent.Modules.Common.React
{
    public class Program
    {
        static void Main(string[] args)
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
                <p aria-live=""polite"">Current count: <strong>{props.count}</strong></p>
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
        return 
            <ReactFragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count: <strong>{this.props.count}</strong></p>

                <button type=""button""
                        className=""btn btn-primary btn-lg""
                        onClick={this.do}>
                    Increment
                </button>
            </ReactFragment>
        ;
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
            tokens.Fill();
            var list = tokens.GetTokens();
            for (var index = 0; index < list.Count; index++)
            {
                var token = list[index];
                Console.WriteLine(
                    $"{index}\t{token.Text}\t\t[{token.Type} - {javaLexer.RuleNames[Math.Min(Math.Max(token.Type - 1, 0), javaLexer.RuleNames.Length)]}]");
            }

            var _rewriter = new TokenStreamRewriter(tokens);
            var parser = new JavaScriptParser(tokens);
            parser.ErrorHandler = new SpecialErrorHandler();
            var listener = new JavaScriptFileFactoryListener();
            //parser.Interpreter.PredictionMode = PredictionMode.SLL; // Performance enhancement
            ParseTreeWalker.Default.Walk(listener, parser.program());
        }
    }

    internal class SpecialErrorHandler : DefaultErrorStrategy
    {
        public override void Recover(Parser recognizer, RecognitionException e)
        {
            var noVi = e as NoViableAltException;
            var expected = noVi?.GetExpectedTokens();
            base.Recover(recognizer, e);
        }
    }
}
