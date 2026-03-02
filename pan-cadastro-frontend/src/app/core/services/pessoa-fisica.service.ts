import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  PessoaFisicaResponse,
  CriarPessoaFisicaRequest,
  AtualizarPessoaFisicaRequest
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
// CRUD de PF via API
export class PessoaFisicaService {
  private readonly url = `${environment.apiUrl}/pessoasfisicas`;

  constructor(private http: HttpClient) {}

  listar(): Observable<PessoaFisicaResponse[]> {
    return this.http.get<ApiResponse<PessoaFisicaResponse[]>>(this.url)
      .pipe(map(r => r.dados ?? []));
  }

  obterPorId(id: string): Observable<PessoaFisicaResponse> {
    return this.http.get<ApiResponse<PessoaFisicaResponse>>(`${this.url}/${id}`)
      .pipe(map(r => r.dados!));
  }
  criar(request: CriarPessoaFisicaRequest): Observable<PessoaFisicaResponse> {
    return this.http.post<ApiResponse<PessoaFisicaResponse>>(this.url, request)
      .pipe(map(r => r.dados!));
  }
  atualizar(id: string, request: AtualizarPessoaFisicaRequest): Observable<PessoaFisicaResponse> {
    return this.http.put<ApiResponse<PessoaFisicaResponse>>(`${this.url}/${id}`, request)
      .pipe(map(r => r.dados!));
  }
  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
