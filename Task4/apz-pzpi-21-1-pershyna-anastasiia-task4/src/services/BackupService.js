class BackupService {
    constructor(apiUrl) {
        this.apiUrl = apiUrl;
    }

    async fetchBackups(token) {
        try {
            const response = await fetch(`${this.apiUrl}/backups`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error fetching backups: ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            throw new Error(error.message);
        }
    }

    async createBackup(token) {
        try {
            const response = await fetch(`${this.apiUrl}/add`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`Error creating backup: ${errorData.message}`);
            }
        } catch (error) {
            throw new Error(error.message);
        }
    }

    async restoreBackup(token, backupFileName) {
        const formData = new FormData();
        formData.append('backupFileName', backupFileName);

        try {
            const response = await fetch(`${this.apiUrl}/restore/${backupFileName}`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`Error restoring database: ${errorData.message}`);
            }
        } catch (error) {
            throw new Error(error.message);
        }
    }
}
