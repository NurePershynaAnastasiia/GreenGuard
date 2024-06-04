document.addEventListener('DOMContentLoaded', function() {
    const localizedText = {};
    const loginService = new LoginService('https://localhost:7042/api/Workers');

    const loadLanguage = async (lang) => {
        try {
            const response = await fetch(`../../public/locales/${lang}/${lang}.json`);
            const translations = await response.json();
            Object.assign(localizedText, translations);
            applyTranslations();
        } catch (error) {
            console.error('Error loading language file:', error);
        }
    };

    const applyTranslations = () => {
        document.querySelectorAll('[data-translate]').forEach(element => {
            const key = element.getAttribute('data-translate');
            if (localizedText[key]) {
                element.textContent = localizedText[key];
            }
        });
    };

    const languageSelect = document.getElementById('language-select');
    languageSelect.addEventListener('change', (event) => {
        const selectedLanguage = event.target.value;
        loadLanguage(selectedLanguage);
    });

    loadLanguage(languageSelect.value);

    document.getElementById('loginForm').addEventListener('submit', async function(event) {
        event.preventDefault();

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        if (!email || !password) {
            console.error('Error: Email and password fields cannot be empty');
            return;
        }

        try {
            const data = await loginService.login(email, password);
            if (data && data.token) {
                localStorage.setItem('token', data.token);

                const decodedToken = jwt_decode(data.token);
                localStorage.setItem('userData', JSON.stringify(decodedToken));
                window.location.href = './UserProfilePage.html';
            } else {
                console.error('Error: Token not found in response');
                document.getElementById('error-message').textContent = localizedText['error_message'] || 'Login failed. Token not found in response.';
            }
        } catch (error) {
            console.error('Error:', error.message);
            document.getElementById('error-message').textContent = localizedText['error_message'] || 'An unexpected error occurred. Please try again later.';
        }
    });
});
