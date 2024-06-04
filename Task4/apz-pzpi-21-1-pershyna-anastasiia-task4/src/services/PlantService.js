class PlantService {
    constructor() {
        this.apiUrl = 'https://localhost:7042/api/Plants';
        this.wateringUrl = 'https://localhost:7042/api/Watering';
        this.plantTypesUrl = 'https://localhost:7042/api/PlantTypes/plantTypes';
    }

    async fetchPlants() {
        const token = localStorage.getItem('token');
        const response = await fetch(`${this.apiUrl}/plants`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error fetching plants: ${response.statusText}`);
        }
        return await response.json();
    }

    async fetchPlantTypes() {
        const token = localStorage.getItem('token');
        const response = await fetch(this.plantTypesUrl, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error fetching plant types: ${response.statusText}`);
        }
        return await response.json();
    }

    async calculateWatering(plantId) {
        const token = localStorage.getItem('token');
        const response = await fetch(`${this.wateringUrl}/${plantId}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error calculating watering: ${response.statusText}`);
        }
        return await response.json();
    }

    async updatePlant(plantId, updatedPlant) {
        const token = localStorage.getItem('token');
        const response = await fetch(`${this.apiUrl}/update/${plantId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(updatedPlant)
        });
        if (!response.ok) {
            throw new Error(`Error saving plant: ${response.statusText}`);
        }
    }

    async deletePlant(plantId) {
        const token = localStorage.getItem('token');
        const response = await fetch(`${this.apiUrl}/delete/${plantId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (!response.ok) {
            throw new Error(`Error deleting plant: ${response.statusText}`);
        }
    }

    async addPlant(newPlant) {
        const token = localStorage.getItem('token');
        const response = await fetch(`${this.apiUrl}/add`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(newPlant)
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(`Error adding plant: ${errorData.message}`);
        }
    }
}