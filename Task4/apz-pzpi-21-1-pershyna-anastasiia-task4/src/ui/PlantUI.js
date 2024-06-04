class PlantUI {
    constructor(plantService) {
        this.plantService = plantService;
        this.plantsTable = document.getElementById('plants-table').getElementsByTagName('tbody')[0];
        this.plantTypeSelect = document.getElementById('input-plant-type');
        this.localizedText = {};
        this.plantTypes = [];
    }

    async initialize() {
        document.querySelector('.add-button').addEventListener('click', () => this.addPlant());
        document.getElementById('language-select').addEventListener('change', (event) => this.fetchTranslations(event.target.value));

        await this.fetchPlantTypes();
        await this.fetchPlants();
        await this.fetchTranslations('en'); // Default language
    }

    async fetchPlants() {
        try {
            const plants = await this.plantService.fetchPlants();
            this.populatePlantsTable(plants);
        } catch (error) {
            console.error('Error fetching plants:', error);
            alert(`Error fetching plants: ${error.message}`);
        }
    }

    async fetchPlantTypes() {
        try {
            this.plantTypes = await this.plantService.fetchPlantTypes();
            this.populatePlantTypeSelect(this.plantTypes);
        } catch (error) {
            console.error('Error fetching plant types:', error);
        }
    }

    populatePlantTypeSelect(plantTypes) {
        this.plantTypeSelect.innerHTML = ''; // Clear existing options
        plantTypes.forEach(type => {
            const option = document.createElement('option');
            option.value = type.plantTypeId;
            option.textContent = type.plantTypeName;
            this.plantTypeSelect.appendChild(option);
        });
    }

    populatePlantsTable(plants) {
        this.plantsTable.innerHTML = ''; // Clear existing rows
        plants.forEach(plant => this.addPlantRow(plant));
    }

    addPlantRow(plant) {
        const row = this.plantsTable.insertRow();
        row.dataset.id = plant.plantId;

        const typeCell = row.insertCell(0);
        const locationCell = row.insertCell(1);
        const tempCell = row.insertCell(2);
        const humidityCell = row.insertCell(3);
        const lightingCell = row.insertCell(4);
        const infoCell = row.insertCell(5);
        const stateCell = row.insertCell(6);
        const wateringCell = row.insertCell(7);
        const editCell = row.insertCell(8);
        const deleteCell = row.insertCell(9);

        typeCell.textContent = plant.plantTypeName;
        locationCell.textContent = plant.plantLocation;
        tempCell.textContent = plant.temp;
        humidityCell.textContent = plant.humidity;
        lightingCell.textContent = plant.light;
        infoCell.textContent = plant.additionalInfo;
        stateCell.textContent = plant.plantState;
        wateringCell.innerHTML = `<button class="watering-button">${this.localizedText['watering'] || 'Calculate Watering'}</button>`;

        editCell.innerHTML = `<button class="edit-button">${this.localizedText['edit'] || 'Edit'}</button>`;
        deleteCell.innerHTML = `<button class="delete-button">${this.localizedText['delete'] || 'Delete'}</button>`;

        wateringCell.querySelector('button').addEventListener('click', () => this.calculateWatering(plant.plantId));
        editCell.querySelector('button').addEventListener('click', () => this.editPlant(row, plant));
        deleteCell.querySelector('button').addEventListener('click', () => this.deletePlant(plant.plantId));
    }

    async calculateWatering(plantId) {
        try {
            const wateringData = await this.plantService.calculateWatering(plantId);
            const nextWateringDate = new Date(wateringData.date).toLocaleDateString('en-CA');
            alert(`Next watering date: ${nextWateringDate}`);
        } catch (error) {
            console.error('Error calculating watering:', error);
        }
    }

    editPlant(row, plant) {
        row.innerHTML = `
            <td>${plant.plantTypeName}</td>
            <td><input type="text" value="${plant.plantLocation}"></td>
            <td><input type="number" value="${plant.temp}"></td>
            <td><input type="number" value="${plant.humidity}"></td>
            <td><input type="number" value="${plant.light}"></td>
            <td><input type="text" value="${plant.additionalInfo}"></td>
            <td>${plant.plantState}</td>
            <td><button class="save-button">${this.localizedText['save'] || 'Save'}</button><button class="cancel-button">${this.localizedText['cancel'] || 'Cancel'}</button></td>
            <td></td>
        `;

        row.querySelector('.save-button').addEventListener('click', () => this.savePlant(row, plant.plantId));
        row.querySelector('.cancel-button').addEventListener('click', () => this.fetchPlants());
    }

    async savePlant(row, plantId) {
        const inputs = row.querySelectorAll('input');
        const updatedPlant = {
            plantLocation: inputs[0].value,
            temp: parseFloat(inputs[1].value),
            humidity: parseFloat(inputs[2].value),
            light: parseFloat(inputs[3].value),
            additionalInfo: inputs[4].value,
        };

        try {
            await this.plantService.updatePlant(plantId, updatedPlant);
            await this.fetchPlants();
        } catch (error) {
            console.error('Error saving plant:', error);
        }
    }

    async deletePlant(plantId) {
        try {
            await this.plantService.deletePlant(plantId);
            await this.fetchPlants();
        } catch (error) {
            console.error('Error deleting plant:', error);
        }
    }

    async addPlant() {
        const typeInput = document.getElementById('input-plant-type');
        const locationInput = document.getElementById('input-plant-location');
        const tempInput = document.getElementById('input-temp');
        const humidityInput = document.getElementById('input-humidity');
        const lightInput = document.getElementById('input-light');
        const infoInput = document.getElementById('input-info');

        const newPlant = {
            plantTypeId: parseInt(typeInput.value),
            plantLocation: locationInput.value,
            temp: parseFloat(tempInput.value),
            humidity: parseFloat(humidityInput.value),
            light: parseFloat(lightInput.value),
            additionalInfo: infoInput.value
        };

        try {
            await this.plantService.addPlant(newPlant);

            // Clear inputs after successful addition
            typeInput.value = '';
            locationInput.value = '';
            tempInput.value = '';
            humidityInput.value = '';
            lightInput.value = '';
            infoInput.value = '';

            await this.fetchPlants();
        } catch (error) {
            console.error('Error adding plant:', error);
        }
    }

    async fetchTranslations(lang) {
        try {
            const response = await fetch(`../../public/locales/${lang}/${lang}.json`);
            if (!response.ok) {
                throw new Error(`Error fetching translations: ${response.statusText}`);
            }

            const translations = await response.json();
            for (const key in translations) {
                this.localizedText[key] = translations[key];
                const elements = document.querySelectorAll(`[data-translate="${key}"]`);
                elements.forEach(el => el.textContent = translations[key]);
            }
        } catch (error) {
            console.error('Error fetching translations:', error);
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const plantService = new PlantService();
    const plantUI = new PlantUI(plantService);
    plantUI.initialize();
});
