function handlePasswordResetRequest() {
    const hash = window.location.hash.substring(1);
    const params = new URLSearchParams(hash);
    
    const accessToken = params.get('access_token');
    const type = params.get('type');
    console.log('Access Token:', accessToken);
    console.log('Type:', type);
    
    if (accessToken && type === 'recovery') {
        window.location.href = `/Login/EnterNewPassword?access_token=${encodeURIComponent(accessToken)}&type=${encodeURIComponent(type)}`;
    }
    else if (type === 'signup') {
        window.location.href = `/Login/SignUpConfirmSuccess`;
    }
    else {
        window.location.href = '/Login/Auth';
    }
}

// Initialize password reset handler on specific pages
document.addEventListener('DOMContentLoaded', function() {
    // Check if we're on the request handling page
    if (document.body.classList.contains('request-handling-page')) {
        handlePasswordResetRequest();
    }
});