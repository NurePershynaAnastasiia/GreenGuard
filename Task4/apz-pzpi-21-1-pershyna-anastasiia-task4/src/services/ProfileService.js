class ProfileService {
    constructor(apiUrl, updateUrl) {
        this.apiUrl = apiUrl;
        this.updateUrl = updateUrl;
    }

    getToken() {
        return localStorage.getItem('token');
    }

    getUserData() {
        try {
            const userData = JSON.parse(localStorage.getItem('userData'));
            return userData?.nameid ? userData : null;
        } catch (error) {
            console.error('Error parsing user data:', error);
            return null;
        }
    }

    async fetchData(url, headers) {
        try {
            const response = await fetch(url, { headers });
            if (response.ok) return response.json();
            console.error('Error:', await response.text());
        } catch (error) {
            console.error('Error:', error.message);
        }
        return null;
    }

    async fetchWorkerId() {
        const userData = this.getUserData();
        if (!userData) {
            console.error('Error: User data not found in localStorage');
            return null;
        }

        const token = this.getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        const url = `${this.apiUrl}${userData.nameid}`;
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        return await this.fetchData(url, headers);
    }

    async updateWorker(updatedWorker) {
        const token = this.getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        const url = `${this.updateUrl}${updatedWorker.workerId}`;
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        try {
            const response = await fetch(url, {
                method: 'PUT',
                headers,
                body: JSON.stringify(updatedWorker)
            });

            if (response.ok) {
                return true;
            } else {
                console.error('Error:', await response.text());
                return false;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return false;
        }
    }

    async loadLanguage(lang) {
        try {
            const response = await fetch(`../../public/locales/${lang}/${lang}.json`);
            return response.json();
        } catch (error) {
            console.error('Error loading language file:', error);
            return {};
        }
    }
}
