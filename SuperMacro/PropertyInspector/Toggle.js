document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");
    showHideSettings(actionInfo.payload.settings);

    websocket.addEventListener('message', function (event) {
        console.log("Got message event!");

        // Received message from Stream Deck
        var jsonObj = JSON.parse(event.data);

        if (jsonObj.event === 'sendToPropertyInspector') {
            var payload = jsonObj.payload;
            showHideSettings(payload);
        }
        else if (jsonObj.event === 'didReceiveSettings') {
            var payload = jsonObj.payload;
            showHideSettings(payload.settings);
        }
    });
});

function showHideSettings(payload) {
    console.log("Show Hide Settings Called");
    setStateSettings("none");
    setTextAreas("");
    setFileNames("none");
    if (payload['loadFromFiles']) {
        setTextAreas("none");
        setFileNames("");
    }
    if (payload['rememberState']) {
        setStateSettings("");
    }
}

function setStateSettings(displayValue) {
    var dvStateSettings = document.getElementById('dvStateSettings');
    dvStateSettings.style.display = displayValue;
}

function setTextAreas(displayValue) {
    var dvTextAreas = document.getElementById('dvTextAreas');
    dvTextAreas.style.display = displayValue;
}

function setFileNames(displayValue) {
    var dvFileNames = document.getElementById('dvFileNames');
    dvFileNames.style.display = displayValue;
}

function resetState() {
    console.log("Reset state...");
    var payload = {};
    payload.property_inspector = 'resetState';
    sendPayloadToPlugin(payload);
}

function resetAllStates() {
    console.log("Reset all states...");
    var payload = {};
    payload.property_inspector = 'resetAllStates';
    sendPayloadToPlugin(payload);
}