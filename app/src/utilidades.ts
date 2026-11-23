import type { Nivel } from './tipos';

// Umbrales de nivel (copiados del backend para pintar la barra de progreso).
export const UMBRALES: Record<Nivel, number> = { Bronce: 0, Plata: 1000, Oro: 5000 };

export const ICONO_NIVEL: Record<Nivel, string> = { Bronce: '🥉', Plata: '🥈', Oro: '🥇' };

export function formatearPuntos(puntos: number): string {
  return puntos.toLocaleString('es-EC');
}

// 1 punto = 1 centavo, igual que el estado de cuenta.
export function valorEnDolares(saldo: number): string {
  return (saldo * 0.01).toLocaleString('es-EC', { style: 'currency', currency: 'USD' });
}

export function progresoNivel(nivel: Nivel, siguiente: Nivel | null, acumulados: number): number {
  if (!siguiente) {
    return 1;
  }
  const desde = UMBRALES[nivel];
  const hasta = UMBRALES[siguiente];
  return Math.min(1, Math.max(0, (acumulados - desde) / (hasta - desde)));
}
