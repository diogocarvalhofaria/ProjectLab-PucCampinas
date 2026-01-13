import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { HomeComponent } from './features/home/home.component';
import { LaboratoryListComponent } from './features/laboratories/pages/laboratory-list/laboratory-list.component';
import { UserAdminComponent } from './features/users/pages/user-admin/user-admin.component';
import { LoginComponent } from './features/auth/login/login.component';
import { SetupPasswordComponent } from './features/auth/setup-password/setup-password.component';
import { MyReservationComponent } from './features/reservations/my-reservation/my-reservation.component';
import { ReservationAdminComponent } from './features/reservations/pages/reservation-admin/reservation-admin.component';
import { LaboratoryAdminComponent } from './features/laboratories/pages/laboratory-admin/laboratory-admin.component';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  // Rotas Públicas
  { path: 'login', component: LoginComponent },
  { path: 'setup-password', component: SetupPasswordComponent },

  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      // Acessível para Alunos e Admins
      { path: '', component: HomeComponent },
      { path: 'laboratories', component: LaboratoryListComponent },
      { path: 'my-reservations', component: MyReservationComponent },
      {
        path: 'admin/laboratories',
        component: LaboratoryAdminComponent,
        canActivate: [adminGuard],
      },
      {
        path: 'admin/reservations',
        component: ReservationAdminComponent,
        canActivate: [adminGuard],
      },
      {
        path: 'users',
        component: UserAdminComponent,
        canActivate: [adminGuard],
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
