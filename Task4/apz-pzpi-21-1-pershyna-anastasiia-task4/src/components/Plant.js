document.addEventListener('DOMContentLoaded', () => {
    const plantsTable = document.getElementById('plants-table').getElementsByTagName('tbody')[0];
    const apiUrl = 'http://localhost:5159/api/Plants';
    const wateringUrl = 'http://localhost:5159/api/Watering';
    const plantTypesUrl = 'http://localhost:5159/api/PlantTypes/plantTypes';
    const plantTypeSelect = document.getElementById('input-plant-type');
    const localizedText = {};
    let plantTypes = [];

    async function fetchPlants() {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/plants`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error fetching plants: ${response.statusText}`);
            }

            const plants = await response.json();
            populatePlantsTable(plants);
        } catch (error) {
            console.error('Error fetching plants:', error);
            alert(`Error fetching plants: ${error.message}`);
        }
    }

    async function fetchPlantTypes() {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(plantTypesUrl, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error fetching plant types: ${response.statusText}`);
            }

            plantTypes = await response.json();
            populatePlantTypeSelect(plantTypes);
        } catch (error) {
            console.error('Error fetching plant types:', error);
        }
    }

    function populatePlantTypeSelect(plantTypes) {
        plantTypeSelect.innerHTML = ''; // Clear existing options
        plantTypes.forEach(type => {
            const option = document.createElement('option');
            option.value = type.plantTypeId;
            option.textContent = type.plantTypeName;
            plantTypeSelect.appendChild(option);
        });
    }

    function populatePlantsTable(plants) {
        plantsTable.innerHTML = ''; // Clear existing rows
        plants.forEach(plant => addPlantRow(plant));
    }

    function addPlantRow(plant) {
        const row = plantsTable.insertRow();
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
        wateringCell.innerHTML = `<button class="watering-button">${localizedText['watering'] || 'Calculate Watering'}</button>`;

        editCell.innerHTML = `<button class="edit-button">${localizedText['edit'] || 'Edit'}</button>`;
        deleteCell.innerHTML = `<button class="delete-button">${localizedText['delete'] || 'Delete'}</button>`;

        wateringCell.querySelector('button').addEventListener('click', () => calculateWatering(plant.plantId));
        editCell.querySelector('button').addEventListener('click', () => editPlant(row, plant));
        deleteCell.querySelector('button').addEventListener('click', () => deletePlant(plant.plantId));
    }

    async function calculateWatering(plantId) {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${wateringUrl}/${plantId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error calculating watering: ${response.statusText}`);
            }

            const wateringData = await response.json();
            const nextWateringDate = new Date(wateringData.date).toLocaleDateString('en-CA');
            alert(`Next watering date: ${nextWateringDate}`);
        } catch (error) {
            console.error('Error calculating watering:', error);
        }
    }

    function editPlant(row, plant) {
        row.innerHTML = `
            <td>${plant.plantTypeName}</td>
            <td><input type="text" value="${plant.plantLocation}"></td>
            <td><input type="number" value="${plant.temp}"></td>
            <td><input type="number" value="${plant.humidity}"></td>
            <td><input type="number" value="${plant.light}"></td>
            <td><input type="text" value="${plant.additionalInfo}"></td>
            <td>${plant.plantState}</td>
            <td><button class="save-button">${localizedText['save'] || 'Save'}</button><button class="cancel-button">${localizedText['cancel'] || 'Cancel'}</button></td>
            <td></td>
        `;

        row.querySelector('.save-button').addEventListener('click', () => savePlant(row, plant.plantId));
        row.querySelector('.cancel-button').addEventListener('click', () => fetchPlants());
    }

    async function savePlant(row, plantId) {
        const inputs = row.querySelectorAll('input');
        const updatedPlant = {
            plantLocation: inputs[0].value,
            temp: parseFloat(inputs[1].value),
            humidity: parseFloat(inputs[2].value),
            light: parseFloat(inputs[3].value),
            additionalInfo: inputs[4].value,
        };

        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/update/${plantId}`, {
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

            await fetchPlants();
        } catch (error) {
            console.error('Error saving plant:', error);
        }
    }

    async function deletePlant(plantId) {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/delete/${plantId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error deleting plant: ${response.statusText}`);
            }

            await fetchPlants();
        } catch (error) {
            console.error('Error deleting plant:', error);
        }
    }

    document.querySelector('.add-button').addEventListener('click', async () => {
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
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}/add`, {
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

            // Clear inputs after successful addition
            typeInput.value = '';
            locationInput.value = '';
            tempInput.value = '';
            humidityInput.value = '';
            lightInput.value = '';
            infoInput.value = '';

            await fetchPlants();
        } catch (error) {
            console.error('Error adding plant:', error);
        }
    });

    // Fetch translations and initialize the page
    async function fetchTranslations(lang) {
        try {
            const response = await fetch(`../locales/${lang}.json`);
            if (!response.ok) {
                throw new Error(`Error fetching translations: ${response.statusText}`);
            }

            const translations = await response.json();
            for (const key in translations) {
                localizedText[key] = translations[key];
                const elements = document.querySelectorAll(`[data-translate="${key}"]`);
                elements.forEach(el => el.textContent = translations[key]);
            }
        } catch (error) {
            console.error('Error fetching translations:', error);
        }
    }

    document.getElementById('language-select').addEventListener('change', (event) => {
        const selectedLang = event.target.value;
        fetchTranslations(selectedLang);
    });

    fetchPlantTypes();
    fetchPlants();
    fetchTranslations('en'); // Default language
});