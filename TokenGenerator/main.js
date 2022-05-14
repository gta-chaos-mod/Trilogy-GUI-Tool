function setup() {
    window.onload = onWindowLoad;
}

function onWindowLoad() {
    const clientIDInput = document.querySelector('#clientIDInput');
    clientIDInput.value = localStorage.getItem('clientID') || '';

    clientIDChange();

    if (window.location.hash === '') return;

    try {
        const params = window.location.hash.substring(1);
        const sp = new URLSearchParams(params);

        if (!sp.has('access_token')) return;

        const tokenInput = document.querySelector('#tokenInput');
        tokenInput.value = sp.get('access_token');

        document.querySelector('#tokenCopyToClipboard').disabled = false;
    }
    catch { }
}

function clientIDChange() {
    const generateTokenButton = document.querySelector('#generateTokenButton');
    const clientIDInput = document.querySelector('#clientIDInput');

    generateTokenButton.disabled = clientIDInput.value === '';

    localStorage.setItem('clientID', clientIDInput.value);
}

function copyToClipboard() {
    const tokenInput = document.querySelector('#tokenInput');
    navigator.clipboard.writeText(tokenInput.value || '');
}

function onRedirectURLClick() {
    navigator.clipboard.writeText('https://chaos.lord.moe/');
}

function generateToken() {
    const scopes = [
        'chat:read',
        'chat:edit',
        'channel:read:polls',
        'channel:manage:polls'
    ];

    const clientIDInput = document.querySelector('#clientIDInput');

    const url = new URL('https://id.twitch.tv/oauth2/authorize');
    const params = url.searchParams;

    params.set('response_type', 'token');
    params.set('client_id', clientIDInput.value);
    params.set('redirect_uri', 'https://chaos.lord.moe/');
    params.set('scope', scopes.join(' '));

    window.location = url.toString();
}

setup();