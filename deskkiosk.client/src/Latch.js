// latch.js controls incomming and outgoing communications using SignalR as backbone

import { HubConnectionBuilder } from "@microsoft/signalr";

class Latch {
    constructor() {
        this.connection = null;
        this.initializeConnection();
    }

    initializeConnection() {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://127.0.0.1:5000/signalrhub")
            .withAutomaticReconnect()
            .build();

        this.connection.start().catch((error) => console.log(error));
    }

    on(component, methodName, callback) {
        this.connection.on(`${component}_${methodName}`, callback);
    }

    invoke(component, methodName, ...args) {
        this.connection.invoke(`${component}_${methodName}`, ...args);
    }
}

const latch = new Latch();

export default latch;
