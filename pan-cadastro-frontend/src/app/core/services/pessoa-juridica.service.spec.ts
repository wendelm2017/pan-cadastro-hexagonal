import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PessoaJuridicaService } from './pessoa-juridica.service';
import { environment } from '../../../environments/environment';

//esse script contém os testes unitários para o PessoaJuridicaService usando o HttpClientTestingModule. Ele verifica se o serviço é criado corretamente, 
// se os métodos listar() e criar() funcionam como esperado, e se os métodos obterPorId(), atualizar() e remover()
//  enviam as requisições corretas.
describe('PessoaJuridicaService', () => {
  let service: PessoaJuridicaService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/pessoasjuridicas`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PessoaJuridicaService]
    });
    service = TestBed.inject(PessoaJuridicaService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  it('listar() deve retornar lista de PJs', () => {
    const mockResponse = {
      sucesso: true,
      mensagem: null,
      dados: [
        { id: '1', razaoSocial: 'Empresa Teste LTDA', cnpj: '11222333000181', cnpjFormatado: '11.222.333/0001-81', nomeFantasia: 'Teste', email: 'contato@teste.com', ativo: true, enderecos: [] }
      ]
    };

    service.listar().subscribe(empresas => {
      expect(empresas.length).toBe(1);
      expect(empresas[0].razaoSocial).toBe('Empresa Teste LTDA');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('obterPorId() deve retornar PJ por id', () => {
    const mockResponse = {
      sucesso: true,
      mensagem: null,
      dados: { id: '1', razaoSocial: 'Empresa Teste LTDA', cnpj: '11222333000181', cnpjFormatado: '11.222.333/0001-81', nomeFantasia: 'Teste', email: 'contato@teste.com', ativo: true, enderecos: [] }
    };

    service.obterPorId('1').subscribe(empresa => {
      expect(empresa.razaoSocial).toBe('Empresa Teste LTDA');
      expect(empresa.id).toBe('1');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('criar() deve enviar POST e retornar PJ criada', () => {
    const request = { razaoSocial: 'Nova Empresa LTDA', cnpj: '11222333000181', nomeFantasia: 'Nova', email: 'nova@teste.com', telefone: null, inscricaoEstadual: null,dataAbertura: '2024-01-15' };
    const mockResponse = {
      sucesso: true,
      mensagem: 'Criado',
      dados: { id: '2', razaoSocial: 'Nova Empresa LTDA', cnpj: '11222333000181', cnpjFormatado: '11.222.333/0001-81', nomeFantasia: 'Nova', email: 'nova@teste.com', ativo: true, enderecos: [], dataAbertura: '2024-01-15'}
    };

    service.criar(request).subscribe(empresa => {
      expect(empresa.razaoSocial).toBe('Nova Empresa LTDA');
      expect(empresa.id).toBe('2');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush(mockResponse);
  });

  it('atualizar() deve enviar PUT', () => {
    const request = { razaoSocial: 'Empresa Atualizada LTDA', nomeFantasia: 'Atualizada', email: 'atualizada@teste.com', telefone: null, inscricaoEstadual: null,dataAbertura: '2024-01-15' };
    const mockResponse = {
      sucesso: true,
      mensagem: 'Atualizado',
      dados: { id: '1', razaoSocial: 'Empresa Atualizada LTDA', cnpj: '11222333000181', cnpjFormatado: '11.222.333/0001-81', nomeFantasia: 'Atualizada', email: 'atualizada@teste.com', ativo: true, enderecos: [] }
    };

    service.atualizar('1', request).subscribe(empresa => {
      expect(empresa.razaoSocial).toBe('Empresa Atualizada LTDA');
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush(mockResponse);
  });

  it('remover() deve enviar DELETE', () => {
    service.remover('abc-123').subscribe();

    const req = httpMock.expectOne(`${apiUrl}/abc-123`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
