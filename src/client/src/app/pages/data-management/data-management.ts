import { Component, OnInit, computed, inject, signal, viewChild, ElementRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { DataImport } from '../../models/data-import';
import { DataImportsService } from '../../services/data-imports.service';

type UploadStatus = 'idle' | 'uploading' | 'success' | 'error';

@Component({
  selector: 'app-data-management',
  imports: [MatTableModule, MatPaginatorModule, DatePipe],
  templateUrl: './data-management.html',
  styleUrl: './data-management.scss',
})
export class DataManagementPage implements OnInit {
  private readonly dataImportsService = inject(DataImportsService);

  private readonly jobsInput = viewChild<ElementRef<HTMLInputElement>>('jobsInput');
  private readonly employeesInput = viewChild<ElementRef<HTMLInputElement>>('employeesInput');

  readonly pageSize = 10;
  readonly columns = [
    'id',
    'uploaded',
    'completed',
    'status',
    'validEmployees',
    'invalidEmployees',
    'validJobs',
    'invalidJobs',
  ] as const;
  readonly rows = signal<DataImport[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly jobsFile = signal<File | null>(null);
  readonly employeesFile = signal<File | null>(null);
  readonly jobsFileError = signal<string | null>(null);
  readonly employeesFileError = signal<string | null>(null);
  readonly uploading = signal(false);
  readonly uploadStatus = signal<UploadStatus>('idle');

  readonly canUpload = computed(
    () =>
      !this.uploading() &&
      (this.jobsFile() !== null || this.employeesFile() !== null) &&
      this.jobsFileError() === null &&
      this.employeesFileError() === null,
  );

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

    this.dataImportsService.getPage(pageIndex * this.pageSize, this.pageSize).subscribe({
      next: (result) => {
        this.rows.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load data imports.');
        this.loading.set(false);
      },
    });
  }

  onJobsSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    if (file && !this.hasFileNamePrefix(file.name, 'jobs')) {
      this.jobsFile.set(null);
      this.jobsFileError.set("Jobs file name must start with 'jobs'.");
      input.value = '';
      return;
    }

    this.jobsFileError.set(null);
    this.jobsFile.set(file);
  }

  onEmployeesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    if (file && !this.hasFileNamePrefix(file.name, 'employees')) {
      this.employeesFile.set(null);
      this.employeesFileError.set("Employees file name must start with 'employees'.");
      input.value = '';
      return;
    }

    this.employeesFileError.set(null);
    this.employeesFile.set(file);
  }

  upload(): void {
    if (!this.canUpload()) {
      return;
    }

    this.uploading.set(true);
    this.uploadStatus.set('uploading');

    this.dataImportsService.upload(this.jobsFile(), this.employeesFile()).subscribe({
      next: () => {
        this.uploading.set(false);
        this.uploadStatus.set('success');
        this.clearFiles();
      },
      error: () => {
        this.uploading.set(false);
        this.uploadStatus.set('error');
      },
    });
  }

  private hasFileNamePrefix(fileName: string, prefix: string): boolean {
    const name = fileName.split(/[/\\]/).pop() ?? fileName;
    return name.toLowerCase().startsWith(prefix.toLowerCase());
  }

  private clearFiles(): void {
    this.jobsFile.set(null);
    this.employeesFile.set(null);
    this.jobsFileError.set(null);
    this.employeesFileError.set(null);
    const jobsEl = this.jobsInput()?.nativeElement;
    const employeesEl = this.employeesInput()?.nativeElement;
    if (jobsEl) {
      jobsEl.value = '';
    }
    if (employeesEl) {
      employeesEl.value = '';
    }
  }
}
