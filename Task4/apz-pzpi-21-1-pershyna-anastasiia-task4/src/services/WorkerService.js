class WorkerService {
    constructor(apiUrl, registerUrl, salaryUrl) {
        this.apiUrl = apiUrl;
        this.registerUrl = registerUrl;
        this.salaryUrl = salaryUrl;
    }

    getToken() {
        return localStorage.getItem('token');
    }

    async fetchWorkers() {
        const response = await fetch(`${this.apiUrl}/workers`, {
            headers: {
                'Authorization': `Bearer ${this.getToken()}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error fetching workers: ${response.statusText}`);
        }
        return response.json();
    }

    async fetchSalary(workerId) {
        const response = await fetch(`${this.salaryUrl}/${workerId}`, {
            headers: {
                'Authorization': `Bearer ${this.getToken()}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error fetching salary: ${response.statusText}`);
        }
        return response.json();
    }

    async updateWorker(workerId, updatedWorker) {
        const response = await fetch(`${this.apiUrl}/update/${workerId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.getToken()}`
            },
            body: JSON.stringify(updatedWorker)
        });
        if (!response.ok) {
            throw new Error(`Error saving worker: ${response.statusText}`);
        }
    }

    async deleteWorker(workerId) {
        const response = await fetch(`${this.apiUrl}/delete/${workerId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${this.getToken()}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error deleting worker: ${response.statusText}`);
        }
    }

    async registerWorker(newWorker) {
        const response = await fetch(this.registerUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.getToken()}`
            },
            body: JSON.stringify(newWorker)
        });
        if (!response.ok) {
            throw new Error(`Error registering worker: ${response.statusText}`);
        }
    }

    async loadTranslations(lang) {
        const response = await fetch(`../../public/locales/${lang}/${lang}.json`);
        if (!response.ok) {
            throw new Error(`Error loading translations: ${response.statusText}`);
        }
        return response.json();
    }
}
