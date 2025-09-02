import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = environment.apiUrl + 'User';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  create(user: any): Observable<any> {
    return this.http.post(this.apiUrl, user);
  }

  update(id: number, user: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, user);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  updateUserRoles(id: number, roles: any[]): Observable<any> {
    return this.http.put(`${this.apiUrl}/Role/UpdateUserRoles/${id}`, roles);
  }

  getUserRoles(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/Role/${id}`);
  }

  getCurrentUser(): Observable<any> {
    return this.http.get(`${this.apiUrl}/CurrentUser`);
  }
}