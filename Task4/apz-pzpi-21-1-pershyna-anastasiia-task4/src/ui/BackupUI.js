document.addEventListener("DOMContentLoaded", function () {
    const backupService = new BackupService('https://localhost:7042/api/Backups');
    const backupTableBody = document.querySelector("#backups-table tbody");
    const createBackupBtn = document.querySelector("#make-backup-button");
    const localizedText = {};

    async function fetchBackups() {
        try {
            const token = localStorage.getItem('token');
            const backups = await backupService.fetchBackups(token);
            populateBackupTable(backups);
        } catch (error) {
            console.error('Error fetching backups:', error);
            alert(`Error fetching backups: ${error.message}`);
        }
    }

    function populateBackupTable(backups) {
        backupTableBody.innerHTML = '';
        backups.forEach(backup => {
            const row = document.createElement('tr');

            const nameCell = document.createElement('td');
            nameCell.textContent = backup;

            const restoreCell = document.createElement('td');
            const restoreButton = document.createElement('button');
            restoreButton.textContent = localizedText['restore'] || 'Restore';
            restoreButton.classList.add('restore-button');
            restoreButton.addEventListener('click', () => restoreBackup(backup));
            restoreCell.appendChild(restoreButton);

            row.appendChild(nameCell);
            row.appendChild(restoreCell);

            backupTableBody.appendChild(row);
        });
    }

    async function createBackup() {
        try {
            const token = localStorage.getItem('token');
            await backupService.createBackup(token);
            alert('Backup created successfully');
            fetchBackups();
        } catch (error) {
            console.error('Error creating backup:', error);
            alert(`Error creating backup: ${error.message}`);
        }
    }

    async function restoreBackup(backupFileName) {
        try {
            const token = localStorage.getItem('token');
            await backupService.restoreBackup(token, backupFileName);
            alert('Database restored successfully');
        } catch (error) {
            console.error('Error restoring database:', error);
            alert(`Error restoring database: ${error.message}`);
        }
    }

    createBackupBtn.addEventListener('click', createBackup);

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

    // Fetch and display backups on page load
    fetchBackups();
});
