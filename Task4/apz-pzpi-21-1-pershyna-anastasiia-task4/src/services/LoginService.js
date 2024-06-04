class LoginService {
    constructor(apiUrl) {
        this.apiUrl = apiUrl;
    }

    async login(email, password) {
        try {
            const response = await fetch(`${this.apiUrl}/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            throw new Error(error.message);
        }
    }
}
