//esse script define as interfaces para as respostas da API e os
// objetos de requisição usados no frontend.
export interface ApiResponse<T> {
  sucesso: boolean;
  mensagem: string | null;
  dados: T | null;
}

export interface PessoaFisicaResponse {
  id: string;
  nome: string;
  cpf: string;
  cpfFormatado: string;
  dataNascimento: string;
  email: string;
  telefone: string | null;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm: string | null;
  enderecos: EnderecoResponse[];
}

export interface PessoaJuridicaResponse {
  id: string;
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  cnpjFormatado: string;
  dataAbertura: string;
  email: string;
  telefone: string | null;
  inscricaoEstadual: string | null;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm: string | null;
  enderecos: EnderecoResponse[];
}

export interface EnderecoResponse {
  id: string;
  cep: string;
  cepFormatado: string;
  logradouro: string;
  numero: string;
  complemento: string | null;
  bairro: string;
  cidade: string;
  estado: string;
  pessoaFisicaId: string | null;
  pessoaJuridicaId: string | null;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm: string | null;
}

export interface ViaCepResponseDto {
  cep: string;
  logradouro: string;
  complemento: string;
  bairro: string;
  localidade: string;
  uf: string;
}

export interface CriarPessoaFisicaRequest {
  nome: string;
  cpf: string;
  dataNascimento: string;
  email: string;
  telefone: string | null;
}

export interface AtualizarPessoaFisicaRequest {
  nome: string;
  dataNascimento: string;
  email: string;
  telefone: string | null;
}

export interface CriarPessoaJuridicaRequest {
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  dataAbertura: string;
  email: string;
  telefone: string | null;
  inscricaoEstadual: string | null;
}

export interface AtualizarPessoaJuridicaRequest {
  razaoSocial: string;
  nomeFantasia: string;
  dataAbertura: string;
  email: string;
  telefone: string | null;
  inscricaoEstadual: string | null;
}

export interface CriarEnderecoRequest {
  cep: string;
  logradouro: string;
  numero: string;
  bairro: string;
  cidade: string;
  estado: string;
  complemento: string | null;
  pessoaFisicaId: string | null;
  pessoaJuridicaId: string | null;
}

export interface AtualizarEnderecoRequest {
  cep: string;
  logradouro: string;
  numero: string;
  bairro: string;
  cidade: string;
  estado: string;
  complemento: string | null;
}
