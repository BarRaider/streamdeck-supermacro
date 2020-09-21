function openHelp() {
    if (websocket && (websocket.readyState === 1)) {
        const json = {
            'event': 'openUrl',
            'payload': {
                'url': 'https://github.com/BarRaider/streamdeck-supermacro/blob/master/README.md'
            }
        };
        websocket.send(JSON.stringify(json));
    }
}