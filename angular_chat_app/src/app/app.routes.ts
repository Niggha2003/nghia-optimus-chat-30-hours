import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./views/login-view/login-view').then(m => m.LoginView)
    },
    {
        path: 'register',
        loadComponent: () => import('./views/register-view/register-view').then(m => m.RegisterView)
    },
    
    {
        path: 'home',
        loadComponent: () => import('./views/home-view/home-view').then(m => m.HomeView),
        canActivate: [AuthGuard],
        children: [
            { path: 'role', loadComponent: () => import('./views/role-view/role-view').then(m => m.RoleView) },
            { path: 'permission', loadComponent: () => import('./views/permission-view/permission-view').then(m => m.PermissionView) },
            { path: 'user', loadComponent: () => import('./views/user-view/user-view').then(m => m.UserView) },
            { path: 'account', loadComponent: () => import('./views/account-view/account-view').then(m => m.AccountView) },
            { path: '', loadComponent: () => import('./views/chat-view/chat-view').then(m => m.ChatView) },
        ]
    },
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
    }
];
