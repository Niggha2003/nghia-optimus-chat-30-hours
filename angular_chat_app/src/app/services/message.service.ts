import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private apiUrl = environment.apiUrl + 'Messages';

  constructor(private http: HttpClient) {}

  getAllGroup(groupId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/Group/${groupId}`);
  }

  getAll(fromId: number, toId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${fromId}/${toId}`);
  }

  getById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  create(message: any): Observable<any> {
    return this.http.post(this.apiUrl, message);
  }

  update(message: any): Observable<any> {
    return this.http.put(`${this.apiUrl}`, message);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}