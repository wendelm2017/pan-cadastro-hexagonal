import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  EnderecoResponse,
  CriarEnderecoRequest,
  AtualizarEnderecoRequest,
  ViaCepResponseDto
} from '../models/api.models';
//esse serviço é responsável por interagir com a API de endereços para realizar operações
// como listar, obter por ID, criar, atualizar e remover endereços. Ele também inclui
// um método para consultar o CEP usando a API ViaCEP, limpando a formatação do CEP
@Injectable({ providedIn: 'root' })
export class EnderecoService {
  private readonly url = `${environment.apiUrl}/enderecos`;

  constructor(private http: HttpClient) {}

  // O método listar() busca todos os endereços cadastrados na API e retorna
  // um array de EnderecoResponse.
  listar(): Observable<EnderecoResponse[]> {
    return this.http.get<ApiResponse<EnderecoResponse[]>>(this.url)
      .pipe(map(r => r.dados ?? []));
  }

  // O método obterPorId() busca um endereço específico pelo seu ID e retorna um
  // objeto EnderecoResponse. Se o endereço não for encontrado, a API deve retornar
  obterPorId(id: string): Observable<EnderecoResponse> {  
    return this.http.get<ApiResponse<EnderecoResponse>>(`${this.url}/${id}`)
      .pipe(map(r => r.dados!));
  }

  // Os métodos obterPorPessoaFisica() e obterPorPessoaJuridica() buscam os endereços
  // associados a uma pessoa física ou jurídica, respectivamente, usando o ID da pessoa.
  obterPorPessoaFisica(pessoaFisicaId: string): Observable<EnderecoResponse[]> {
    return this.http.get<ApiResponse<EnderecoResponse[]>>(`${this.url}/pessoa-fisica/${pessoaFisicaId}`)
      .pipe(map(r => r.dados ?? []));
  }

  // O método consultarCep() recebe um CEP, remove qualquer formatação (como hífens) e
  // faz uma requisição para a API de endereços para obter os dados do endereço correspondente
  // ao CEP usando a API ViaCEP. Ele retorna um objeto ViaCepResponseDto ou null se o CEP não for encontrado.
  obterPorPessoaJuridica(pessoaJuridicaId: string): Observable<EnderecoResponse[]> {
    return this.http.get<ApiResponse<EnderecoResponse[]>>(`${this.url}/pessoa-juridica/${pessoaJuridicaId}`)
      .pipe(map(r => r.dados ?? []));
  }

  // O método criar() envia uma requisição POST para a API com os dados do novo endereço
  // encapsulados em um objeto CriarEnderecoRequest. Ele retorna o endereço criado como um
  // objeto EnderecoResponse.
  consultarCep(cep: string): Observable<ViaCepResponseDto | null> {
    const apenasDigitos = cep.replace(/\D/g, '');
    return this.http.get<ApiResponse<ViaCepResponseDto>>(`${this.url}/cep/${apenasDigitos}`)
      .pipe(map(r => r.dados));
  }

  // O método atualizar() envia uma requisição PUT para a API com o ID do endereço a ser atualizado
  // e os dados atualizados encapsulados em um objeto AtualizarEnderecoRequest. Ele retorna o endereço
  // atualizado como um objeto EnderecoResponse.
  criar(request: CriarEnderecoRequest): Observable<EnderecoResponse> {
    return this.http.post<ApiResponse<EnderecoResponse>>(this.url, request)
      .pipe(map(r => r.dados!));
  }

  // O método remover() envia uma requisição DELETE para a API com o ID do endereço a ser removido.
  atualizar(id: string, request: AtualizarEnderecoRequest): Observable<EnderecoResponse> {
    return this.http.put<ApiResponse<EnderecoResponse>>(`${this.url}/${id}`, request)
      .pipe(map(r => r.dados!));
  }

  // O método remover() envia uma requisição DELETE para a API com o ID do endereço a ser removido.    
  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
