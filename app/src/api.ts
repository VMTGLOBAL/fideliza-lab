import type { Cliente, ClienteResumen, Movimiento, Nivel, Recompensa } from './tipos';

export const API_URL = process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:5080';

async function pedir<T>(ruta: string, opciones?: RequestInit): Promise<T> {
  const respuesta = await fetch(`${API_URL}/api${ruta}`, {
    headers: { 'Content-Type': 'application/json' },
    ...opciones,
  });
  const datos = await respuesta.json();
  if (!respuesta.ok) {
    throw new Error(datos?.error ?? `Error ${respuesta.status}`);
  }
  return datos as T;
}

export const api = {
  clientes: () => pedir<ClienteResumen[]>('/clientes'),
  cliente: (id: number) => pedir<Cliente>(`/clientes/${id}`),
  movimientos: (id: number) => pedir<Movimiento[]>(`/clientes/${id}/movimientos?limite=8`),
  recompensas: () => pedir<Recompensa[]>('/recompensas'),
  registrarCompra: (id: number, monto: number) =>
    pedir<{ puntosGanados: number; saldo: number; nivel: Nivel }>(`/clientes/${id}/compras`, {
      method: 'POST',
      body: JSON.stringify({ monto, canal: 'app' }),
    }),
  canjear: (id: number, codigoRecompensa: string) =>
    pedir<{ saldo: number }>(`/clientes/${id}/canjes`, {
      method: 'POST',
      body: JSON.stringify({ codigoRecompensa, canal: 'app' }),
    }),
};
