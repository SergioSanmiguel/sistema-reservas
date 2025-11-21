const TOKEN_KEY = 'reservas_token';

export const TokenStorage = {
  saveToken(token: string) {
    localStorage.setItem(TOKEN_KEY, token);
  },
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  },
  removeToken() {
    localStorage.removeItem(TOKEN_KEY);
  },
  isLogged(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  }
};
