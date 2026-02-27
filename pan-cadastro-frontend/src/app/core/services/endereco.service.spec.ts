import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EnderecoService } from './endereco.service';
import { environment } from '../../../environments/environment';

//esse script contém os testes unitários para o EnderecoService usando o 
// HttpClientTestingModule
describe('EnderecoService', () => {
  let service: EnderecoService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/enderecos`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EnderecoService]
    });
    service = TestBed.inject(EnderecoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  it('consultarCep() deve retornar dados do ViaCEP', () => {
    const mockResponse = {
      sucesso: true,
      mensagem: null,
      dados: { cep: '01001-000', logradouro: 'Praça da Sé', complemento: '', bairro: 'Sé', localidade: 'São Paulo', uf: 'SP' }
    };

    service.consultarCep('01001-000').subscribe(dados => {
      expect(dados).toBeTruthy();
      expect(dados!.localidade).toBe('São Paulo');
      expect(dados!.uf).toBe('SP');
    });

    const req = httpMock.expectOne(`${apiUrl}/cep/01001000`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('consultarCep() deve limpar formatação do CEP', () => {
    const mockResponse = { sucesso: true, mensagem: null, dados: null };

    service.consultarCep('01001-000').subscribe();

    const req = httpMock.expectOne(`${apiUrl}/cep/01001000`);
    expect(req.request.url).toContain('01001000');
    req.flush(mockResponse);
  });

  it('obterPorPessoaFisica() deve buscar endereços por PF', () => {
    const pfId = 'abc-123';
    const mockResponse = {
      sucesso: true,
      mensagem: null,
      dados: [{ id: '1', cep: '01001000', logradouro: 'Rua A', numero: '100', bairro: 'Centro', cidade: 'SP', estado: 'SP', ativo: true }]
    };

    service.obterPorPessoaFisica(pfId).subscribe(enderecos => {
      expect(enderecos.length).toBe(1);
    });

    const req = httpMock.expectOne(`${apiUrl}/pessoa-fisica/${pfId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('criar() deve enviar POST com endereço', () => {
    const request = {
      cep: '01001000', logradouro: 'Rua Teste', numero: '100',
      bairro: 'Centro', cidade: 'SP', estado: 'SP',
      complemento: null, pessoaFisicaId: 'pf-1', pessoaJuridicaId: null
    };
    const mockResponse = { sucesso: true, mensagem: null, dados: { ...request, id: 'end-1' } };

    service.criar(request).subscribe(end => {
      expect(end).toBeTruthy();
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });
});
