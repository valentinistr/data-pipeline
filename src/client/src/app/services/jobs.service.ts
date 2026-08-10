import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url';
import { Job } from '../models/job';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class JobsService {
  private readonly http = inject(HttpClient);

  getPage(skip: number, take: number, search?: string): Observable<PagedResult<Job>> {
    let params = new HttpParams().set('skip', skip).set('take', take);
    const term = search?.trim();
    if (term) {
      params = params.set('search', term);
    }
    return this.http.get<PagedResult<Job>>(`${API_BASE_URL}/Jobs`, { params });
  }
}
