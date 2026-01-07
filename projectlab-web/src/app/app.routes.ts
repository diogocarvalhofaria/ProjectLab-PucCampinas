import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { HomeComponent } from './features/home/home.component';
import { LaboratoryListComponent } from './features/laboratories/pages/laboratory-list/laboratory-list.component';
import { LaboratoryAdminComponent } from './features/laboratories/pages/laboratory-admin/laboratory-admin.component';
import { UserAdminComponent } from './features/users/pages/user-admin/user-admin.component';
import { LoginComponent } from './features/auth/login/login.component';
import { SetupPasswordComponent } from './features/auth/setup-password/setup-password.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'setup-password', component: SetupPasswordComponent },
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomeComponent },

      { path: 'laboratories', component: LaboratoryListComponent },

      { path: 'admin', component: LaboratoryAdminComponent },
      { path: 'users', component: UserAdminComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
