function setup() {
    window.onload = onWindowLoad;
}

function onWindowLoad() {
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

function copyToClipboard() {
    const tokenInput = document.querySelector('#tokenInput');
    navigator.clipboard.writeText(tokenInput.value || "");
}

function generateToken() {
    const scopes = [
        'chat:read',
        'chat:edit',
        'channel:read:polls',
        'channel:manage:polls'
    ];

    // https://id.twitch.tv/oauth2/authorize?client_id=d9rifiqcfbgz93ft16o8bsya9ho2ih&redirect_uri=https%3a%2f%2fchaos.lord.moe%2f&response_type=code&scope=chat_login+channel:manage:polls+channel:read:polls&state=&force_verify=False

    const url = new URL('https://id.twitch.tv/oauth2/authorize');
    const params = url.searchParams;

    params.set('response_type', 'token');
    params.set('client_id', 'd9rifiqcfbgz93ft16o8bsya9ho2ih');
    params.set('redirect_uri', 'https://chaos.lord.moe/');
    params.set('scope', scopes.join(' '));

    window.location = url.toString();
}

setup();