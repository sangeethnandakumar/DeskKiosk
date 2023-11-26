import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [forecasts, setForecasts] = useState();

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
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
        </table>;

    return (
        <div>
            <h1 id="tabelLabel">Sangee</h1>
            {/*<button onClick={window.shell.Execute("exit")}>*/}
            {/*    Click Me To Sent A Command To WinForms Shell*/}
            {/*</button>*/}
            <p>{JSON.stringify(window.shell)}</p>
            {contents}
        </div>
    );
    
    async function populateWeatherData() {
        console.log(window.shell);
        const response = await fetch('http://localhost:5000/WeatherForecast');
        const data = await response.json();
        setForecasts(data);
    }
}

export default App;