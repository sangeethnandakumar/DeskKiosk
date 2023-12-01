import { useState } from 'react';
import './App.css';
import { HubConnectionBuilder } from "@microsoft/signalr";
import { useEffect } from 'react';

function App() {

    const [connection, setConnection] = useState(null);
    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl("http://127.0.0.1:5000/signalrhub")
            .withAutomaticReconnect()
            .build();
        setConnection(connect);
    }, []);

    useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(() => {
                    connection.on("NotificationListner", (message) => {
                        //console.log(message);
                        setEncodedText(message);
                    });
                })
                .catch((error) => console.log(error));
        }
    }, [connection]);

    const [text, setText] = useState('');
    const [encodedText, setEncodedText] = useState('');

    const OnTextChange = e => {
        setText(e.target.value);     
        connection.invoke("SendMessage", e.target.value);
    }

    return (
        <div><h1>Base64 Encoder / Decoder</h1>

            <textarea rows="5" cols="100" placeholder="Normal Text" value={text} onChange={OnTextChange}></textarea>
            <br />
            <br />
            <textarea rows="5" cols="100" disabled placeholder="Encoded Text" value={encodedText}></textarea>
            <br />
            <br />
            <button>DECODE</button>
            <button>ENCODE</button>
        </div>
    );

}

export default App;