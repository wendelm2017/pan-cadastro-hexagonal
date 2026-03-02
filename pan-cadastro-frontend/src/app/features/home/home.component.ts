import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
//template angular para a página inicial do sistema, que apresenta dois cards: 
// Home com cards de PF e PJ
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, CardModule, ButtonModule],
  template: `
    <div class="grid">
      <div class="col-12">
        <h1>PanCadastro</h1>
        <p class="text-500">Sistema de Cadastro de Clientes</p>
      </div>

      <div class="col-12 md:col-6">
        <p-card header="Pessoa Física" subheader="Cadastro de pessoas físicas com validação de CPF" [style]="{'height':'100%'}">
          <p>Gerencie cadastros de pessoas físicas com validação completa de CPF (algoritmo de dígitos verificadores),
          e-mail, data de nascimento e endereços vinculados via ViaCEP.</p>
          <ng-template pTemplate="footer">
            <p-button label="Acessar" icon="pi pi-arrow-right" routerLink="/pessoas-fisicas"></p-button>
          </ng-template>
        </p-card>
      </div>

      <div class="col-12 md:col-6">
        <p-card header="Pessoa Jurídica" subheader="Cadastro de empresas com validação de CNPJ" [style]="{'height':'100%'}">
          <p>Gerencie cadastros de pessoas jurídicas com validação completa de CNPJ,
          razão social, nome fantasia, inscrição estadual e endereços vinculados via ViaCEP.</p>
          <ng-template pTemplate="footer">
            <p-button label="Acessar" icon="pi pi-arrow-right" routerLink="/pessoas-juridicas"></p-button>
          </ng-template>
        </p-card>
      </div>
    </div>
  `
})
export class HomeComponent {}
