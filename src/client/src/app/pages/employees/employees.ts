import { Component, OnInit, inject, signal } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { Employee } from '../../models/employee';
import { EmployeesService } from '../../services/employees.service';

@Component({
  selector: 'app-employees',
  imports: [MatTableModule, MatPaginatorModule],
  templateUrl: './employees.html',
})
export class EmployeesPage implements OnInit {
  private readonly employeesService = inject(EmployeesService);

  readonly pageSize = 10;
  readonly columns = ['id', 'employeeCode', 'jobCode', 'firstName', 'lastName', 'department'] as const;
  readonly rows = signal<Employee[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadPage(0);
  }

  onPage(event: PageEvent): void {
    this.loadPage(event.pageIndex, false);
  }

  private loadPage(pageIndex: number, showLoading = true): void {
    if (showLoading) {
      this.loading.set(true);
    }
    this.error.set(null);

    this.employeesService.getPage(pageIndex * this.pageSize, this.pageSize).subscribe({
      next: (result) => {
        this.rows.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load employees.');
        this.loading.set(false);
      },
    });
  }
}
