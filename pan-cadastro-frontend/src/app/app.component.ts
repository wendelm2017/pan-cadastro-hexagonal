import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { MenubarModule } from 'primeng/menubar';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
// Shell da aplicação: topbar PAN + menu + router-outlet
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MenubarModule, ToastModule, ConfirmDialogModule],
  template: `
    <p-toast position="top-right"></p-toast>
    <p-confirmDialog></p-confirmDialog>
    <div class="pan-topbar">
      <div class="pan-topbar-inner">
        <div class="pan-logo-area">
          <img src="logo-pan.png" alt="PAN" height="32" />
          <span class="pan-title">Banco Pan</span>
        </div>
      </div>
    </div>
    <p-menubar [model]="menuItems" styleClass="pan-nav"></p-menubar>
    <div class="pan-content">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [`
    :host { display: block; min-height: 100vh; background: #f0f4f8; }

    .pan-topbar {
      background: #fff;
      padding: 12px 24px;
      border-bottom: 1px solid #e0e0e0;
    }
    .pan-topbar-inner {
      max-width: 1400px;
      margin: 0 auto;
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    .pan-logo-area {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .pan-logo-area img {
      border-radius: 6px;
    }
    .pan-title {
      color: #00AEEF;
      font-size: 20px;
      font-weight: 700;
      letter-spacing: -0.5px;
    }

    .pan-content {
      max-width: 1400px;
      margin: 0 auto;
      padding: 24px;
    }
  `]
})
export class AppComponent {
  menuItems: MenuItem[] = [
    { label: 'Início', icon: 'pi pi-home', routerLink: '/' },
    { label: 'Pessoa Física', icon: 'pi pi-user', routerLink: '/pessoas-fisicas' },
    { label: 'Pessoa Jurídica', icon: 'pi pi-briefcase', routerLink: '/pessoas-juridicas' },
    { label: 'Auditoria', icon: 'pi pi-list', routerLink: '/auditoria' }
  ];
}