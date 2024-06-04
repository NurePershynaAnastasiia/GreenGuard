document.addEventListener('DOMContentLoaded', () => {
    const taskService = new TaskService(
        'https://localhost:7042/api/Tasks',
        'https://localhost:7042/api/Fertilizers/fertilizers',
        'https://localhost:7042/api/Plants/plants',
        'https://localhost:7042/api/Workers/workers',
        localStorage.getItem('token')
    );

    const tasksTable = document.getElementById('tasks-table').querySelector('tbody');
    const newTaskForm = document.getElementById('new-task-form');
    const addButtonForm = document.getElementById('add-button-form');
    const fertilizersMap = {};
    let plantsList = [];
    let workersList = [];

    const fetchAndDisplayFertilizers = async () => {
        const fertilizers = await taskService.fetchFertilizers();
        const fertilizerSelect = document.getElementById('new-task-fertilizer');
        fertilizers.forEach(fertilizer => {
            fertilizersMap[fertilizer.fertilizerId] = fertilizer.fertilizerName;
            fertilizerSelect.innerHTML += `<option value="${fertilizer.fertilizerId}">${fertilizer.fertilizerName}</option>`;
        });
    };

    const fetchAndDisplayTasks = async () => {
        const tasks = await taskService.fetchTasks();
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
    };

    const fetchPlantsAndWorkers = async () => {
        plantsList = await taskService.fetchPlants();
        workersList = await taskService.fetchWorkers();
    };

    const addTask = async () => {
        const task = {
            taskDate: newTaskForm.querySelector('#new-task-date').value,
            taskType: newTaskForm.querySelector('#new-task-type').value,
            taskDetails: newTaskForm.querySelector('#new-task-details').value,
            taskState: newTaskForm.querySelector('#new-task-state').value,
            fertilizerId: newTaskForm.querySelector('#new-task-fertilizer').value
        };
        await taskService.addTask(task);
        await fetchAndDisplayTasks();
    };

    const deleteTask = async (taskId) => {
        await taskService.deleteTask(taskId);
        await fetchAndDisplayTasks();
    };

    const saveEditTask = async (taskId) => {
        const editForm = document.getElementById('edit-task-form');
        const task = {
            taskDate: editForm.querySelector('#edit-task-date').value,
            taskDetails: editForm.querySelector('#edit-task-details').value,
            taskType: editForm.querySelector('#edit-task-type').value,
            taskState: editForm.querySelector('#edit-task-state').value,
            fertilizerId: editForm.querySelector('#edit-task-fertilizer').value
        };
        await taskService.updateTask(taskId, task);
        cancelEditForm();
        await fetchAndDisplayTasks();
    };

    const showEditForm = (taskId, task) => {
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
    };

    const cancelEditForm = () => {
        const editFormRow = document.querySelector('.edit-form-row');
        if (editFormRow) {
            editFormRow.remove();
        }
    };

    const addWorkerToTask = async (taskId) => {
        const workerSelect = document.getElementById('edit-task-workers');
        const workerId = workerSelect.value;
        await taskService.addWorkerToTask(taskId, workerId);
        await fetchAndDisplayTasks();
    };

    const addPlantToTask = async (taskId) => {
        const plantSelect = document.getElementById('edit-task-plants');
        const plantId = plantSelect.value;
        await taskService.addPlantToTask(taskId, plantId);
        await fetchAndDisplayTasks();
    };

    const removeWorkerFromTask = async (taskId, workerId) => {
        await taskService.removeWorkerFromTask(taskId, workerId);
        await fetchAndDisplayTasks();
    };

    const removePlantFromTask = async (taskId, plantId) => {
        await taskService.removePlantFromTask(taskId, plantId);
        await fetchAndDisplayTasks();
    };

    addButtonForm.addEventListener('click', addTask);

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

    fetchAndDisplayFertilizers().then(() => {
        fetchPlantsAndWorkers().then(fetchAndDisplayTasks);
    });
});
