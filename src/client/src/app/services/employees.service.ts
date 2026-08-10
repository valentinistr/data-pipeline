import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url';
import { Employee } from '../models/employee';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class EmployeesService {
  private readonly http = inject(HttpClient);

  getPage(skip: number, take: number): Observable<PagedResult<Employee>> {
    const params = new HttpParams().set('skip', skip).set('take', take);
    return this.http.get<PagedResult<Employee>>(`${API_BASE_URL}/Employees`, { params });
  }
}
