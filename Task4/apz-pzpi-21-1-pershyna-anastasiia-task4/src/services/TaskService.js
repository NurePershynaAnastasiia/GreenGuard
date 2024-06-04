class TaskService {
    constructor(apiUrl, fertilizersUrl, plantsUrl, workersUrl, token) {
        this.apiUrl = apiUrl;
        this.fertilizersUrl = fertilizersUrl;
        this.plantsUrl = plantsUrl;
        this.workersUrl = workersUrl;
        this.headers = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        };
    }

    async fetchFertilizers() {
        try {
            const response = await fetch(this.fertilizersUrl, { headers: this.headers });
            if (!response.ok) throw new Error('Failed to fetch fertilizers');
            return response.json();
        } catch (error) {
            console.error('Error fetching fertilizers:', error);
            return [];
        }
    }

    async fetchPlants() {
        try {
            const response = await fetch(this.plantsUrl, { headers: this.headers });
            if (!response.ok) throw new Error('Failed to fetch plants');
            return response.json();
        } catch (error) {
            console.error('Error fetching plants:', error);
            return [];
        }
    }

    async fetchWorkers() {
        try {
            const response = await fetch(this.workersUrl, { headers: this.headers });
            if (!response.ok) throw new Error('Failed to fetch workers');
            return response.json();
        } catch (error) {
            console.error('Error fetching workers:', error);
            return [];
        }
    }

    async fetchTasks() {
        try {
            const response = await fetch(`${this.apiUrl}/tasks`, { headers: this.headers });
            if (!response.ok) throw new Error('Failed to fetch tasks');
            return response.json();
        } catch (error) {
            console.error('Error fetching tasks:', error);
            return [];
        }
    }

    async addTask(task) {
        try {
            const response = await fetch(`${this.apiUrl}/add`, {
                method: 'POST',
                headers: this.headers,
                body: JSON.stringify(task)
            });
            if (!response.ok) throw new Error('Failed to add task');
        } catch (error) {
            console.error('Error adding task:', error);
        }
    }

    async updateTask(taskId, task) {
        try {
            const response = await fetch(`${this.apiUrl}/update/${taskId}`, {
                method: 'PUT',
                headers: this.headers,
                body: JSON.stringify(task)
            });
            if (!response.ok) throw new Error('Failed to update task');
        } catch (error) {
            console.error('Error updating task:', error);
        }
    }

    async deleteTask(taskId) {
        try {
            const response = await fetch(`${this.apiUrl}/delete/${taskId}`, {
                method: 'DELETE',
                headers: this.headers
            });
            if (!response.ok) throw new Error('Failed to delete task');
        } catch (error) {
            console.error('Error deleting task:', error);
        }
    }

    async addWorkerToTask(taskId, workerId) {
        try {
            const response = await fetch(`${this.apiUrl}/add-workers/${taskId}`, {
                method: 'POST',
                headers: this.headers,
                body: JSON.stringify([workerId])
            });
            if (!response.ok) throw new Error('Failed to add worker to task');
        } catch (error) {
            console.error('Error adding worker to task:', error);
        }
    }

    async addPlantToTask(taskId, plantId) {
        try {
            const response = await fetch(`${this.apiUrl}/add-plants/${taskId}`, {
                method: 'POST',
                headers: this.headers,
                body: JSON.stringify([plantId])
            });
            if (!response.ok) throw new Error('Failed to add plant to task');
        } catch (error) {
            console.error('Error adding plant to task:', error);
        }
    }

    async removeWorkerFromTask(taskId, workerId) {
        try {
            const response = await fetch(`${this.apiUrl}/delete-worker/${taskId}/${workerId}`, {
                method: 'DELETE',
                headers: this.headers
            });
            if (!response.ok) throw new Error('Failed to remove worker from task');
        } catch (error) {
            console.error('Error removing worker from task:', error);
        }
    }

    async removePlantFromTask(taskId, plantId) {
        try {
            const response = await fetch(`${this.apiUrl}/delete-plant/${taskId}/${plantId}`, {
                method: 'DELETE',
                headers: this.headers
            });
            if (!response.ok) throw new Error('Failed to remove plant from task');
        } catch (error) {
            console.error('Error removing plant from task:', error);
        }
    }
}
