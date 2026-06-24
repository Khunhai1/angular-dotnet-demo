import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, tap, throwError } from 'rxjs';
import { User } from '../models/user.model';
import { AuthResponse } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  // Dependencies
  private http = inject(HttpClient);
  private router = inject(Router);

  // Private attributes
  private currentUser = signal<User | null>(null);
  private accessToken = signal<string | null>(null);

  // Public attributes
  isLoggedIn = computed(() => this.accessToken() !== null);

  getToken() {
    return this.accessToken();
  }

  /**
   * Signs in the user, keeps its token in memory and navigates to chat page.
   * @param email
   * @param password
   * @returns an Observable
   */
  login(email: string, password: string) {
    return this.http.post<AuthResponse>('/api/Auth/login', { email, password }).pipe(
      tap({ next: (response) => this.handleSuccessAuthResponse(response) }),
      catchError((err: HttpErrorResponse) =>
        throwError(() => new Error(err.error ?? 'Login failed')),
      ),
    );
  }

  /**
   * Registers and signs in the user, keeps its token in memory and navigates to chat page.
   * @param displayName
   * @param email
   * @param password
   * @returns an Observable
   */
  register(displayName: string, email: string, password: string) {
    return this.http
      .post<AuthResponse>('/api/Auth/register', { displayName, email, password })
      .pipe(
        tap({ next: (response) => this.handleSuccessAuthResponse(response) }),
        catchError((err: HttpErrorResponse) => {
          const message = Array.isArray(err.error)
            ? err.error.map((e: { description: string }) => e.description).join(' ')
            : (err.error ?? 'Registration failed');
          return throwError(() => new Error(message));
        }),
      );
  }

  private handleSuccessAuthResponse(response: AuthResponse) {
    this.currentUser.set({
      userId: response.userId,
      displayName: response.displayName,
      email: response.email,
    });
    this.accessToken.set(response.token);
    this.router.navigate(['/chat']);
  }
}
