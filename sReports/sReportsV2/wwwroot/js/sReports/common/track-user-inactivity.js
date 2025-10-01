var debounceTimeout;
const debouncePeriod = 1000;
var logoutTimeout;
const logoutTime = 2 * 60 * 60 * 1000;

// Initialize the logout timer on page load
function initializeLogoutTimer() {
    document.addEventListener('mousemove', resetLogoutTimer);
    document.addEventListener('keypress', resetLogoutTimer);
    document.addEventListener('touchstart', resetLogoutTimer);
    document.addEventListener('visibilitychange', handleVisibilityChange);
    window.addEventListener('storage', syncLogoutAcrossTabs);

    resetLogoutTimer();
}

function handleVisibilityChange() {
    if (document.visibilityState === 'visible') {
        const lastActivity = parseInt(localStorage.getItem('lastActivity') || '0');
        const currentTime = new Date().getTime();
        if (currentTime - lastActivity < logoutTime) {
            resetLogoutTimer();
        }
    }
}

function resetLogoutTimer() {
    const currentTime = new Date().getTime();
    localStorage.setItem('lastActivity', currentTime);

    clearTimeout(logoutTimeout);
    logoutTimeout = setTimeout(trackUserInactivity, logoutTime);
}

function trackUserInactivity() {
    const currentTime = new Date().getTime();
    const lastActivity = parseInt(localStorage.getItem('lastActivity') || '0');
    const ellapsedTime = currentTime - lastActivity;

    if (ellapsedTime >= logoutTime) {
        logoutUser();
    } else {
        const newTimer = logoutTime - ellapsedTime;
        clearTimeout(logoutTimeout);
        logoutTimeout = setTimeout(trackUserInactivity, newTimer);
    }
}

function logoutUser() {
    localStorage.setItem('loggedOut', Date.now()); // Set flag to notify other tabs
    window.location.href = '/User/Logout';
}

function syncLogoutAcrossTabs(event) {
    if (event.key === 'loggedOut') {
        // Redirect to logout in all tabs when 'loggedOut' is detected
        window.location.href = `/User/Logout?returnUrl=${getReturnUrl()}`;
    }
}

function getReturnUrl() {
    const path = window.location.pathname;
    const search = window.location.search;
    return encodeURIComponent(path + search);
}

// Reset the logout timer on user activity events
$(document).on('click mousemove keypress', function () {
    clearTimeout(debounceTimeout);
    debounceTimeout = setTimeout(resetLogoutTimer, debouncePeriod);
});

// Update the last activity timestamp in local storage
function updateLastActivity() {
    localStorage.setItem('lastActivity', new Date().getTime());
}

// Attach event handlers to update the last activity timestamp
$(window).on('blur', function () {
    updateLastActivity();
});

// Clear the last activity timestamp on logout
function clearLastActivity() {
    localStorage.removeItem('lastActivity');
    clearTimeout(logoutTimeout);
}