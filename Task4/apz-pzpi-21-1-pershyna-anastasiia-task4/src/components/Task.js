document.addEventListener('DOMContentLoaded', () => {
    const tasksTable = document.getElementById('tasks-table').querySelector('tbody');
    const newTaskForm = document.getElementById('new-task-form');
    const addButtonForm = document.getElementById('add-button-form');
    const apiUrl = 'https://localhost:7042/api/Tasks';
    const fertilizersUrl = 'https://localhost:7042/api/Fertilizers/fertilizers';
    const plantsUrl = 'https://localhost:7042/api/Plants/plants';
    const workersUrl = 'https://localhost:7042/api/Workers/workers';
    const token = localStorage.getItem('token');

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };

    let fertilizersMap = {};
    let plantsList = [];
    let workersList = [];

    async function fetchFertilizers() {
        try {
            const response = await fetch(fertilizersUrl, { headers });
            if (!response.ok) throw new Error('Failed to fetch fertilizers');
            const fertilizers = await response.json();
            const fertilizerSelect = document.getElementById('new-task-fertilizer');
            fertilizers.forEach(fertilizer => {
                fertilizersMap[fertilizer.fertilizerId] = fertilizer.fertilizerName;
                fertilizerSelect.innerHTML += `<option value="${fertilizer.fertilizerId}">${fertilizer.fertilizerName}</option>`;
            });
        } catch (error) {
            console.error('Error fetching fertilizers:', error);
        }
    }

    // Fetch all plants and workers
    async function fetchPlantsAndWorkers() {
        try {
            const plantsResponse = await fetch(plantsUrl, { headers });
            const workersResponse = await fetch(workersUrl, { headers });
            if (!plantsResponse.ok) throw new Error('Failed to fetch plants');
            if (!workersResponse.ok) throw new Error('Failed to fetch workers');

            plantsList = await plantsResponse.json();
            workersList = await workersResponse.json();
        } catch (error) {
            console.error('Error fetching plants or workers:', error);
        }
    }

    // Fetch tasks and populate table
    async function fetchTasks() {
        try {
            const response = await fetch(`${apiUrl}/tasks`, { headers });
            if (!response.ok) throw new Error('Failed to fetch tasks');
            const tasks = await response.json();
            tasksTable.innerHTML = '';
            tasks.forEach(task => {
                const row = document.createElement('tr');
                const fertilizerName = task.fertilizerId ? fertilizersMap[task.fertilizerId] : 'N/A';
                row.innerHTML = `
                    <td>${new Date(task.taskDate).toLocaleString()}</td>
                    <td>${task.taskType}</td>
                    <td>${fertilizerName}</td>
                    <td>${task.taskDetails}</td>
                    <td>${task.taskState}</td>
                    <td>${task.workers.join(', ')}</td>
                    <td>${task.plants.join(', ')}</td>
                    <td><button class="edit-button" data-id="${task.taskId}">Edit</button></td>
                    <td><button class="delete-button" data-id="${task.taskId}">Delete</button></td>
                `;
                tasksTable.appendChild(row);
            });
        } catch (error) {
            console.error('Error fetching tasks:', error);
        }
    }

    // Add new task
    async function addTask(task) {
        try {
            const response = await fetch(`${apiUrl}/add`, {
                method: 'POST',
                headers,
                body: JSON.stringify(task)
            });
            if (response.ok) {
                fetchTasks();
            } else {
                console.error('Error adding task:', response.statusText);
            }
        } catch (error) {
            console.error('Error adding task:', error);
        }
    }

    // Update task
    async function updateTask(taskId, task) {
        try {
            task.taskDate = new Date(task.taskDate).toISOString().slice(0, -1);

            if (typeof task.fertilizerId === 'string') {
                task.fertilizerId = parseInt(task.fertilizerId, 10);
            }

            const response = await fetch(`${apiUrl}/update/${taskId}`, {
                method: 'PUT',
                headers,
                body: JSON.stringify(task)
            });

            if (response.ok) {
                fetchTasks();
            } else {
                const errorMessage = await response.text();
                console.error('Error updating task:', response.statusText, errorMessage);
            }
        } catch (error) {
            console.error('Error updating task:', error);
        }
    }

    // Function to save edited task
    async function saveEditTask(taskId) {
        const editForm = document.getElementById('edit-task-form');
        const task = {
            taskDate: editForm.querySelector('#edit-task-date').value,
            taskDetails: editForm.querySelector('#edit-task-details').value,
            taskType: editForm.querySelector('#edit-task-type').value,
            taskState: editForm.querySelector('#edit-task-state').value,
            fertilizerId: editForm.querySelector('#edit-task-fertilizer').value
        };

    console.log("task id: ", taskId);
    console.log(task);
    console.log(task.taskDate);
    console.log(task.taskType);
    console.log(task.taskDetails);
    console.log(task.taskState);
    console.log(task.fertilizerId);

    await updateTask(taskId, task);
    cancelEditForm();
    }

    // Delete task
    async function deleteTask(taskId) {
        try {
            const response = await fetch(`${apiUrl}/delete/${taskId}`, {
                method: 'DELETE',
                headers
            });
            if (response.ok) {
                fetchTasks();
            } else {
                console.error('Error deleting task:', response.statusText);
            }
        } catch (error) {
            console.error('Error deleting task:', error);
        }
    }

    // Function to show edit form
    function showEditForm(taskId, task) {
        const workersOptions = workersList.map(worker => `<option value="${worker.workerId}">${worker.workerName}</option>`).join('');
        const plantsOptions = plantsList.map(plant => `<option value="${plant.plantId}">${plant.plantTypeName} (${plant.plantLocation})</option>`).join('');
        const selectedWorkers = task.workers.map(worker => `<li>${worker} <button class="remove-worker" data-id="${worker}">Remove</button></li>`).join('');
        const selectedPlants = task.plants.map(plant => `<li>${plant} <button class="remove-plant" data-id="${plant}">Remove</button></li>`).join('');
        const editFormHtml = `
            <form id="edit-task-form">
                <label for="edit-task-date">Date:</label>
                <input id="edit-task-date" type="datetime-local" value="${new Date(task.taskDate).toISOString().slice(0, -1)}">
                <label for="edit-task-type">Type:</label>
                <input id="edit-task-type" type="text" value="${task.taskType}">
                <label for="edit-task-details">Details:</label>
                <input id="edit-task-details" type="text" value="${task.taskDetails}">
                <label for="edit-task-state">State:</label>
                <input id="edit-task-state" type="text" value="${task.taskState}">
                <label for="edit-task-fertilizer">Fertilizer:</label>
                <select id="edit-task-fertilizer">${document.getElementById('new-task-fertilizer').innerHTML}</select>
                <label for="edit-task-workers">Add Workers:</label>
                <select id="edit-task-workers">${workersOptions}</select>
                <button type="button" id="add-worker-button">Add Worker</button>
                <ul id="selected-workers">${selectedWorkers}</ul>
                <label for="edit-task-plants">Add Plants:</label>
                <select id="edit-task-plants">${plantsOptions}</select>
                <button type="button" id="add-plant-button">Add Plant</button>
                <ul id="selected-plants">${selectedPlants}</ul>
                <button type="button" id="save-edit-button">Save</button>
                <button type="button" id="cancel-edit-button">Cancel</button>
            </form>
        `;
        const taskRow = document.querySelector(`.edit-button[data-id="${taskId}"]`).closest('tr');
        if (taskRow) {
            taskRow.insertAdjacentHTML('afterend', `<tr class="edit-form-row"><td colspan="9">${editFormHtml}</td></tr>`);
            document.getElementById('save-edit-button').addEventListener('click', () => saveEditTask(taskId));
            document.getElementById('cancel-edit-button').addEventListener('click', cancelEditForm);
            document.getElementById('add-worker-button').addEventListener('click', () => addWorkerToTask(taskId));
            document.getElementById('add-plant-button').addEventListener('click', () => addPlantToTask(taskId));
            document.querySelectorAll('.remove-worker').forEach(button => button.addEventListener('click', () => removeWorkerFromTask(taskId, button.dataset.id)));
            document.querySelectorAll('.remove-plant').forEach(button => button.addEventListener('click', () => removePlantFromTask(taskId, button.dataset.id)));
        } else {
            console.error(`Task row not found for task ID: ${taskId}`);
        }
    }

    // Function to cancel edit form
    function cancelEditForm() {
        const editFormRow = document.querySelector('.edit-form-row');
        if (editFormRow) {
            editFormRow.remove();
        }
    }

    // Function to add worker to task
    async function addWorkerToTask(taskId) {
        const workerSelect = document.getElementById('edit-task-workers');
        const workerId = workerSelect.value;
        try {
            const response = await fetch(`${apiUrl}/add-workers/${taskId}`, {
                method: 'POST',
                headers,
                body: JSON.stringify([workerId]) // Отправляем массив
            });
            if (response.ok) {
                fetchTasks();
            } else {
                console.error('Error adding worker to task:', response.statusText);
            }
        } catch (error) {
            console.error('Error adding worker to task:', error);
        }
    }

    // Function to add plant to task
    async function addPlantToTask(taskId) {
        const plantSelect = document.getElementById('edit-task-plants');
        const plantId = plantSelect.value;
        try {
            const response = await fetch(`${apiUrl}/add-plants/${taskId}`, {
                method: 'POST',
                headers,
                body: JSON.stringify([plantId])
            });
            if (response.ok) {
                fetchTasks();
            } else {
                console.error('Error adding plant to task:', response.statusText);
            }
        } catch (error) {
            console.error('Error adding plant to task:', error);
        }
    }

    // Function to remove worker from task
    async function removeWorkerFromTask(taskId, workerId) {
        try {
            const response = await fetch(`${apiUrl}/delete-worker/${taskId}/${workerId}`, {
                method: 'DELETE',
                headers
            });
            if (response.ok) {
                fetchTasks();
            } else {
                console.error('Error removing worker from task:', response.statusText);
            }
        } catch (error) {
            console.error('Error removing worker from task:', error);
        }
    }

    // Function to remove plant from task
    async function removePlantFromTask(taskId, plantId) {
        try {
            const response = await fetch(`${apiUrl}/delete-plant/${taskId}/${plantId}`, {
                method: 'DELETE',
                headers
            });
            if (response.ok) {
                fetchTasks();
            } else {
                console.error('Error removing plant from task:', response.statusText);
            }
        } catch (error) {
            console.error('Error removing plant from task:', error);
        }
    }

    // Add event listeners
    addButtonForm.addEventListener('click', () => {
        const task = {
            taskDate: newTaskForm.querySelector('#new-task-date').value,
            taskType: newTaskForm.querySelector('#new-task-type').value,
            taskDetails: newTaskForm.querySelector('#new-task-details').value,
            taskState: newTaskForm.querySelector('#new-task-state').value,
            fertilizerId: newTaskForm.querySelector('#new-task-fertilizer').value
        };
        addTask(task);
    });

    tasksTable.addEventListener('click', (event) => {
        if (event.target.classList.contains('edit-button')) {
            const taskId = event.target.dataset.id;
            const taskRow = event.target.closest('tr');
            if (taskRow) {
                const task = {
                    taskDate: taskRow.children[0].textContent,
                    taskType: taskRow.children[1].textContent,
                    fertilizerId: taskRow.children[2].textContent === 'N/A' ? null : taskRow.children[2].textContent,
                    taskDetails: taskRow.children[3].textContent,
                    taskState: taskRow.children[4].textContent,
                    workers: taskRow.children[5].textContent.split(', '),
                    plants: taskRow.children[6].textContent.split(', ')
                };
                showEditForm(taskId, task);
            } else {
                console.error(`Task row not found for task ID: ${taskId}`);
            }
        } else if (event.target.classList.contains('delete-button')) {
            const taskId = event.target.dataset.id;
            deleteTask(taskId);
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
    
    // Initialize
    fetchFertilizers().then(() => {
        fetchPlantsAndWorkers().then(fetchTasks);
    });
});