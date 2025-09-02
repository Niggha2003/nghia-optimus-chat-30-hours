import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
  private apiUrl = environment.apiUrl + 'Group';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any> {
    return this.http.get(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  create(group: any): Observable<any> {
    return this.http.post(this.apiUrl, group);
  }

  update(id: number, group: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, group);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  updateGroupUsers(id: number, users: any[]): Observable<any> {
    return this.http.put(`${this.apiUrl}/User/UpdateGroupUsers/${id}`, users);
  }

  getGroupUsers(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/User/${id}`);
  }
}