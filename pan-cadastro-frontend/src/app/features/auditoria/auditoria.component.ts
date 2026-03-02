import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { TooltipModule } from 'primeng/tooltip';
import { ToolbarModule } from 'primeng/toolbar';
import { DialogModule } from 'primeng/dialog';
import { MessageService } from 'primeng/api';
import { AuditoriaService, LogEntry, LogResumo } from '../../core/services/auditoria.service';
//template angular para o componente de auditoria, que exibe uma tabela de logs com filtros
// Tela de auditoria: logs do Serilog com filtros
@Component({
  selector: 'app-auditoria',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    TableModule, CardModule, TagModule, ButtonModule,
    InputTextModule, DropdownModule, CalendarModule,
    TooltipModule, ToolbarModule, DialogModule
  ],
  template: `
    <p-card>
      <p-toolbar>
        <div class="p-toolbar-group-start">
          <h2 class="m-0"><i class="pi pi-list mr-2"></i>Auditoria & Logs</h2>
        </div>
        <div class="p-toolbar-group-end">
          <p-button icon="pi pi-refresh" label="Atualizar" severity="secondary"
                    (onClick)="carregarDados()" [loading]="carregando" class="mr-2"></p-button>
        </div>
      </p-toolbar>

      <!-- Cards Resumo -->
      <div class="grid mt-3" *ngIf="resumo">
        <div class="col-6 md:col-3">
          <div class="surface-card shadow-1 p-3 border-round text-center">
            <div class="text-3xl font-bold text-blue-500">{{ resumo.info }}</div>
            <div class="text-500 mt-1"><i class="pi pi-info-circle mr-1"></i>Info</div>
          </div>
        </div>
        <div class="col-6 md:col-3">
          <div class="surface-card shadow-1 p-3 border-round text-center">
            <div class="text-3xl font-bold text-orange-500">{{ resumo.warning }}</div>
            <div class="text-500 mt-1"><i class="pi pi-exclamation-triangle mr-1"></i>Warning</div>
          </div>
        </div>
        <div class="col-6 md:col-3">
          <div class="surface-card shadow-1 p-3 border-round text-center">
            <div class="text-3xl font-bold text-red-500">{{ resumo.error }}</div>
            <div class="text-500 mt-1"><i class="pi pi-times-circle mr-1"></i>Error</div>
          </div>
        </div>
        <div class="col-6 md:col-3">
          <div class="surface-card shadow-1 p-3 border-round text-center">
            <div class="text-3xl font-bold text-purple-500">{{ resumo.fatal }}</div>
            <div class="text-500 mt-1"><i class="pi pi-ban mr-1"></i>Fatal</div>
          </div>
        </div>
      </div>

      <!-- Filtros -->
      <div class="grid mt-3 align-items-end">
        <div class="col-12 md:col-3">
          <label class="font-bold block mb-1">Nível</label>
          <p-dropdown [options]="niveisOptions" [(ngModel)]="filtroNivel"
                      placeholder="Todos" [showClear]="true"
                      (onChange)="carregarDados()"></p-dropdown>
        </div>
        <div class="col-12 md:col-3">
          <label class="font-bold block mb-1">Data</label>
          <p-dropdown [options]="datasOptions" [(ngModel)]="filtroData"
                      placeholder="Hoje" [showClear]="true"
                      (onChange)="carregarDados()"></p-dropdown>
        </div>
        <div class="col-12 md:col-4">
          <label class="font-bold block mb-1">Busca</label>
          <div class="p-inputgroup">
            <input pInputText [(ngModel)]="filtroBusca" placeholder="Buscar nas mensagens..."
                   (keyup.enter)="carregarDados()" />
            <button pButton icon="pi pi-search" (click)="carregarDados()"></button>
          </div>
        </div>
        <div class="col-12 md:col-2">
          <p-button label="Limpar" icon="pi pi-filter-slash" severity="secondary"
                    [text]="true" (onClick)="limparFiltros()"></p-button>
        </div>
      </div>

      <!-- Tabela de Logs -->
      <p-table [value]="registros" [paginator]="true" [rows]="25"
               [rowsPerPageOptions]="[10, 25, 50, 100]"
               [loading]="carregando"
               styleClass="p-datatable-sm p-datatable-striped mt-3"
               [globalFilterFields]="['mensagem', 'origem']"
               [scrollable]="true" scrollHeight="500px">
        <ng-template pTemplate="header">
          <tr>
            <th style="width:80px">Hora</th>
            <th style="width:80px">Nível</th>
            <th style="width:120px">Origem</th>
            <th>Mensagem</th>
            <th style="width:60px">Detalhe</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-log>
          <tr [ngClass]="{'bg-red-50': log.nivel === 'ERR' || log.nivel === 'FTL',
                          'bg-orange-50': log.nivel === 'WRN'}">
            <td class="font-mono text-sm">{{ log.timestamp }}</td>
            <td>
              <p-tag [value]="log.nivel" [severity]="$any(getSeverity(log.nivel))"
                     [icon]="getIcon(log.nivel)"></p-tag>
            </td>
            <td class="text-sm">{{ log.origem }}</td>
            <td class="text-sm" style="word-break: break-word;">{{ log.mensagem | slice:0:200 }}</td>
            <td class="text-center">
              <p-button *ngIf="log.detalhes" icon="pi pi-eye" [rounded]="true" [text]="true"
                        severity="info" pTooltip="Ver detalhes"
                        (onClick)="verDetalhes(log)"></p-button>
            </td>
          </tr>
        </ng-template>
        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="5" class="text-center p-4">
              <i class="pi pi-check-circle text-green-500 text-2xl mb-2" style="display:block"></i>
              Nenhum registro encontrado para os filtros selecionados.
            </td>
          </tr>
        </ng-template>
        <ng-template pTemplate="summary">
          <div class="flex justify-content-between">
            <span>Total: <strong>{{ registros.length }}</strong> registros</span>
            <span class="text-500">Data: {{ dataAtual }}</span>
          </div>
        </ng-template>
      </p-table>
    </p-card>

    <!-- Dialog Detalhes -->
    <p-dialog [(visible)]="dialogDetalhes" header="Detalhes do Log"
              [modal]="true" [style]="{width: '700px'}" [draggable]="false">
      <div *ngIf="logSelecionado">
        <div class="grid">
          <div class="col-4"><strong>Hora:</strong></div>
          <div class="col-8 font-mono">{{ logSelecionado.timestamp }}</div>
          <div class="col-4"><strong>Nível:</strong></div>
          <div class="col-8">
            <p-tag [value]="logSelecionado.nivel" [severity]="$any(getSeverity(logSelecionado.nivel))"></p-tag>
          </div>
          <div class="col-4"><strong>Origem:</strong></div>
          <div class="col-8">{{ logSelecionado.origem }}</div>
          <div class="col-12"><strong>Mensagem:</strong></div>
          <div class="col-12 surface-100 p-2 border-round font-mono text-sm" style="word-break: break-word;">
            {{ logSelecionado.mensagem }}
          </div>
          <div class="col-12" *ngIf="logSelecionado.detalhes">
            <strong>Stack Trace / Detalhes:</strong>
          </div>
          <div class="col-12 surface-900 text-white p-3 border-round font-mono text-xs"
               *ngIf="logSelecionado.detalhes"
               style="white-space: pre-wrap; max-height: 300px; overflow-y: auto;">{{ logSelecionado.detalhes }}</div>
        </div>
      </div>
    </p-dialog>
  `,
  styles: [`
    :host ::ng-deep .p-datatable .p-datatable-tbody > tr.bg-red-50 { background: #fff5f5 !important; }
    :host ::ng-deep .p-datatable .p-datatable-tbody > tr.bg-orange-50 { background: #fff8f0 !important; }
    .font-mono { font-family: 'Consolas', 'Monaco', 'Courier New', monospace; }
  `]
})
export class AuditoriaComponent implements OnInit {

  registros: LogEntry[] = [];
  resumo: LogResumo | null = null;
  carregando = false;

  filtroNivel: string | null = null;
  filtroBusca = '';
  filtroData: string | null = null;
  dataAtual = '';

  datasOptions: any[] = [];

  niveisOptions = [
    { label: 'Info', value: 'INF' },
    { label: 'Warning', value: 'WRN' },
    { label: 'Error', value: 'ERR' },
    { label: 'Fatal', value: 'FTL' }
  ];

  dialogDetalhes = false;
  logSelecionado: LogEntry | null = null;

  constructor(
    private service: AuditoriaService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.carregarDados();
  }

  carregarDados(): void {
    this.carregando = true;

    const filtros = {
      nivel: this.filtroNivel || undefined,
      busca: this.filtroBusca || undefined,
      data: this.filtroData || undefined,
      limite: 500
    };

    // Carrega logs
    this.service.obterLogs(filtros).subscribe({
      next: (res) => {
        this.registros = res.registros;
        this.dataAtual = res.data;
        this.datasOptions = res.datasDisponiveis.map(d => ({ label: d, value: d }));
        this.carregando = false;
      },
      error: (err) => {
        this.carregando = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: 'Não foi possível carregar os logs.'
        });
      }
    });

    // Carrega resumo
    this.service.obterResumo(this.filtroData || undefined).subscribe({
      next: (res) => { this.resumo = res; }
    });
  }

  limparFiltros(): void {
    this.filtroNivel = null;
    this.filtroBusca = '';
    this.filtroData = null;
    this.carregarDados();
  }

  verDetalhes(log: LogEntry): void {
    this.logSelecionado = log;
    this.dialogDetalhes = true;
  }

  getSeverity(nivel: string): string {
    switch (nivel) {
      case 'INF': return 'info';
      case 'WRN': return 'warning';
      case 'ERR': return 'danger';
      case 'FTL': return 'danger';
      default: return 'info';
    }
  }

  getIcon(nivel: string): string {
    switch (nivel) {
      case 'INF': return 'pi pi-info-circle';
      case 'WRN': return 'pi pi-exclamation-triangle';
      case 'ERR': return 'pi pi-times-circle';
      case 'FTL': return 'pi pi-ban';
      default: return 'pi pi-circle';
    }
  }
}
