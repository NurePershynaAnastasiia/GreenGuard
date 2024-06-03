document.addEventListener('DOMContentLoaded', async () => {
    const workersTable = document.getElementById('workers-table').getElementsByTagName('tbody')[0];
    const apiUrl = 'http://localhost:5159/api/Workers';
    const registerUrl = 'http://localhost:5159/api/Workers/register';
    const salaryUrl = 'http://localhost:5159/api/Salary';
    const localizedText = {};

    async function fetchWorkers() {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/workers`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error fetching workers: ${response.statusText}`);
            }

            const workers = await response.json();
            populateWorkersTable(workers);
        } catch (error) {
            console.error('Error fetching workers:', error);
            alert(`Error fetching workers: ${error.message}`);
        }
    }

    function populateWorkersTable(workers) {
        workersTable.innerHTML = ''; 
        workers.forEach(worker => addWorkerRow(worker));
    }

    function addWorkerRow(worker) {
        const row = workersTable.insertRow();
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
        positionCell.textContent = worker.isAdmin ? localizedText['admin'] || 'Admin' : localizedText['worker'] || 'Worker';
        salaryCell.innerHTML = `<button class="salary-button">${localizedText['view_salary'] || 'View Salary'}</button>`;

        editCell.innerHTML = `<button class="edit-button">${localizedText['edit'] || 'Edit'}</button>`;
        deleteCell.innerHTML = `<button class="delete-button">${localizedText['delete'] || 'Delete'}</button>`;

        salaryCell.querySelector('button').addEventListener('click', () => viewSalary(worker.workerId));
        editCell.querySelector('button').addEventListener('click', () => editWorker(row, worker));
        deleteCell.querySelector('button').addEventListener('click', () => deleteWorker(worker.workerId));
    }

    async function viewSalary(workerId) {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${salaryUrl}/${workerId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error fetching salary: ${response.statusText}`);
            }

            const salary = await response.json();
            alert(`${localizedText['salary'] || 'Salary'}: ${salary}`);
        } catch (error) {
            console.error('Error fetching salary:', error);
        }
    }

    function editWorker(row, worker) {
        row.innerHTML = `
            <td><input type="text" value="${worker.workerName}"></td>
            <td><input type="text" value="${worker.phoneNumber}"></td>
            <td><input type="email" value="${worker.email}"></td>
            <td><input type="time" value="${worker.startWorkTime}"></td>
            <td><input type="time" value="${worker.endWorkTime}"></td>
            <td>
                <select>
                    <option value="true" ${worker.isAdmin ? 'selected' : ''} data-translate="admin">${localizedText['admin'] || 'Admin'}</option>
                    <option value="false" ${!worker.isAdmin ? 'selected' : ''} data-translate="worker">${localizedText['worker'] || 'Worker'}</option>
                </select>
            </td>
            <td><input type="number" value="${worker.salary}"></td>
            <td><button class="save-button">${localizedText['save'] || 'Save'}</button><button class="cancel-button">${localizedText['cancel'] || 'Cancel'}</button></td>
            <td></td>
        `;

        row.querySelector('.save-button').addEventListener('click', () => saveWorker(row, worker.workerId));
        row.querySelector('.cancel-button').addEventListener('click', () => fetchWorkers());
    }

    async function saveWorker(row, workerId) {
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
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/update/${workerId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(updatedWorker)
            });

            if (!response.ok) {
                throw new Error(`Error saving worker: ${response.statusText}`);
            }

            await fetchWorkers();
        } catch (error) {
            console.error('Error saving worker:', error);
        }
    }

    async function deleteWorker(workerId) {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/delete/${workerId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error deleting worker: ${response.statusText}`);
            }

            await fetchWorkers();
        } catch (error) {
            console.error('Error deleting worker:', error);
        }
    }

    async function registerWorker() {
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
            const token = localStorage.getItem('token');
            const response = await fetch(registerUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(newWorker)
            });

            if (!response.ok) {
                throw new Error(`Error registering worker: ${response.statusText}`);
            }

            await fetchWorkers();
        } catch (error) {
            console.error('Error registering worker:', error);
        }
    }

    async function loadTranslations(language) {
        const response = await fetch(`./translations/${language}.json`);
        if (response.ok) {
            Object.assign(localizedText, await response.json());
            applyTranslations();
        }
    }

    function applyTranslations() {
        document.querySelectorAll('[data-translate]').forEach(element => {
            const key = element.getAttribute('data-translate');
            if (localizedText[key]) {
                element.textContent = localizedText[key];
            }
        });
    }

    document.querySelector('.register-button').addEventListener('click', registerWorker);
    document.getElementById('language-select').addEventListener('change', (event) => {
        loadTranslations(event.target.value);
    });

    fetchWorkers();
    loadTranslations(document.getElementById('language-select').value); // Load initial language
});
