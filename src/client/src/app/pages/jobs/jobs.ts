import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { API_BASE_URL } from '../../api-base-url';
import { Job } from '../../models/job';

@Component({
  selector: 'app-jobs',
  imports: [MatTableModule],
  templateUrl: './jobs.html',
})
export class JobsPage implements OnInit {
  private readonly http = inject(HttpClient);

  readonly columns = ['id', 'jobCode', 'name'] as const;
  readonly rows = signal<Job[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.http.get<Job[]>(`${API_BASE_URL}/Jobs`).subscribe({
      next: (data) => {
        this.rows.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load jobs.');
        this.loading.set(false);
      },
    });
  }
}
