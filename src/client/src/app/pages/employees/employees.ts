import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { API_BASE_URL } from '../../api-base-url';
import { Employee } from '../../models/employee';

@Component({
  selector: 'app-employees',
  imports: [MatTableModule],
  templateUrl: './employees.html',
})
export class EmployeesPage implements OnInit {
  private readonly http = inject(HttpClient);

  readonly columns = ['id', 'jobCode', 'firstName', 'lastName', 'department'] as const;
  readonly rows = signal<Employee[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.http.get<Employee[]>(`${API_BASE_URL}/Employees`).subscribe({
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
