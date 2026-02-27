import { Routes } from '@angular/router';
//definição das rotas do aplicativo, associando caminhos a componentes específicos. 
// A rota raiz ('') carrega o componente HomeComponent, enquanto as rotas 
// '/pessoas-fisicas', '/pessoas-juridicas' e '/auditoria' carregam os componentes 
// correspondentes para cada seção do sistema. A rota '**' é um wildcard que redireciona
//  para a página inicial caso o caminho não corresponda a nenhuma rota definida.
export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'pessoas-fisicas',
    loadComponent: () => import('./features/pessoa-fisica/pessoa-fisica-list.component').then(m => m.PessoaFisicaListComponent)
  },
  {
    path: 'pessoas-juridicas',
    loadComponent: () => import('./features/pessoa-juridica/pessoa-juridica-list.component').then(m => m.PessoaJuridicaListComponent)
  },
  {
    path: 'auditoria',
    loadComponent: () => import('./features/auditoria/auditoria.component').then(m => m.AuditoriaComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
