import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url';
import { DataImport } from '../models/data-import';

@Injectable({ providedIn: 'root' })
export class DataImportsService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<DataImport[]> {
    return this.http.get<DataImport[]>(`${API_BASE_URL}/DataImports`);
  }

  upload(jobs: File | null, employees: File | null): Observable<unknown> {
    const formData = new FormData();
    if (jobs) {
      formData.append('jobs', jobs);
    }
    if (employees) {
      formData.append('employees', employees);
    }
    return this.http.post(`${API_BASE_URL}/DataImports/upload`, formData);
  }
}
