import { Component, OnInit, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { Employee } from '../../models/employee';
import { EmployeesService } from '../../services/employees.service';

@Component({
  selector: 'app-employees',
  imports: [MatTableModule],
  templateUrl: './employees.html',
})
export class EmployeesPage implements OnInit {
  private readonly employeesService = inject(EmployeesService);

  readonly columns = ['id', 'employeeCode', 'jobCode', 'firstName', 'lastName', 'department'] as const;
  readonly rows = signal<Employee[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.employeesService.getAll().subscribe({
      next: (data) => {
        this.rows.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load employees.');
        this.loading.set(false);
      },
    });
  }
}
