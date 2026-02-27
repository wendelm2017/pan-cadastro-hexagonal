import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PessoaFisicaService } from './pessoa-fisica.service';
import { environment } from '../../../environments/environment';

//esse script contém os testes unitários para o PessoaFisicaService usando o 
// HttpClientTestingModule. Ele verifica se o serviço é criado corretamente, 
// se os métodos listar() e criar() funcionam como esperado, e se o método remover()
//  envia a requisição DELETE correta.
describe('PessoaFisicaService', () => {
  let service: PessoaFisicaService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/pessoasfisicas`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PessoaFisicaService]
    });
    service = TestBed.inject(PessoaFisicaService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  it('listar() deve retornar lista de PFs', () => {
    const mockResponse = {
      sucesso: true,
      mensagem: null,
      dados: [
        { id: '1', nome: 'João', cpf: '52998224725', cpfFormatado: '529.982.247-25', email: 'joao@test.com', ativo: true, enderecos: [] }
      ]
    };

    service.listar().subscribe(pessoas => {
      expect(pessoas.length).toBe(1);
      expect(pessoas[0].nome).toBe('João');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('criar() deve enviar POST e retornar PF criada', () => {
    const request = { nome: 'Maria', cpf: '39053344705', dataNascimento: '1990-01-01', email: 'maria@test.com', telefone: null };
    const mockResponse = {
      sucesso: true,
      mensagem: 'Criado',
      dados: { id: '2', nome: 'Maria', cpf: '39053344705', cpfFormatado: '390.533.447-05', email: 'maria@test.com', ativo: true, enderecos: [] }
    };

    service.criar(request).subscribe(pessoa => {
      expect(pessoa.nome).toBe('Maria');
      expect(pessoa.id).toBe('2');
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush(mockResponse);
  });

  it('remover() deve enviar DELETE', () => {
    service.remover('abc-123').subscribe();

    const req = httpMock.expectOne(`${apiUrl}/abc-123`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
