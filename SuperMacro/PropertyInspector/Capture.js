var textAreaName;
var isCapturing = false;


document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");
    showHideCapture();

    websocket.addEventListener('message', function (event) {
        console.log("Got message event!");
        var jsonObj = JSON.parse(event.data);

        if (jsonObj.event === 'sendToPropertyInspector') {
            showHideCapture();
        }
        else if (jsonObj.event === 'didReceiveSettings') {
            showHideCapture();
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
    document.body.addEventListener('keydown', handleKeypressCapture);
});


function captureKeystroke(textArea) {
    console.log("Start Capture into ", textArea);
    textAreaName = textArea;
    isCapturing = true;
    showHideCapture();
}

function cancelCapture() {
    console.log("Canceling capture");
    isCapturing = false;
    showHideCapture();
}

function showHideCapture() {
    if (!textAreaName) {
        return;
    }
    var btnCaptureKeystroke = document.getElementById(textAreaName + 'CaptureKeystroke');
    var btnCancelCapture = document.getElementById(textAreaName + 'CancelCapture');
    

    if (isCapturing) {
        btnCaptureKeystroke.style.display = "none";
        btnCancelCapture.style.display = "";
    }
    else {
        btnCaptureKeystroke.style.display = "";
        btnCancelCapture.style.display = "none";
    }
}

function handleKeypressCapture(event) {
    console.log('Handling keypress');
    if (!isCapturing) {
        return;
    }

    var textArea = document.getElementById(textAreaName);
    if (!textArea) {
        console.log('Invalid textarea ', textAreaName);
        return;
    }

    console.log(event);


    var keyStr = ["Control", "Shift", "Alt", "Meta"].includes(event.key) ? "" : event.key + " ";
    keyStr = keyStr.trim();

    // Special case if the event.key has a malformed value
    var keyValue = keyStr.charCodeAt(0);

    // If it's a malformed key, get the actual value from the 'code' property
    if (keyValue <= 26 && event.code.startsWith("Key")) {
        keyStr = event.code.charAt(event.code.length - 1);
    }
    // Borrowed from https://keyboardchecker.com/
    else if (event.key === "_") keyStr = "-";
    else if (event.key === "+") keyStr = "=";
    else if (event.key === "!") keyStr = "1";
    else if (event.key === "@") keyStr = "2";
    else if (event.key === "#") keyStr = "3";
    else if (event.key === "$") keyStr = "4";
    else if (event.key === "%") keyStr = "5";
    else if (event.key === "^") keyStr = "6";
    else if (event.key === "&") keyStr = "7";
    else if (event.key === "*") keyStr = "8";
    else if (event.key === "(") keyStr = "9";
    else if (event.key === ")") keyStr = "0";

    if (keyStr !== "") {
        let keystroke = '{';
        if (event.ctrlKey) {
            keystroke += '{CTRL}';
        }
        if (event.altKey) {
            keystroke += '{ALT}'
        }
        if (event.shiftKey) {
            keystroke += '{SHIFT}'
        }
        if (event.metaKey) {
            keystroke += '{WIN}'
        }

        keystroke += '{' + keyStr + '}';

        // Close keystroke
        keystroke += '}';
        console.log(keystroke);
        textArea.value += keystroke;
        isCapturing = false;
        showHideCapture();

        // Remove if outside of a plugin
        if (websocket) {
            setSettings();
        }
    }
}

