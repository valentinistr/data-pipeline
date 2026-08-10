import { Routes } from '@angular/router';
import { Layout } from './layout/layout';
import { HomePage } from './pages/home/home';
import { EmployeesPage } from './pages/employees/employees';
import { JobsPage } from './pages/jobs/jobs';
import { DataManagementPage } from './pages/data-management/data-management';

export const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      { path: '', component: HomePage },
      { path: 'employees', component: EmployeesPage },
      { path: 'jobs', component: JobsPage },
      { path: 'data-management', component: DataManagementPage },
    ],
  },
  { path: '**', redirectTo: '' },
];
