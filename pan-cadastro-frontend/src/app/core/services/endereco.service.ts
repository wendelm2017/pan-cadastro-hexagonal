import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  EnderecoResponse,
  CriarEnderecoRequest,
  AtualizarEnderecoRequest,
  ViaCepResponseDto
} from '../models/api.models';
// CRUD de Endereços + consulta ViaCEP
@Injectable({ providedIn: 'root' })
export class EnderecoService {
  private readonly url = `${environment.apiUrl}/enderecos`;

  constructor(private http: HttpClient) {}

  listar(): Observable<EnderecoResponse[]> {
    return this.http.get<ApiResponse<EnderecoResponse[]>>(this.url)
      .pipe(map(r => r.dados ?? []));
  }

  obterPorId(id: string): Observable<EnderecoResponse> {  
    return this.http.get<ApiResponse<EnderecoResponse>>(`${this.url}/${id}`)
      .pipe(map(r => r.dados!));
  }

  obterPorPessoaFisica(pessoaFisicaId: string): Observable<EnderecoResponse[]> {
    return this.http.get<ApiResponse<EnderecoResponse[]>>(`${this.url}/pessoa-fisica/${pessoaFisicaId}`)
      .pipe(map(r => r.dados ?? []));
  }

  obterPorPessoaJuridica(pessoaJuridicaId: string): Observable<EnderecoResponse[]> {
    return this.http.get<ApiResponse<EnderecoResponse[]>>(`${this.url}/pessoa-juridica/${pessoaJuridicaId}`)
      .pipe(map(r => r.dados ?? []));
  }

  consultarCep(cep: string): Observable<ViaCepResponseDto | null> {
    const apenasDigitos = cep.replace(/\D/g, '');
    return this.http.get<ApiResponse<ViaCepResponseDto>>(`${this.url}/cep/${apenasDigitos}`)
      .pipe(
        map(r => r.dados),
        catchError(() => of(null))
      );
  }

  criar(request: CriarEnderecoRequest): Observable<EnderecoResponse> {
    return this.http.post<ApiResponse<EnderecoResponse>>(this.url, request)
      .pipe(map(r => r.dados!));
  }

  atualizar(id: string, request: AtualizarEnderecoRequest): Observable<EnderecoResponse> {
    return this.http.put<ApiResponse<EnderecoResponse>>(`${this.url}/${id}`, request)
      .pipe(map(r => r.dados!));
  }

  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
