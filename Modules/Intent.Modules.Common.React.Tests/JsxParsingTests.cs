using Xunit;

namespace Intent.Modules.Common.React.Tests
{
    public class JsxParsingTests
    {
        [Fact]
        public void BasicHtmlTest()
        {
            Assert.Equal("return <div><p>My inner content</p></div>;", ParserFactory.GetReturnType(@"
export class FetchData extends Component {
    basicHtmlTest() {
        return <div><p>My inner content</p></div>;
    }   
}"));
        }

        [Fact]
        public void BasicHtmlJavascriptMixTest()
        {
            Assert.Equal("return <div><p>{this.props.myValue}</p></div>;", ParserFactory.GetReturnType(@"
export class FetchData extends Component {
    basicHtmlJavascriptMixTest() {
        return <div><p>{this.props.myValue}</p></div>;
    }
}"));
        }

        [Fact]
        public void BasicHtmlJavascriptMixTestWithBraces()
        {
            Assert.Equal(@"return (
            <div><p>{this.props.myValue}</p></div>
        );", ParserFactory.GetReturnType(@"
export class FetchData extends Component {
    basicHtmlJavascriptMixTestWithBraces() {
        return (
            <div><p>{this.props.myValue}</p></div>
        );
    }
}"));
        }

        [Fact]
        public void ComplexHtmlTest()
        {
            Assert.Equal(@"return (
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
        );", ParserFactory.GetReturnType(@"
export class FetchData extends Component {
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
}"));
        }



        [Fact]
        public void ComplexHtml2Test()
        {
            Assert.Equal(@"return (
            <React.Fragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count: <strong>{this.props.count}</strong></p>

                <button type=""button""
                        className=""btn btn-primary btn-lg""
                        onClick={() => {  this.props.increment(); }}>
                    Increment
                </button>
            </React.Fragment>
        );", ParserFactory.GetReturnType(@"
export class FetchData extends Component {
    static renderForecastsTable(forecasts) {
        return (
            <React.Fragment>
                <h1>Counter</h1>

                <p>This is a simple example of a React component.</p>

                <p aria-live=""polite"">Current count: <strong>{this.props.count}</strong></p>

                <button type=""button""
                        className=""btn btn-primary btn-lg""
                        onClick={() => {  this.props.increment(); }}>
                    Increment
                </button>
            </React.Fragment>
        );
    }
}"));
        }
    }
}