import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { API_BASE_URL } from '../../api-base-url';
import { DataImport } from '../../models/data-import';

@Component({
  selector: 'app-data-management',
  imports: [MatTableModule, DatePipe],
  templateUrl: './data-management.html',
})
export class DataManagementPage implements OnInit {
  private readonly http = inject(HttpClient);

  readonly columns = [
    'id',
    'timestamp',
    'validEmployees',
    'invalidEmployees',
    'validJobs',
    'invalidJobs',
  ] as const;
  readonly rows = signal<DataImport[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.http.get<DataImport[]>(`${API_BASE_URL}/DataImports`).subscribe({
      next: (data) => {
        this.rows.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load data imports.');
        this.loading.set(false);
      },
    });
  }
}
