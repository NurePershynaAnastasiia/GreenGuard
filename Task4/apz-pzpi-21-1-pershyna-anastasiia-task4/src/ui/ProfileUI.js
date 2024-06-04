class ProfileUI {
    constructor(profileService) {
        this.profileService = profileService;
        this.workerContainer = document.getElementById('worker-info');
        this.editButton = document.getElementById('edit-button');
        this.saveButton = document.getElementById('save-button');
        this.cancelButton = document.getElementById('cancel-button');
        this.localizedText = {};
        this.originalWorkerData = {};

        this.editButton.addEventListener('click', () => this.toggleEditMode(true));
        this.cancelButton.addEventListener('click', () => location.reload());
        this.saveButton.addEventListener('click', () => this.saveWorker());

        const languageSelect = document.getElementById('language-select');
        languageSelect.addEventListener('change', (event) => {
            const selectedLanguage = event.target.value;
            this.loadLanguage(selectedLanguage);
        });

        this.loadLanguage(languageSelect.value); // Load default language
    }

    async initialize() {
        const workerData = await this.profileService.fetchWorkerId();
        if (workerData) {
            this.originalWorkerData = workerData;
            this.displayWorkerInfo(workerData);
        } else {
            console.error('Error: Could not fetch worker ID');
        }
    }

    displayWorkerInfo(worker) {
        const position = worker.isAdmin ? 'admin' : 'worker';
        this.workerContainer.innerHTML = `
            <h2><input type="text" id="workerName" value="${worker.workerName}" readonly></h2>
            <p><strong>${this.localizedText['phone'] || 'Phone'}:</strong> <input type="text" id="phoneNumber" value="${worker.phoneNumber}" readonly></p>
            <p><strong>${this.localizedText['email'] || 'Email'}:</strong> <input type="text" id="email" value="${worker.email}" readonly></p>
            <p><strong>${this.localizedText['start_work_time'] || 'Start Work Time'}:</strong> <input type="text" id="startWorkTime" value="${worker.startWorkTime}" readonly></p>
            <p><strong>${this.localizedText['end_work_time'] || 'End Work Time'}:</strong> <input type="text" id="endWorkTime" value="${worker.endWorkTime}" readonly></p>
            <p><strong>${this.localizedText['position'] || 'Position'}:</strong> ${position}</p>
        `;
    }

    toggleEditMode(enable) {
        const inputs = this.workerContainer.querySelectorAll('input');
        inputs.forEach(input => input.readOnly = !enable);
        this.editButton.style.display = enable ? 'none' : 'block';
        this.saveButton.style.display = enable ? 'block' : 'none';
        this.cancelButton.style.display = enable ? 'block' : 'none';
    }

    async saveWorker() {
        const updatedWorker = {
            ...this.originalWorkerData,
            workerName: document.getElementById('workerName').value,
            phoneNumber: document.getElementById('phoneNumber').value,
            email: document.getElementById('email').value,
            startWorkTime: document.getElementById('startWorkTime').value,
            endWorkTime: document.getElementById('endWorkTime').value,
        };

        const success = await this.profileService.updateWorker(updatedWorker);
        if (success) {
            location.reload(); // Перезагружаем страницу после успешного сохранения
        }
    }

    async loadLanguage(lang) {
        const translations = await this.profileService.loadLanguage(lang);
        this.localizedText = translations;
        this.applyTranslations();
    }

    applyTranslations() {
        document.querySelectorAll('[data-translate]').forEach(element => {
            const key = element.getAttribute('data-translate');
            if (this.localizedText[key]) {
                element.textContent = this.localizedText[key];
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7042/api/Workers/workers/';
    const updateUrl = 'https://localhost:7042/api/Workers/update/';

    const profileService = new ProfileService(apiUrl, updateUrl);
    const profileUI = new ProfileUI(profileService);
    await profileUI.initialize();
});
