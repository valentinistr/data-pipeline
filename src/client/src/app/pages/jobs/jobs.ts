import { Component, OnInit, inject, signal } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { Job } from '../../models/job';
import { JobsService } from '../../services/jobs.service';

@Component({
  selector: 'app-jobs',
  imports: [MatTableModule, MatPaginatorModule],
  templateUrl: './jobs.html',
})
export class JobsPage implements OnInit {
  private readonly jobsService = inject(JobsService);

  readonly pageSize = 10;
  readonly columns = ['id', 'jobCode', 'name'] as const;
  readonly rows = signal<Job[]>([]);
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

    this.jobsService.getPage(pageIndex * this.pageSize, this.pageSize).subscribe({
      next: (result) => {
        this.rows.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load jobs.');
        this.loading.set(false);
      },
    });
  }
}
