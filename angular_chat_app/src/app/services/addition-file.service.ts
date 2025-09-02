import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class  AdditionFileService {
  private apiUrl = environment.apiUrl + 'AdditionFile';

  constructor(private http: HttpClient) {}

  getAll(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/Message/${id}`);
  }

  getById(id: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`, {
      responseType: 'blob'
    });
  }

  create(message: any): Observable<any> {
    return this.http.post(this.apiUrl, message);
  }

  update(id: number, message: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, message);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}