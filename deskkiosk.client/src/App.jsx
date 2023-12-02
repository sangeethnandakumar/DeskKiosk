// App.js

import { useState, useEffect } from 'react';
import latch from './latch';

function App() {
    const [text, setText] = useState('');
    const [encodedText, setEncodedText] = useState('');

    useEffect(() => {
        latch.on("AppPage", "OnTextChange", msg => setEncodedText(msg));
        latch.on("AppPage", "OnAlert", msg => alert(msg));
    }, []);

    const onTextChange = (e) => {
        setText(e.target.value);
        latch.invoke("AppPage", "OnTextChange", e.target.value);
    }

    return (
        <div>
            <h1>Base64 Encoder / Decoder</h1>
            <textarea rows="5" cols="100" placeholder="Normal Text" value={text} onChange={onTextChange}></textarea>
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
