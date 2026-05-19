import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login.component';
import { RegisterComponent } from './pages/register.component';
import { HomeComponent } from './pages/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent }
];
