import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { LoginCreds, RegisterCreds, User } from '../../types/user';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);

  baseUrl = 'https://localhost:5001/api/';

  currentUser = signal<User | null>(null);

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      }),
    );
  }

  login(creds: LoginCreds) {
    return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      }),
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  setCurrentUser(user: User) {
    localStorage.setItem("user", JSON.stringify(user));
    this.currentUser.set(user);
  }
}
