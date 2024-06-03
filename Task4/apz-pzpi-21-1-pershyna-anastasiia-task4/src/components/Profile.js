document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'http://localhost:5159/api/Workers/workers/';
    const updateUrl = 'http://localhost:5159/api/Workers/update/';
    const workerContainer = document.getElementById('worker-info');
    const editButton = document.getElementById('edit-button');
    const saveButton = document.getElementById('save-button');
    const cancelButton = document.getElementById('cancel-button');
    const localizedText = {};
    let originalWorkerData = {};

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        try {
            const userData = JSON.parse(localStorage.getItem('userData'));
            return userData?.nameid ? userData : null;
        } catch (error) {
            console.error('Error parsing user data:', error);
            return null;
        }
    };

    const fetchData = async (url, headers) => {
        try {
            const response = await fetch(url, { headers });
            if (response.ok) return response.json();
            console.error('Error:', await response.text());
        } catch (error) {
            console.error('Error:', error.message);
        }
        return null;
    };

    const fetchWorkerId = async () => {
        const userData = getUserData();
        if (!userData) {
            console.error('Error: User data not found in localStorage');
            return null;
        }

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        const url = `${apiUrl}${userData.nameid}`;
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        return await fetchData(url, headers);
    };

    const fetchWorkerInfo = async (workerData) => {
        if (workerData) {
            originalWorkerData = workerData; 
            displayWorkerInfo(workerData);
        }
    };

    const displayWorkerInfo = (worker) => {
        const position = worker.isAdmin ? 'admin' : 'worker';
        workerContainer.innerHTML = `
            <h2><input type="text" id="workerName" value="${worker.workerName}" readonly></h2>
            <p><strong>${localizedText['phone'] || 'Phone'}:</strong> <input type="text" id="phoneNumber" value="${worker.phoneNumber}" readonly></p>
            <p><strong>${localizedText['email'] || 'Email'}:</strong> <input type="text" id="email" value="${worker.email}" readonly></p>
            <p><strong>${localizedText['start_work_time'] || 'Start Work Time'}:</strong> <input type="text" id="startWorkTime" value="${worker.startWorkTime}" readonly></p>
            <p><strong>${localizedText['end_work_time'] || 'End Work Time'}:</strong> <input type="text" id="endWorkTime" value="${worker.endWorkTime}" readonly></p>
            <p><strong>${localizedText['position'] || 'Position'}:</strong> ${position}</p>
        `;
    };

    const toggleEditMode = (enable) => {
        const inputs = workerContainer.querySelectorAll('input');
        inputs.forEach(input => input.readOnly = !enable);
        editButton.style.display = enable ? 'none' : 'block';
        saveButton.style.display = enable ? 'block' : 'none';
        cancelButton.style.display = enable ? 'block' : 'none';
    };

    editButton.addEventListener('click', () => toggleEditMode(true));

    cancelButton.addEventListener('click', () => location.reload());

    saveButton.addEventListener('click', async () => {
        const updatedWorker = {
            ...originalWorkerData,
            workerName: document.getElementById('workerName').value,
            phoneNumber: document.getElementById('phoneNumber').value,
            email: document.getElementById('email').value,
            startWorkTime: document.getElementById('startWorkTime').value,
            endWorkTime: document.getElementById('endWorkTime').value,
        };

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return;
        }

        const url = `${updateUrl}${updatedWorker.workerId}`;
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        try {
            const response = await fetch(url, {
                method: 'PUT',
                headers,
                body: JSON.stringify(updatedWorker)
            });

            if (response.ok) {
                location.reload(); // Перезагружаем страницу после успешного сохранения
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    });

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

    // Load default language
    loadLanguage(languageSelect.value);

    const workerData = await fetchWorkerId();
    if (workerData) {
        await fetchWorkerInfo(workerData);
    } else {
        console.error('Error: Could not fetch worker ID');
    }
});
