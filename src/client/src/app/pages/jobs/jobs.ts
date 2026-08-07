import { Component, OnInit, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { Job } from '../../models/job';
import { JobsService } from '../../services/jobs.service';

@Component({
  selector: 'app-jobs',
  imports: [MatTableModule],
  templateUrl: './jobs.html',
})
export class JobsPage implements OnInit {
  private readonly jobsService = inject(JobsService);

  readonly columns = ['id', 'jobCode', 'name'] as const;
  readonly rows = signal<Job[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.jobsService.getAll().subscribe({
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
