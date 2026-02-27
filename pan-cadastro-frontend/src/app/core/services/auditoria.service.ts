import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

//esse serviço é responsável por interagir com a API de auditoria para obter logs
// e resumos de logs. Ele define as interfaces para os dados de log e os métodos 
// para buscar os logs com filtros e obter um resumo dos logs por data.
export interface LogEntry {
  timestamp: string;
  nivel: string;
  mensagem: string;
  origem: string;
  detalhes: string | null;
}

export interface LogPageResponse {
  registros: LogEntry[];
  total: number;
  data: string;
  datasDisponiveis: string[];
}

export interface LogResumo {
  info: number;
  warning: number;
  error: number;
  fatal: number;
  data: string;
}

@Injectable({ providedIn: 'root' })
export class AuditoriaService {
  private readonly url = `${environment.apiUrl}/logs`;

  constructor(private http: HttpClient) {}

  obterLogs(filtros?: {
    nivel?: string;
    busca?: string;
    data?: string;
    limite?: number;
  }): Observable<LogPageResponse> {
    let params = new HttpParams();
    if (filtros?.nivel) params = params.set('nivel', filtros.nivel);
    if (filtros?.busca) params = params.set('busca', filtros.busca);
    if (filtros?.data) params = params.set('data', filtros.data);
    if (filtros?.limite) params = params.set('limite', filtros.limite.toString());
    return this.http.get<LogPageResponse>(this.url, { params });
  }

  obterResumo(data?: string): Observable<LogResumo> {
    let params = new HttpParams();
    if (data) params = params.set('data', data);
    return this.http.get<LogResumo>(`${this.url}/resumo`, { params });
  }
}
