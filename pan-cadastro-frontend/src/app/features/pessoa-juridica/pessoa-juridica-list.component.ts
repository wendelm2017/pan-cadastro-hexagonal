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
import { PessoaJuridicaService } from '../../core/services/pessoa-juridica.service';
import { EnderecoService } from '../../core/services/endereco.service';
import { PessoaJuridicaResponse, EnderecoResponse } from '../../core/models/api.models';
import { UFS } from '../../core/models/constants';
// Tela de PJ: tabela + dialogs
@Component({
  selector: 'app-pessoa-juridica-list',
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
          <h2 class="m-0"><i class="pi pi-briefcase mr-2"></i>Pessoas Jurídicas</h2>
        </div>
        <div class="p-toolbar-group-end">
          <p-button label="Nova Pessoa Jurídica" icon="pi pi-plus" (onClick)="abrirDialogNovo()"></p-button>
        </div>
      </p-toolbar>

      <p-table
        [value]="empresas"
        [paginator]="true"
        [rows]="10"
        [rowsPerPageOptions]="[5, 10, 25]"
        [loading]="carregando"
        styleClass="p-datatable-striped p-datatable-sm mt-3"
        responsiveLayout="scroll">

        <ng-template pTemplate="header">
          <tr>
            <th pSortableColumn="razaoSocial">Razão Social <p-sortIcon field="razaoSocial"></p-sortIcon></th>
            <th>Nome Fantasia</th>
            <th>CNPJ</th>
            <th pSortableColumn="email">E-mail <p-sortIcon field="email"></p-sortIcon></th>
            <th>Endereços</th>
            <th>Status</th>
            <th style="width: 12rem">Ações</th>
          </tr>
        </ng-template>

        <ng-template pTemplate="body" let-empresa>
          <tr>
            <td>{{ empresa.razaoSocial }}</td>
            <td>{{ empresa.nomeFantasia }}</td>
            <td>{{ empresa.cnpjFormatado }}</td>
            <td>{{ empresa.email }}</td>
            <td>
              <p-tag [value]="empresa.enderecos?.length?.toString() || '0'" severity="info"></p-tag>
            </td>
            <td>
              <p-tag [value]="empresa.ativo ? 'Ativo' : 'Inativo'" [severity]="empresa.ativo ? 'success' : 'danger'"></p-tag>
            </td>
            <td>
              <p-button icon="pi pi-pencil" [rounded]="true" [text]="true" severity="info"
                        pTooltip="Editar" (onClick)="abrirDialogEditar(empresa)" class="mr-1"></p-button>
              <p-button icon="pi pi-map-marker" [rounded]="true" [text]="true" severity="help"
                        pTooltip="Endereços" (onClick)="abrirDialogEnderecos(empresa)" class="mr-1"></p-button>
              <p-button icon="pi pi-trash" [rounded]="true" [text]="true" severity="danger"
                        pTooltip="Remover" (onClick)="confirmarRemocao(empresa)"></p-button>
            </td>
          </tr>
        </ng-template>

        <ng-template pTemplate="emptymessage">
          <tr><td colspan="7" class="text-center p-4">Nenhuma pessoa jurídica cadastrada.</td></tr>
        </ng-template>
      </p-table>
    </p-card>

    <!-- Dialog Criar/Editar PJ -->
    <p-dialog [(visible)]="dialogVisivel" [header]="editando ? 'Editar Pessoa Jurídica' : 'Nova Pessoa Jurídica'"
              [modal]="true" [style]="{width: '600px'}" [draggable]="false" [focusTrap]="false">
      <form [formGroup]="form" (ngSubmit)="salvar()">
        <div class="grid p-fluid mt-2">
          <div class="col-12">
            <label class="font-bold block mb-1">Razão Social *</label>
            <input pInputText formControlName="razaoSocial" placeholder="Razão Social completa" />
          </div>
          <div class="col-12 md:col-6">
            <label class="font-bold block mb-1">Nome Fantasia *</label>
            <input pInputText formControlName="nomeFantasia" placeholder="Nome Fantasia" />
          </div>
          <div class="col-12 md:col-6">
            <label class="font-bold block mb-1">CNPJ {{editando ? '(somente leitura)' : '*'}}</label>
            <p-inputMask formControlName="cnpj" mask="99.999.999/9999-99" placeholder="00.000.000/0000-00"
                         [styleClass]="editando ? 'p-disabled' : ''"></p-inputMask>
          </div>
          <div class="col-12 md:col-6">
            <label class="font-bold block mb-1">Data Abertura *</label>
            <p-calendar formControlName="dataAbertura" dateFormat="dd/mm/yy"
                        [showIcon]="true" [maxDate]="hoje" placeholder="dd/mm/aaaa"></p-calendar>
          </div>
          <div class="col-12 md:col-6">
            <label class="font-bold block mb-1">E-mail *</label>
            <input pInputText formControlName="email" type="email" placeholder="contato@empresa.com" />
          </div>
          <div class="col-12 md:col-6">
            <label class="font-bold block mb-1">Telefone</label>
            <p-inputMask formControlName="telefone" mask="(99) 9999-9999" placeholder="(00) 0000-0000"></p-inputMask>
          </div>
          <div class="col-12 md:col-6">
            <label class="font-bold block mb-1">Inscrição Estadual</label>
            <input pInputText formControlName="inscricaoEstadual" placeholder="Opcional" />
          </div>
        </div>
        <div class="flex justify-content-end gap-2 mt-4">
          <p-button label="Cancelar" icon="pi pi-times" severity="secondary" (onClick)="dialogVisivel = false"></p-button>
          <p-button label="Salvar" icon="pi pi-check" type="submit" [disabled]="form.invalid" [loading]="salvando"></p-button>
        </div>
      </form>
    </p-dialog>

    <!-- Dialog Endereços (reutilizado) -->
    <p-dialog [(visible)]="dialogEnderecosVisivel" header="Endereços"
              [modal]="true" [style]="{width: '750px'}" [draggable]="false" [focusTrap]="false">
      <p-toolbar styleClass="mb-3">
        <div class="p-toolbar-group-start">
          <span class="font-semibold">{{ empresaSelecionada?.nomeFantasia }}</span>
        </div>
        <div class="p-toolbar-group-end">
          <p-button label="Novo Endereço" icon="pi pi-plus" size="small" (onClick)="abrirFormEndereco()"></p-button>
        </div>
      </p-toolbar>

      <div [hidden]="!formEnderecoVisivel" class="surface-100 p-3 border-round mb-3">
        <div class="font-semibold mb-2">{{ enderecoEditandoId ? 'Editar Endereço' : 'Novo Endereço' }}</div>
        <form [formGroup]="formEndereco" (ngSubmit)="salvarEndereco()">
          <div class="grid p-fluid">
            <div class="col-12 md:col-4">
              <label class="font-bold block mb-1">CEP *</label>
              <div class="p-inputgroup">
                <input pInputText formControlName="cep" placeholder="00000-000" maxlength="9" />
                <button type="button" pButton icon="pi pi-search" (click)="consultarCep()" [loading]="buscandoCep"></button>
              </div>
            </div>
            <div class="col-12 md:col-6">
              <label class="font-bold block mb-1">Logradouro *</label>
              <input pInputText formControlName="logradouro" />
            </div>
            <div class="col-12 md:col-2">
              <label class="font-bold block mb-1">Nº *</label>
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
            <p-button label="Cancelar" size="small" severity="secondary" (onClick)="cancelarFormEndereco()"></p-button>
            <p-button [label]="enderecoEditandoId ? 'Atualizar Endereço' : 'Salvar Endereço'" size="small" icon="pi pi-check" type="submit"
                      [disabled]="formEndereco.invalid" [loading]="salvandoEndereco"></p-button>
          </div>
        </form>
      </div>

      <p-table [value]="enderecosDaEmpresa" styleClass="p-datatable-sm" responsiveLayout="scroll">
        <ng-template pTemplate="header">
          <tr>
            <th>CEP</th><th>Logradouro</th><th>Nº</th><th>Bairro</th><th>Cidade/UF</th><th style="width:8rem">Ações</th>
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
              <p-button icon="pi pi-pencil" [rounded]="true" [text]="true" severity="info" size="small"
                        pTooltip="Editar" (onClick)="editarEndereco(end)" class="mr-1"></p-button>
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
export class PessoaJuridicaListComponent implements OnInit {
  empresas: PessoaJuridicaResponse[] = [];
  carregando = false;
  salvando = false;
  dialogVisivel = false;
  editando = false;
  empresaEditandoId: string | null = null;
  hoje = new Date();
  form!: FormGroup;

  // Endereços
  dialogEnderecosVisivel = false;
  formEnderecoVisivel = false;
  empresaSelecionada: PessoaJuridicaResponse | null = null;
  enderecosDaEmpresa: EnderecoResponse[] = [];
  formEndereco!: FormGroup;
  buscandoCep = false;
  salvandoEndereco = false;
  enderecoEditandoId: string | null = null;
  cepDetalhes: any = null;
  cepDetalhesAberto = false;
  ufs = UFS;

  constructor(
    private fb: FormBuilder,
    private empresaService: PessoaJuridicaService,
    private enderecoService: EnderecoService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.carregarEmpresas();
    this.initForm();
    this.initFormEndereco();
  }

  private initForm(): void {
    this.form = this.fb.group({
      razaoSocial: ['', [Validators.required, Validators.minLength(2)]],
      nomeFantasia: ['', [Validators.required, Validators.minLength(2)]],
      cnpj: ['', Validators.required],
      dataAbertura: [null, Validators.required],
      email: ['', [Validators.required, Validators.email]],
      telefone: [''],
      inscricaoEstadual: ['']
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

  carregarEmpresas(): void {
    this.carregando = true;
    this.empresaService.listar().subscribe({
      next: (dados) => { this.empresas = dados; this.carregando = false; },
      error: () => { this.carregando = false; }
    });
  }

  abrirDialogNovo(): void {
    this.editando = false;
    this.empresaEditandoId = null;
    this.form.reset();
    this.form.get('cnpj')?.enable();
    this.dialogVisivel = true;
  }

  abrirDialogEditar(empresa: PessoaJuridicaResponse): void {
    this.editando = true;
    this.empresaEditandoId = empresa.id;
    this.form.patchValue({
      razaoSocial: empresa.razaoSocial,
      nomeFantasia: empresa.nomeFantasia,
      cnpj: empresa.cnpj,
      dataAbertura: new Date(empresa.dataAbertura),
      email: empresa.email,
      telefone: empresa.telefone,
      inscricaoEstadual: empresa.inscricaoEstadual
    });
    this.form.get('cnpj')?.disable();
    this.dialogVisivel = true;
  }

  salvar(): void {
    if (this.form.invalid) return;
    this.salvando = true;
    const val = this.form.getRawValue();
    const dataAbert = val.dataAbertura instanceof Date
      ? val.dataAbertura.toISOString().split('T')[0]
      : val.dataAbertura;

    if (this.editando && this.empresaEditandoId) {
      this.empresaService.atualizar(this.empresaEditandoId, {
        razaoSocial: val.razaoSocial, nomeFantasia: val.nomeFantasia,
        dataAbertura: dataAbert, email: val.email,
        telefone: val.telefone || null, inscricaoEstadual: val.inscricaoEstadual || null
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Pessoa Jurídica atualizada.' });
          this.dialogVisivel = false; this.salvando = false;
          this.carregarEmpresas();
        },
        error: () => { this.salvando = false; }
      });
    } else {
      this.empresaService.criar({
        razaoSocial: val.razaoSocial, nomeFantasia: val.nomeFantasia,
        cnpj: val.cnpj, dataAbertura: dataAbert, email: val.email,
        telefone: val.telefone || null, inscricaoEstadual: val.inscricaoEstadual || null
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Pessoa Jurídica criada.' });
          this.dialogVisivel = false; this.salvando = false;
          this.carregarEmpresas();
        },
        error: () => { this.salvando = false; }
      });
    }
  }

  confirmarRemocao(empresa: PessoaJuridicaResponse): void {
    this.confirmationService.confirm({
      message: `Deseja remover "${empresa.nomeFantasia}"?`,
      header: 'Confirmar Remoção',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.empresaService.remover(empresa.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Removido', detail: 'Pessoa Jurídica removida.' });
            this.carregarEmpresas();
          }
        });
      }
    });
  }

  // === ENDEREÇOS ===

  abrirDialogEnderecos(empresa: PessoaJuridicaResponse): void {
    this.empresaSelecionada = empresa;
    this.enderecosDaEmpresa = empresa.enderecos || [];
    this.formEnderecoVisivel = false;
    this.enderecoEditandoId = null;
    this.dialogEnderecosVisivel = true;
    this.carregarEnderecos(empresa.id);
  }

  carregarEnderecos(empresaId: string): void {
    this.enderecoService.obterPorPessoaJuridica(empresaId).subscribe({
      next: (dados) => { this.enderecosDaEmpresa = dados; }
    });
  }

  abrirFormEndereco(): void {
    this.enderecoEditandoId = null;
    this.formEndereco.reset();
    this.cepDetalhes = null;
    this.cepDetalhesAberto = false;
    this.formEnderecoVisivel = true;
  }

  editarEndereco(endereco: EnderecoResponse): void {
    this.enderecoEditandoId = endereco.id;
    this.formEndereco.patchValue({
      cep: endereco.cep,
      logradouro: endereco.logradouro,
      numero: endereco.numero,
      complemento: endereco.complemento,
      bairro: endereco.bairro,
      cidade: endereco.cidade,
      estado: endereco.estado
    });
    this.cepDetalhes = null;
    this.cepDetalhesAberto = false;
    this.formEnderecoVisivel = true;
  }

  cancelarFormEndereco(): void {
    this.enderecoEditandoId = null;
    this.formEnderecoVisivel = false;
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
            logradouro: dados.logradouro, bairro: dados.bairro,
            cidade: dados.localidade, estado: dados.uf
          });
          this.cepDetalhes = dados;
          this.messageService.add({ severity: 'info', summary: 'CEP encontrado', detail: `${dados.localidade}/${dados.uf}` });
        } else {
          this.messageService.add({ severity: 'warn', summary: 'CEP', detail: 'CEP não encontrado. Verifique o número digitado.', life: 4000 });
        }
      }
    });
  }

  salvarEndereco(): void {
    if (this.formEndereco.invalid || !this.empresaSelecionada) return;
    this.salvandoEndereco = true;
    const val = this.formEndereco.value;

    if (this.enderecoEditandoId) {
      this.enderecoService.atualizar(this.enderecoEditandoId, {
        cep: val.cep, logradouro: val.logradouro, numero: val.numero,
        bairro: val.bairro, cidade: val.cidade, estado: val.estado,
        complemento: val.complemento || null
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Endereço atualizado.' });
          this.formEnderecoVisivel = false;
          this.salvandoEndereco = false;
          this.enderecoEditandoId = null;
          this.carregarEnderecos(this.empresaSelecionada!.id);
          this.carregarEmpresas();
        },
        error: () => { this.salvandoEndereco = false; }
      });
    } else {
      this.enderecoService.criar({
        cep: val.cep, logradouro: val.logradouro, numero: val.numero,
        bairro: val.bairro, cidade: val.cidade, estado: val.estado,
        complemento: val.complemento || null,
        pessoaFisicaId: null,
        pessoaJuridicaId: this.empresaSelecionada.id
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Endereço criado.' });
          this.formEnderecoVisivel = false;
          this.salvandoEndereco = false;
          this.carregarEnderecos(this.empresaSelecionada!.id);
          this.carregarEmpresas();
        },
        error: () => { this.salvandoEndereco = false; }
      });
    }
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
            this.carregarEnderecos(this.empresaSelecionada!.id);
            this.carregarEmpresas();
          }
        });
      }
    });
  }
}
