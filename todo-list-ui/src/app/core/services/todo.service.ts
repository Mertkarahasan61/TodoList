import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Todo } from '../models/todo.model';

import {
  ApiResponse,
  PagedResult
} from '../models/api-response.model';


@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private readonly apiUrl =
    'https://localhost:7183/api/todos';


  constructor(
    private http: HttpClient
  ) {
  }


  // Todo listesini getirir
  getTodos(
    params?: HttpParams
  ): Observable<ApiResponse<PagedResult<Todo>>> {

    return this.http.get<ApiResponse<PagedResult<Todo>>>(
      this.apiUrl,
      { params }
    );
  }


  // Yeni Todo oluşturur
  createTodo(
    request: {
      title: string;
      description: string | null;
      priority: number;

      // Artık Date göndermiyoruz.
      // Backend'e 2026-08-20 gibi string göndereceğiz.
      dueDate: string | null;

      categoryId: number | null;
    }
  ): Observable<ApiResponse<Todo>> {

    return this.http.post<ApiResponse<Todo>>(
      this.apiUrl,
      request
    );
  }


  // Todo günceller
  updateTodo(
    id: number,
    request: {
      title: string;
      description: string | null;
      priority: number;

      // Burada da string tarih gönderiyoruz.
      dueDate: string | null;

      categoryId: number | null;
    }
  ): Observable<ApiResponse<Todo>> {

    return this.http.put<ApiResponse<Todo>>(
      `${this.apiUrl}/${id}`,
      request
    );
  }


  // Tamamlandı / aktif durumunu değiştirir
  changeStatus(
    id: number,
    isCompleted: boolean
  ): Observable<ApiResponse<unknown>> {

    return this.http.patch<ApiResponse<unknown>>(
      `${this.apiUrl}/${id}/status`,
      { isCompleted }
    );
  }


  // Todo siler
  deleteTodo(
    id: number
  ): Observable<ApiResponse<unknown>> {

    return this.http.delete<ApiResponse<unknown>>(
      `${this.apiUrl}/${id}`
    );
  }

}