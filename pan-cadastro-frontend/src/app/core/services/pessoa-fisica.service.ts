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
//esse serviço é responsável por interagir com a API de pessoas físicas para realizar operações
// como listar, obter por ID, criar, atualizar e remover pessoas físicas. Ele define os métodos
// correspondentes para cada operação, utilizando o HttpClient para fazer as requisições HTTP
// e mapeando as respostas da API para os objetos de resposta definidos nas interfaces.
export class PessoaFisicaService {
  private readonly url = `${environment.apiUrl}/pessoasfisicas`;

  constructor(private http: HttpClient) {}

  // O método listar() busca todos os registros de pessoas físicas cadastrados na API e retorna
  listar(): Observable<PessoaFisicaResponse[]> {
    return this.http.get<ApiResponse<PessoaFisicaResponse[]>>(this.url)
      .pipe(map(r => r.dados ?? []));
  }

  // O método obterPorId() busca uma pessoa física específica pelo seu ID e retorna um objeto
  obterPorId(id: string): Observable<PessoaFisicaResponse> {
    return this.http.get<ApiResponse<PessoaFisicaResponse>>(`${this.url}/${id}`)
      .pipe(map(r => r.dados!));
  }
  // O método criar() envia uma requisição POST para a API com os dados da nova pessoa física
  criar(request: CriarPessoaFisicaRequest): Observable<PessoaFisicaResponse> {
    return this.http.post<ApiResponse<PessoaFisicaResponse>>(this.url, request)
      .pipe(map(r => r.dados!));
  }
  // O método atualizar() envia uma requisição PUT para a API com o ID da pessoa física a ser atualizada
  atualizar(id: string, request: AtualizarPessoaFisicaRequest): Observable<PessoaFisicaResponse> {
    return this.http.put<ApiResponse<PessoaFisicaResponse>>(`${this.url}/${id}`, request)
      .pipe(map(r => r.dados!));
  }
  // O método remover() envia uma requisição DELETE para a API com o ID da pessoa física a ser removida.
  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
