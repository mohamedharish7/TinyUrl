import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TinyUrl, CreateUrlDto } from '../models/tiny-url.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TinyUrlService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createUrl(dto: CreateUrlDto): Observable<TinyUrl> {
    return this.http.post<TinyUrl>(`${this.baseUrl}/api/shorten`, dto);
  }

  getUrls(): Observable<TinyUrl[]> {
    return this.http.get<TinyUrl[]>(`${this.baseUrl}/api/urls`);
  }

  deleteUrl(shortCode: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/api/urls/${shortCode}`);
  }
}
