import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api-base-url';
import { Job } from '../models/job';

@Injectable({ providedIn: 'root' })
export class JobsService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Job[]> {
    return this.http.get<Job[]>(`${API_BASE_URL}/Jobs`);
  }
}
