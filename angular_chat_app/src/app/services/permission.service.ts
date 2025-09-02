import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private apiUrl = environment.apiUrl + 'Permission';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  create(permission: any): Observable<any> {
    return this.http.post(this.apiUrl, permission);
  }

  update(id: number, permission: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, permission);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}