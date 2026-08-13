import { Location } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { AuthService } from '../../core/services/auth.service';

type AuthMode = 'login' | 'register';

@Component({
  selector: 'app-auth',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, ButtonModule, CheckboxModule, InputTextModule, MessageModule],
  template: `
    <div class="bg-surface-50 dark:bg-surface-950 min-h-screen px-6 py-20 md:px-20 lg:px-80">
      <div
        class="bg-surface-0 dark:bg-surface-900 mx-auto flex w-full max-w-xl flex-col gap-8 rounded-2xl p-8 shadow-sm md:p-12"
      >
        <div class="flex flex-col items-center gap-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-14 w-14" width="33" height="32" viewBox="0 0 33 32" fill="none" aria-hidden="true">
            <path
              fill-rule="evenodd"
              clip-rule="evenodd"
              d="M7.09219 2.87829C5.94766 3.67858 4.9127 4.62478 4.01426 5.68992C7.6857 5.34906 12.3501 5.90564 17.7655 8.61335C23.5484 11.5047 28.205 11.6025 31.4458 10.9773C31.1517 10.087 30.7815 9.23135 30.343 8.41791C26.6332 8.80919 21.8772 8.29127 16.3345 5.51998C12.8148 3.76014 9.71221 3.03521 7.09219 2.87829ZM28.1759 5.33332C25.2462 2.06 20.9887 0 16.25 0C14.8584 0 13.5081 0.177686 12.2209 0.511584C13.9643 0.987269 15.8163 1.68319 17.7655 2.65781C21.8236 4.68682 25.3271 5.34013 28.1759 5.33332ZM32.1387 14.1025C28.2235 14.8756 22.817 14.7168 16.3345 11.4755C10.274 8.44527 5.45035 8.48343 2.19712 9.20639C2.0292 9.24367 1.86523 9.28287 1.70522 9.32367C1.2793 10.25 0.939308 11.2241 0.695362 12.2356C0.955909 12.166 1.22514 12.0998 1.50293 12.0381C5.44966 11.161 11.0261 11.1991 17.7655 14.5689C23.8261 17.5991 28.6497 17.561 31.9029 16.838C32.0144 16.8133 32.1242 16.7877 32.2322 16.7613C32.2441 16.509 32.25 16.2552 32.25 16C32.25 15.358 32.2122 14.7248 32.1387 14.1025ZM31.7098 20.1378C27.8326 20.8157 22.5836 20.5555 16.3345 17.431C10.274 14.4008 5.45035 14.439 2.19712 15.1619C1.475 15.3223 0.825392 15.5178 0.252344 15.7241C0.250782 15.8158 0.25 15.9078 0.25 16C0.25 24.8366 7.41344 32 16.25 32C23.6557 32 29.8862 26.9687 31.7098 20.1378Z"
              class="fill-surface-700 dark:fill-surface-200"
            />
          </svg>
          <div class="flex w-full flex-col items-center gap-2">
            <div class="text-surface-900 dark:text-surface-0 w-full text-center text-2xl leading-tight font-semibold">
              {{ currentMode() === 'login' ? 'Welcome Back' : 'Create an Account' }}
            </div>
            <div class="w-full text-center">
              <span class="text-surface-700 dark:text-surface-200 leading-normal">
                {{ currentMode() === 'login' ? "Don't have an account?" : 'Already have an account?' }}
              </span>
              <button
                type="button"
                class="text-primary hover:text-primary-emphasis ml-1 cursor-pointer bg-transparent p-0 font-medium"
                (click)="toggleMode()"
              >
                {{ currentMode() === 'login' ? 'Create today!' : 'Sign in' }}
              </button>
            </div>
          </div>
        </div>

        @if (errorMessage()) {
          <p-message severity="error" variant="simple">{{ errorMessage() }}</p-message>
        }

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="flex w-full flex-col gap-6">
          @if (currentMode() === 'register') {
            <div class="flex w-full flex-col gap-2">
              <label for="displayName" class="text-surface-900 dark:text-surface-0 font-medium leading-normal">Display Name</label>
              <input
                pInputText
                id="displayName"
                type="text"
                formControlName="displayName"
                placeholder="Display name"
                class="w-full rounded-lg px-3 py-2 shadow-sm"
                [invalid]="isInvalid('displayName')"
              />
              @if (isInvalid('displayName')) {
                <p-message severity="error" variant="simple" size="small">Display name is required.</p-message>
              }
            </div>
          }

          <div class="flex w-full flex-col gap-2">
            <label for="email" class="text-surface-900 dark:text-surface-0 font-medium leading-normal">Email Address</label>
            <input
              pInputText
              id="email"
              type="email"
              formControlName="email"
              placeholder="Email address"
              class="w-full rounded-lg px-3 py-2 shadow-sm"
              [invalid]="isInvalid('email')"
            />
            @if (isInvalid('email')) {
              <p-message severity="error" variant="simple" size="small">Enter a valid email address.</p-message>
            }
          </div>

          <div class="flex w-full flex-col gap-2">
            <label for="password" class="text-surface-900 dark:text-surface-0 font-medium leading-normal">Password</label>
            <input
              pInputText
              id="password"
              type="password"
              formControlName="password"
              placeholder="Password"
              class="w-full rounded-lg px-3 py-2 shadow-sm"
              [invalid]="isInvalid('password')"
            />
            @if (isInvalid('password')) {
              <p-message severity="error" variant="simple" size="small">Password must be at least 8 characters.</p-message>
            }
          </div>

          @if (currentMode() === 'login') {
            <div class="flex w-full flex-col items-start justify-between gap-3 sm:flex-row sm:items-center sm:gap-0">
              <div class="flex items-center gap-2">
                <p-checkbox formControlName="rememberMe" [binary]="true" inputId="rememberMe" />
                <label for="rememberMe" class="text-surface-900 dark:text-surface-0 leading-normal">Remember me</label>
              </div>
              <button type="button" class="text-primary hover:text-primary-emphasis cursor-pointer bg-transparent p-0 font-medium" disabled title="Coming soon">
                Forgot your password?
              </button>
            </div>
          }

          <button pButton type="submit" [disabled]="loading()" class="flex w-full items-center justify-center gap-2 rounded-lg py-2">
            @if (loading()) {
              <i class="pi pi-spinner pi-spin" aria-hidden="true"></i>
            } @else {
              <i class="pi pi-user" aria-hidden="true"></i>
            }
            <span>{{ currentMode() === 'login' ? 'Sign In' : 'Create Account' }}</span>
          </button>
        </form>
      </div>
    </div>
  `,
})
export class AuthComponent {
  // Dependencies
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private location = inject(Location);
  private destroyRef = inject(DestroyRef);

  // Route-bound input: `data.mode` on the /login and /register routes (see app.routes.ts).
  // Requires withComponentInputBinding() in app.config.ts.
  mode = input<AuthMode>('login');

  // State
  currentMode = signal<AuthMode>(this.mode());
  loading = signal(false);
  errorMessage = signal<string | null>(null);
  private submitted = false;

  form = this.fb.nonNullable.group({
    displayName: [''],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    rememberMe: [true],
  });

  constructor() {
    this.updateDisplayNameValidator(this.currentMode());
  }

  /**
   * Switches between login/register instantly, in place, with no route
   * navigation (so no component remount / flicker). The address bar is kept
   * in sync via replaceState so direct links and refreshes still land on the
   * right mode, without piling up extra back-button history entries.
   */
  toggleMode() {
    const next: AuthMode = this.currentMode() === 'login' ? 'register' : 'login';
    this.currentMode.set(next);
    this.errorMessage.set(null);
    this.submitted = false;
    this.updateDisplayNameValidator(next);
    this.location.replaceState(`/${next}`);
  }

  isInvalid(controlName: keyof typeof this.form.controls) {
    const control = this.form.controls[controlName];
    return control.invalid && ((control.dirty && control.touched) || this.submitted);
  }

  onSubmit() {
    this.submitted = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.loading.set(true);

    const { displayName, email, password } = this.form.getRawValue();
    const request$ =
      this.currentMode() === 'login'
        ? this.authService.login(email, password)
        : this.authService.register(displayName, email, password);

    request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      error: (err: Error) => {
        this.errorMessage.set(err.message);
        this.loading.set(false);
      },
    });
  }

  private updateDisplayNameValidator(mode: AuthMode) {
    const control = this.form.controls.displayName;
    if (mode === 'register') {
      control.setValidators([Validators.required, Validators.maxLength(50)]);
    } else {
      control.clearValidators();
    }
    control.updateValueAndValidity();
  }
}
