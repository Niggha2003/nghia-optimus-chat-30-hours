import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = environment.apiUrl + 'Role';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  create(role: any): Observable<any> {
    return this.http.post(this.apiUrl, role);
  }

  update(id: number, role: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, role);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  updateRolePermissions(id: number, permissions: any[]): Observable<any> {
    return this.http.put(`${this.apiUrl}/Permissions/UpdateRolePermissions/${id}`, permissions);
  }

  getRolePermissions(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/Permissions/${id}`);
  }
}