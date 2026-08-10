import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url';
import { DataImport } from '../models/data-import';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class DataImportsService {
  private readonly http = inject(HttpClient);

  getPage(skip: number, take: number): Observable<PagedResult<DataImport>> {
    const params = new HttpParams().set('skip', skip).set('take', take);
    return this.http.get<PagedResult<DataImport>>(`${API_BASE_URL}/DataImports`, { params });
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
