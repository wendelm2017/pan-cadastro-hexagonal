import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { PessoaFisicaService } from '../../core/services/pessoa-fisica.service';
import { EnderecoService } from '../../core/services/endereco.service';
import { PessoaFisicaResponse, EnderecoResponse } from '../../core/models/api.models';
import { UFS } from '../../core/models/constants';
// Tela de PF: tabela + dialogs de edição e endereços
@Component({
  selector: 'app-pessoa-fisica-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    TableModule, ButtonModule, DialogModule, InputTextModule,
    InputMaskModule, CalendarModule, DropdownModule, TooltipModule,
    TagModule, CardModule, ToolbarModule, ConfirmDialogModule
  ],
  template: `
    <p-card>
      <p-toolbar>
        <div class="p-toolbar-group-start">
          <h2 class="m-0"><i class="pi pi-user mr-2"></i>Pessoas Físicas</h2>
        </div>
        <div class="p-toolbar-group-end">
          <p-button label="Nova Pessoa Física" icon="pi pi-plus" (onClick)="abrirDialogNovo()"></p-button>
        </div>
      </p-toolbar>

      <p-table
        [value]="pessoas"
        [paginator]="true"
        [rows]="10"
        [rowsPerPageOptions]="[5, 10, 25]"
        [loading]="carregando"
        [globalFilterFields]="['nome', 'cpfFormatado', 'email']"
        styleClass="p-datatable-striped p-datatable-sm mt-3"
        responsiveLayout="scroll">

        <ng-template pTemplate="header">
          <tr>
            <th pSortableColumn="nome">Nome <p-sortIcon field="nome"></p-sortIcon></th>
            <th>CPF</th>
            <th pSortableColumn="email">E-mail <p-sortIcon field="email"></p-sortIcon></th>
            <th>Telefone</th>
            <th>Endereços</th>
            <th>Status</th>
            <th style="width: 12rem">Ações</th>
          </tr>
        </ng-template>

        <ng-template pTemplate="body" let-pessoa>
          <tr>
            <td>{{ pessoa.nome }}</td>
            <td>{{ pessoa.cpfFormatado }}</td>
            <td>{{ pessoa.email }}</td>
            <td>{{ pessoa.telefone || '-' }}</td>
            <td>
              <p-tag [value]="pessoa.enderecos?.length?.toString() || '0'" severity="info"></p-tag>
            </td>
            <td>
              <p-tag [value]="pessoa.ativo ? 'Ativo' : 'Inativo'" [severity]="pessoa.ativo ? 'success' : 'danger'"></p-tag>
            </td>
            <td>
              <p-button icon="pi pi-pencil" [rounded]="true" [text]="true" severity="info"
                        pTooltip="Editar" (onClick)="abrirDialogEditar(pessoa)" class="mr-1"></p-button>
              <p-button icon="pi pi-map-marker" [rounded]="true" [text]="true" severity="help"
                        pTooltip="Endereços" (onClick)="abrirDialogEnderecos(pessoa)" class="mr-1"></p-button>
              <p-button icon="pi pi-trash" [rounded]="true" [text]="true" severity="danger"
                        pTooltip="Remover" (onClick)="confirmarRemocao(pessoa)"></p-button>
            </td>
          </tr>
        </ng-template>

        <ng-template pTemplate="emptymessage">
          <tr><td colspan="7" class="text-center p-4">Nenhuma pessoa física cadastrada.</td></tr>
        </ng-template>
      </p-table>
    </p-card>

    <!-- Dialog Criar/Editar Pessoa Física -->
    <p-dialog [(visible)]="dialogVisivel" [header]="editando ? 'Editar Pessoa Física' : 'Nova Pessoa Física'"
              [modal]="true" [style]="{width: '650px', overflow: 'visible'}" [draggable]="false" [focusTrap]="false">
      <form [formGroup]="form" (ngSubmit)="salvar()">
        <div class="grid p-fluid mt-2">
          <div class="col-12">
            <label for="nome" class="font-bold block mb-1">Nome *</label>
            <input pInputText id="nome" formControlName="nome" placeholder="Nome completo" />
          </div>
          <div class="col-12 md:col-6">
            <label for="cpf" class="font-bold block mb-1">CPF {{editando ? '(somente leitura)' : '*'}}</label>
            <p-inputMask id="cpf" formControlName="cpf" mask="999.999.999-99" placeholder="000.000.000-00"
                         [styleClass]="editando ? 'p-disabled' : ''"></p-inputMask>
          </div>
          <div class="col-12 md:col-6">
            <label for="dataNascimento" class="font-bold block mb-1">Data Nascimento *</label>
            <p-calendar id="dataNascimento" formControlName="dataNascimento" dateFormat="dd/mm/yy"
                        [showIcon]="true" [maxDate]="hoje" placeholder="dd/mm/aaaa"></p-calendar>
          </div>
          <div class="col-12 md:col-6">
            <label for="email" class="font-bold block mb-1">E-mail *</label>
            <input pInputText id="email" formControlName="email" type="email" placeholder="email@exemplo.com" />
          </div>
          <div class="col-12 md:col-6">
            <label for="telefone" class="font-bold block mb-1">Telefone</label>
            <p-inputMask id="telefone" formControlName="telefone" mask="(99) 99999-9999" placeholder="(00) 00000-0000"></p-inputMask>
          </div>
        </div>
        <div class="flex justify-content-end gap-2 mt-4">
          <p-button label="Cancelar" icon="pi pi-times" severity="secondary" (onClick)="dialogVisivel = false"></p-button>
          <p-button label="Salvar" icon="pi pi-check" type="submit" [disabled]="form.invalid" [loading]="salvando"></p-button>
        </div>
      </form>
    </p-dialog>

    <!-- Dialog Endereços -->
    <p-dialog [(visible)]="dialogEnderecosVisivel" header="Endereços"
              [modal]="true" [style]="{width: '750px'}" [draggable]="false" [focusTrap]="false">

      <p-toolbar styleClass="mb-3">
        <div class="p-toolbar-group-start">
          <span class="font-semibold">{{ pessoaSelecionada?.nome }}</span>
        </div>
        <div class="p-toolbar-group-end">
          <p-button label="Novo Endereço" icon="pi pi-plus" size="small" (onClick)="abrirFormEndereco()"></p-button>
        </div>
      </p-toolbar>

      <!-- Form novo/editar endereço -->
      <div [hidden]="!formEnderecoVisivel" class="surface-100 p-3 border-round mb-3">
        <form [formGroup]="formEndereco" (ngSubmit)="salvarEndereco()">
          <div class="grid p-fluid">
            <div class="col-12 md:col-4">
              <label class="font-bold block mb-1">CEP *</label>
              <div class="p-inputgroup">
                <input pInputText formControlName="cep" placeholder="00000-000" maxlength="9" />
                <button type="button" pButton icon="pi pi-search" pTooltip="Buscar CEP"
                        (click)="consultarCep()" [loading]="buscandoCep"></button>
              </div>
            </div>
            <div class="col-12 md:col-6">
              <label class="font-bold block mb-1">Logradouro *</label>
              <input pInputText formControlName="logradouro" />
            </div>
            <div class="col-12 md:col-2">
              <label class="font-bold block mb-1">Número *</label>
              <input pInputText formControlName="numero" />
            </div>
            <div class="col-12 md:col-4">
              <label class="font-bold block mb-1">Complemento</label>
              <input pInputText formControlName="complemento" />
            </div>
            <div class="col-12 md:col-3">
              <label class="font-bold block mb-1">Bairro *</label>
              <input pInputText formControlName="bairro" />
            </div>
            <div class="col-12 md:col-3">
              <label class="font-bold block mb-1">Cidade *</label>
              <input pInputText formControlName="cidade" />
            </div>
            <div class="col-12 md:col-2">
              <label class="font-bold block mb-1">UF *</label>
              <p-dropdown formControlName="estado" [options]="ufs" placeholder="UF"></p-dropdown>
            </div>
            <div class="col-12" *ngIf="cepDetalhes">
              <a class="text-sm cursor-pointer no-underline text-primary" (click)="cepDetalhesAberto = !cepDetalhesAberto">
                <i class="pi" [ngClass]="cepDetalhesAberto ? 'pi-chevron-down' : 'pi-chevron-right'"></i>
                Detalhes do CEP
              </a>
              <div *ngIf="cepDetalhesAberto" class="surface-200 border-round p-2 text-sm flex align-items-center gap-3 mt-1">
                <span><i class="pi pi-map-marker mr-1"></i>{{ cepDetalhes.estado }} — {{ cepDetalhes.regiao }}</span>
                <span *ngIf="cepDetalhes.ddd">DDD: {{ cepDetalhes.ddd }}</span>
                <span *ngIf="cepDetalhes.ibge">IBGE: {{ cepDetalhes.ibge }}</span>
                <span *ngIf="cepDetalhes.siafi">SIAFI: {{ cepDetalhes.siafi }}</span>
              </div>
            </div>
          </div>
          <div class="flex justify-content-end gap-2 mt-3">
            <p-button label="Cancelar" size="small" severity="secondary" (onClick)="formEnderecoVisivel = false"></p-button>
            <p-button label="Salvar Endereço" size="small" icon="pi pi-check" type="submit"
                      [disabled]="formEndereco.invalid" [loading]="salvandoEndereco"></p-button>
          </div>
        </form>
      </div>

      <!-- Lista de endereços -->
      <p-table [value]="enderecosDaPessoa" styleClass="p-datatable-sm" responsiveLayout="scroll">
        <ng-template pTemplate="header">
          <tr>
            <th>CEP</th>
            <th>Logradouro</th>
            <th>Nº</th>
            <th>Bairro</th>
            <th>Cidade/UF</th>
            <th style="width: 6rem">Ações</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-end>
          <tr>
            <td>{{ end.cepFormatado }}</td>
            <td>{{ end.logradouro }}</td>
            <td>{{ end.numero }}</td>
            <td>{{ end.bairro }}</td>
            <td>{{ end.cidade }}/{{ end.estado }}</td>
            <td>
              <p-button icon="pi pi-trash" [rounded]="true" [text]="true" severity="danger" size="small"
                        (onClick)="removerEndereco(end)"></p-button>
            </td>
          </tr>
        </ng-template>
        <ng-template pTemplate="emptymessage">
          <tr><td colspan="6" class="text-center p-3">Nenhum endereço cadastrado.</td></tr>
        </ng-template>
      </p-table>
    </p-dialog>
  `
})
export class PessoaFisicaListComponent implements OnInit {
  pessoas: PessoaFisicaResponse[] = [];
  carregando = false;
  salvando = false;
  dialogVisivel = false;
  editando = false;
  pessoaEditandoId: string | null = null;
  hoje = new Date();
  form!: FormGroup;

  // Endereços
  dialogEnderecosVisivel = false;
  formEnderecoVisivel = false;
  pessoaSelecionada: PessoaFisicaResponse | null = null;
  enderecosDaPessoa: EnderecoResponse[] = [];
  formEndereco!: FormGroup;
  buscandoCep = false;
  salvandoEndereco = false;
  cepDetalhes: any = null;
  cepDetalhesAberto = false;
  ufs = UFS;

  constructor(
    private fb: FormBuilder,
    private pessoaService: PessoaFisicaService,
    private enderecoService: EnderecoService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.carregarPessoas();
    this.initForm();
    this.initFormEndereco();
  }

  private initForm(): void {
    this.form = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      cpf: ['', Validators.required],
      dataNascimento: [null, Validators.required],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['']
    });
  }

  private initFormEndereco(): void {
    this.formEndereco = this.fb.group({
      cep: ['', Validators.required],
      logradouro: ['', Validators.required],
      numero: ['', Validators.required],
      complemento: [''],
      bairro: ['', Validators.required],
      cidade: ['', Validators.required],
      estado: ['', Validators.required]
    });
  }

  carregarPessoas(): void {
    this.carregando = true;
    this.pessoaService.listar().subscribe({
      next: (dados) => { this.pessoas = dados; this.carregando = false; },
      error: () => { this.carregando = false; }
    });
  }

  abrirDialogNovo(): void {
    this.editando = false;
    this.pessoaEditandoId = null;
    this.form.reset();
    this.form.get('cpf')?.enable();
    this.dialogVisivel = true;
  }

  abrirDialogEditar(pessoa: PessoaFisicaResponse): void {
    this.editando = true;
    this.pessoaEditandoId = pessoa.id;
    this.form.patchValue({
      nome: pessoa.nome,
      cpf: pessoa.cpf,
      dataNascimento: new Date(pessoa.dataNascimento),
      email: pessoa.email,
      telefone: pessoa.telefone
    });
    this.form.get('cpf')?.disable();
    this.dialogVisivel = true;
  }

  salvar(): void {
    if (this.form.invalid) return;
    this.salvando = true;
    const val = this.form.getRawValue();
    const dataNasc = val.dataNascimento instanceof Date
      ? val.dataNascimento.toISOString().split('T')[0]
      : val.dataNascimento;

    if (this.editando && this.pessoaEditandoId) {
      this.pessoaService.atualizar(this.pessoaEditandoId, {
        nome: val.nome, dataNascimento: dataNasc,
        email: val.email, telefone: val.telefone || null
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Pessoa Física atualizada.' });
          this.dialogVisivel = false;
          this.salvando = false;
          this.carregarPessoas();
        },
        error: () => { this.salvando = false; }
      });
    } else {
      this.pessoaService.criar({
        nome: val.nome, cpf: val.cpf, dataNascimento: dataNasc,
        email: val.email, telefone: val.telefone || null
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Pessoa Física criada.' });
          this.dialogVisivel = false;
          this.salvando = false;
          this.carregarPessoas();
        },
        error: () => { this.salvando = false; }
      });
    }
  }

  confirmarRemocao(pessoa: PessoaFisicaResponse): void {
    this.confirmationService.confirm({
      message: `Deseja remover "${pessoa.nome}"?`,
      header: 'Confirmar Remoção',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, remover',
      rejectLabel: 'Cancelar',
      accept: () => {
        this.pessoaService.remover(pessoa.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Removido', detail: 'Pessoa Física removida.' });
            this.carregarPessoas();
          }
        });
      }
    });
  }

  // === ENDEREÇOS ===

  abrirDialogEnderecos(pessoa: PessoaFisicaResponse): void {
    this.pessoaSelecionada = pessoa;
    this.enderecosDaPessoa = pessoa.enderecos || [];
    this.formEnderecoVisivel = false;
    this.dialogEnderecosVisivel = true;
    this.carregarEnderecos(pessoa.id);
  }

  carregarEnderecos(pessoaId: string): void {
    this.enderecoService.obterPorPessoaFisica(pessoaId).subscribe({
      next: (dados) => { this.enderecosDaPessoa = dados; }
    });
  }

  abrirFormEndereco(): void {
    this.formEndereco.reset();
    this.cepDetalhes = null;
    this.cepDetalhesAberto = false;
    this.formEnderecoVisivel = true;
  }

  consultarCep(): void {
    const cep = this.formEndereco.get('cep')?.value;
    if (!cep) return;
    this.buscandoCep = true;
    this.enderecoService.consultarCep(cep).subscribe({
      next: (dados) => {
        this.buscandoCep = false;
        if (dados) {
          this.formEndereco.patchValue({
            logradouro: dados.logradouro,
            bairro: dados.bairro,
            cidade: dados.localidade,
            estado: dados.uf
          });
          this.cepDetalhes = dados;
          this.messageService.add({ severity: 'info', summary: 'CEP encontrado', detail: `${dados.localidade}/${dados.uf}` });
        }
      },
      error: () => { this.buscandoCep = false; }
    });
  }

  salvarEndereco(): void {
    if (this.formEndereco.invalid || !this.pessoaSelecionada) return;
    this.salvandoEndereco = true;
    const val = this.formEndereco.value;
    this.enderecoService.criar({
      cep: val.cep, logradouro: val.logradouro, numero: val.numero,
      bairro: val.bairro, cidade: val.cidade, estado: val.estado,
      complemento: val.complemento || null,
      pessoaFisicaId: this.pessoaSelecionada.id,
      pessoaJuridicaId: null
    }).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Endereço criado.' });
        this.formEnderecoVisivel = false;
        this.salvandoEndereco = false;
        this.carregarEnderecos(this.pessoaSelecionada!.id);
        this.carregarPessoas();
      },
      error: () => { this.salvandoEndereco = false; }
    });
  }

  removerEndereco(endereco: EnderecoResponse): void {
    this.confirmationService.confirm({
      message: `Remover endereço ${endereco.logradouro}, ${endereco.numero}?`,
      header: 'Confirmar',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.enderecoService.remover(endereco.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Removido', detail: 'Endereço removido.' });
            this.carregarEnderecos(this.pessoaSelecionada!.id);
            this.carregarPessoas();
          }
        });
      }
    });
  }
}
