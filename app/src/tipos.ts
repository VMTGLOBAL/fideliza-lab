export type Nivel = 'Bronce' | 'Plata' | 'Oro';

export interface ClienteResumen {
  id: number;
  nombre: string;
}

export interface Cliente {
  id: number;
  nombre: string;
  email: string;
  nivel: Nivel;
  acumulados: number;
  saldo: number;
  siguienteNivel: Nivel | null;
  puntosParaSiguienteNivel: number | null;
}

export interface Movimiento {
  fecha: string;
  tipo: 'Compra' | 'Canje';
  monto: number;
  puntos: number;
  recompensa: string | null;
  canal: string;
}

export interface Recompensa {
  codigo: string;
  nombre: string;
  puntos: number;
  categoria: string;
}
