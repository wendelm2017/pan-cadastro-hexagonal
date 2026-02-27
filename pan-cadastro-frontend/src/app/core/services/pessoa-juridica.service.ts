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
//esse serviço é responsável por interagir com a API de pessoas jurídicas para realizar operações
// como listar, obter por ID, criar, atualizar e remover pessoas jurídicas. Ele define os métodos
// correspondentes para cada operação, utilizando o HttpClient para fazer as requisições HTTP
// e mapeando as respostas da API para os objetos de resposta definidos nas interfaces.
export class PessoaJuridicaService {
  private readonly url = `${environment.apiUrl}/pessoasjuridicas`;

  constructor(private http: HttpClient) {}

  // O método listar() busca todos os registros de pessoas jurídicas cadastrados na API e retorna
  listar(): Observable<PessoaJuridicaResponse[]> {
    return this.http.get<ApiResponse<PessoaJuridicaResponse[]>>(this.url)
      .pipe(map(r => r.dados ?? []));
  }
  // O método obterPorId() busca uma pessoa jurídica específica pelo seu ID e retorna um objeto
  obterPorId(id: string): Observable<PessoaJuridicaResponse> {
    return this.http.get<ApiResponse<PessoaJuridicaResponse>>(`${this.url}/${id}`)
      .pipe(map(r => r.dados!));
  }
  // O método criar() envia uma requisição POST para a API com os dados da nova pessoa jurídica  
  criar(request: CriarPessoaJuridicaRequest): Observable<PessoaJuridicaResponse> {
    return this.http.post<ApiResponse<PessoaJuridicaResponse>>(this.url, request)
      .pipe(map(r => r.dados!));
  }
  // O método atualizar() envia uma requisição PUT para a API com o ID da pessoa jurídica a ser atualizada
  atualizar(id: string, request: AtualizarPessoaJuridicaRequest): Observable<PessoaJuridicaResponse> {
    return this.http.put<ApiResponse<PessoaJuridicaResponse>>(`${this.url}/${id}`, request)
      .pipe(map(r => r.dados!));
  }

  // O método remover() envia uma requisição DELETE para a API com o ID da pessoa jurídica a ser removida.
  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
