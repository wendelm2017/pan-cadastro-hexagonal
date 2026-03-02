import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  PessoaJuridicaResponse,
  CriarPessoaJuridicaRequest,
  AtualizarPessoaJuridicaRequest
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
// CRUD de PJ via API
export class PessoaJuridicaService {
  private readonly url = `${environment.apiUrl}/pessoasjuridicas`;

  constructor(private http: HttpClient) {}

  listar(): Observable<PessoaJuridicaResponse[]> {
    return this.http.get<ApiResponse<PessoaJuridicaResponse[]>>(this.url)
      .pipe(map(r => r.dados ?? []));
  }
  obterPorId(id: string): Observable<PessoaJuridicaResponse> {
    return this.http.get<ApiResponse<PessoaJuridicaResponse>>(`${this.url}/${id}`)
      .pipe(map(r => r.dados!));
  }
  criar(request: CriarPessoaJuridicaRequest): Observable<PessoaJuridicaResponse> {
    return this.http.post<ApiResponse<PessoaJuridicaResponse>>(this.url, request)
      .pipe(map(r => r.dados!));
  }
  atualizar(id: string, request: AtualizarPessoaJuridicaRequest): Observable<PessoaJuridicaResponse> {
    return this.http.put<ApiResponse<PessoaJuridicaResponse>>(`${this.url}/${id}`, request)
      .pipe(map(r => r.dados!));
  }

  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
