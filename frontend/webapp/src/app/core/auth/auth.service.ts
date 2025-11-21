import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TokenStorage } from '../utils/token-storage';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private base = '/api/auth';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<void> {
    return this.http.post<{ token: string; user?: any }>(`${this.base}/login`, { email, password })
      .pipe(
        map(resp => {
          if (resp && resp.token) {
            TokenStorage.saveToken(resp.token);
          }
        })
      );
  }

  logout() {
    TokenStorage.removeToken();
  }

  getToken(): string | null {
    return TokenStorage.getToken();
  }

  isAuthenticated(): boolean {
    return TokenStorage.isLogged();
  }
}
