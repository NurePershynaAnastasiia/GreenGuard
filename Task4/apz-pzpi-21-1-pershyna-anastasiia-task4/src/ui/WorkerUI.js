class WorkerUI {
    constructor(workerService) {
        this.workerService = workerService;
        this.workersTable = document.getElementById('workers-table').getElementsByTagName('tbody')[0];
        this.localizedText = {};

        document.querySelector('.register-button').addEventListener('click', () => this.registerWorker());
        document.getElementById('language-select').addEventListener('change', (event) => {
            this.loadTranslations(event.target.value);
        });
    }

    async initialize() {
        await this.fetchWorkers();
        await this.loadTranslations(document.getElementById('language-select').value);
    }

    async fetchWorkers() {
        try {
            const workers = await this.workerService.fetchWorkers();
            this.populateWorkersTable(workers);
        } catch (error) {
            console.error('Error fetching workers:', error);
            alert(`Error fetching workers: ${error.message}`);
        }
    }

    populateWorkersTable(workers) {
        this.workersTable.innerHTML = ''; 
        workers.forEach(worker => this.addWorkerRow(worker));
    }

    addWorkerRow(worker) {
        const row = this.workersTable.insertRow();
        row.dataset.id = worker.workerId;

        const nameCell = row.insertCell(0);
        const phoneCell = row.insertCell(1);
        const emailCell = row.insertCell(2);
        const startWorkTimeCell = row.insertCell(3);
        const endWorkTimeCell = row.insertCell(4);
        const positionCell = row.insertCell(5);
        const salaryCell = row.insertCell(6);
        const editCell = row.insertCell(7);
        const deleteCell = row.insertCell(8);

        nameCell.textContent = worker.workerName;
        phoneCell.textContent = worker.phoneNumber;
        emailCell.textContent = worker.email;
        startWorkTimeCell.textContent = worker.startWorkTime;
        endWorkTimeCell.textContent = worker.endWorkTime;
        positionCell.textContent = worker.isAdmin ? this.localizedText['admin'] || 'Admin' : this.localizedText['worker'] || 'Worker';
        salaryCell.innerHTML = `<button class="salary-button">${this.localizedText['view_salary'] || 'View Salary'}</button>`;

        editCell.innerHTML = `<button class="edit-button">${this.localizedText['edit'] || 'Edit'}</button>`;
        deleteCell.innerHTML = `<button class="delete-button">${this.localizedText['delete'] || 'Delete'}</button>`;

        salaryCell.querySelector('button').addEventListener('click', () => this.viewSalary(worker.workerId));
        editCell.querySelector('button').addEventListener('click', () => this.editWorker(row, worker));
        deleteCell.querySelector('button').addEventListener('click', () => this.deleteWorker(worker.workerId));
    }

    async viewSalary(workerId) {
        try {
            const salary = await this.workerService.fetchSalary(workerId);
            alert(`${this.localizedText['salary'] || 'Salary'}: ${salary}`);
        } catch (error) {
            console.error('Error fetching salary:', error);
        }
    }

    editWorker(row, worker) {
        row.innerHTML = `
            <td><input type="text" value="${worker.workerName}"></td>
            <td><input type="text" value="${worker.phoneNumber}"></td>
            <td><input type="email" value="${worker.email}"></td>
            <td><input type="time" value="${worker.startWorkTime}"></td>
            <td><input type="time" value="${worker.endWorkTime}"></td>
            <td>
                <select>
                    <option value="true" ${worker.isAdmin ? 'selected' : ''} data-translate="admin">${this.localizedText['admin'] || 'Admin'}</option>
                    <option value="false" ${!worker.isAdmin ? 'selected' : ''} data-translate="worker">${this.localizedText['worker'] || 'Worker'}</option>
                </select>
            </td>
            <td><input type="number" value="${worker.salary}"></td>
            <td><button class="save-button">${this.localizedText['save'] || 'Save'}</button><button class="cancel-button">${this.localizedText['cancel'] || 'Cancel'}</button></td>
            <td></td>
        `;

        row.querySelector('.save-button').addEventListener('click', () => this.saveWorker(row, worker.workerId));
        row.querySelector('.cancel-button').addEventListener('click', () => this.fetchWorkers());
    }

    async saveWorker(row, workerId) {
        const inputs = row.querySelectorAll('input, select');
        const updatedWorker = {
            workerId,
            workerName: inputs[0].value,
            phoneNumber: inputs[1].value,
            email: inputs[2].value,
            startWorkTime: inputs[3].value,
            endWorkTime: inputs[4].value,
            isAdmin: inputs[5].value === 'true',
            salary: inputs[6].value
        };

        try {
            await this.workerService.updateWorker(workerId, updatedWorker);
            await this.fetchWorkers();
        } catch (error) {
            console.error('Error saving worker:', error);
        }
    }

    async deleteWorker(workerId) {
        try {
            await this.workerService.deleteWorker(workerId);
            await this.fetchWorkers();
        } catch (error) {
            console.error('Error deleting worker:', error);
        }
    }

    async registerWorker() {
        const nameInput = document.getElementById('new-worker-name');
        const phoneInput = document.getElementById('new-worker-phone');
        const emailInput = document.getElementById('new-worker-email');
        const passwordInput = document.getElementById('new-worker-password');
        const isAdminInput = document.getElementById('new-worker-isAdmin');

        const newWorker = {
            workerName: nameInput.value,
            phoneNumber: phoneInput.value,
            email: emailInput.value,
            password: passwordInput.value,
            isAdmin: isAdminInput.value === 'true'
        };

        try {
            await this.workerService.registerWorker(newWorker);
            await this.fetchWorkers();
        } catch (error) {
            console.error('Error registering worker:', error);
        }
    }

    async loadTranslations(language) {
        try {
            const translations = await this.workerService.loadTranslations(language);
            this.localizedText = translations;
            this.applyTranslations();
        } catch (error) {
            console.error('Error loading translations:', error);
        }
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
    const apiUrl = 'https://localhost:7042/api/Workers';
    const registerUrl = 'https://localhost:7042/api/Workers/register';
    const salaryUrl = 'https://localhost:7042/api/Salary';

    const workerService = new WorkerService(apiUrl, registerUrl, salaryUrl);
    const workerUI = new WorkerUI(workerService);
    await workerUI.initialize();
});
