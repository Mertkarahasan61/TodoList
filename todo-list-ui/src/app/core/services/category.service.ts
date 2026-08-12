import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Category } from '../models/category.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private readonly apiUrl =
    'https://localhost:7183/api/categories';


  constructor(
    private http: HttpClient
  ) {
  }


  // Kategorileri getirir
  getCategories(): Observable<ApiResponse<Category[]>> {

    return this.http.get<ApiResponse<Category[]>>(
      this.apiUrl
    );
  }


  // Yeni kategori oluşturur
  createCategory(
    request: {
      name: string;
      color: string | null;
    }
  ): Observable<ApiResponse<Category>> {

    return this.http.post<ApiResponse<Category>>(
      this.apiUrl,
      request
    );
  }


  // Kategoriyi günceller
  updateCategory(
    id: number,
    request: {
      name: string;
      color: string | null;
    }
  ): Observable<ApiResponse<Category>> {

    return this.http.put<ApiResponse<Category>>(
      `${this.apiUrl}/${id}`,
      request
    );
  }


  // Kategoriyi siler
  deleteCategory(
    id: number
  ): Observable<ApiResponse<unknown>> {

    return this.http.delete<ApiResponse<unknown>>(
      `${this.apiUrl}/${id}`
    );
  }

}