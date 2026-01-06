import { Routes } from '@angular/router';
import { LaboratoryListComponent } from './features/laboratories/pages/laboratory-list/laboratory-list.component';
import { LaboratoryAdminComponent } from './features/laboratories/pages/laboratory-admin/laboratory-admin.component';
import { UserAdminComponent } from './features/users/pages/user-admin/user-admin.component';

export const routes: Routes = [
  { path: '', component: LaboratoryListComponent },

  { path: 'admin', component: LaboratoryAdminComponent },

  { path: 'users', component: UserAdminComponent },

  { path: '**', redirectTo: '' },
];
