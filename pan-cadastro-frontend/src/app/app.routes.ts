import { Routes } from '@angular/router';
// rotas com lazy loading
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
