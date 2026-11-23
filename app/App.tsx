import { StatusBar } from 'expo-status-bar';
import { useCallback, useEffect, useState } from 'react';
import { ActivityIndicator, SafeAreaView, ScrollView, StyleSheet, Text, View } from 'react-native';
import { API_URL, api } from './src/api';
import { Historial } from './src/componentes/Historial';
import { ListaRecompensas } from './src/componentes/ListaRecompensas';
import { RegistrarCompra } from './src/componentes/RegistrarCompra';
import { SelectorCliente } from './src/componentes/SelectorCliente';
import { TarjetaSaldo } from './src/componentes/TarjetaSaldo';
import { tema } from './src/tema';
import type { Cliente, ClienteResumen, Movimiento, Recompensa } from './src/tipos';

export default function App() {
  const [clientes, setClientes] = useState<ClienteResumen[]>([]);
  const [idCliente, setIdCliente] = useState(1);
  const [cliente, setCliente] = useState<Cliente | null>(null);
  const [movimientos, setMovimientos] = useState<Movimiento[]>([]);
  const [recompensas, setRecompensas] = useState<Recompensa[]>([]);
  const [mensaje, setMensaje] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const cargarCliente = useCallback(async (id: number) => {
    try {
      const [c, m] = await Promise.all([api.cliente(id), api.movimientos(id)]);
      setCliente(c);
      setMovimientos(m);
      setError(null);
    } catch {
      setError(`No se pudo conectar con la API en ${API_URL}. ¿Está corriendo el backend?`);
    }
  }, []);

  useEffect(() => {
    Promise.all([api.clientes(), api.recompensas()])
      .then(([cs, rs]) => {
        setClientes(cs);
        setRecompensas(rs);
      })
      .catch(() => setError(`No se pudo conectar con la API en ${API_URL}. ¿Está corriendo el backend?`));
  }, []);

  useEffect(() => {
    setMensaje(null);
    cargarCliente(idCliente);
  }, [idCliente, cargarCliente]);

  async function registrarCompra(monto: number) {
    const r = await api.registrarCompra(idCliente, monto);
    setMensaje(`¡Ganaste ${r.puntosGanados} puntos! 🎉`);
    await cargarCliente(idCliente);
  }

  async function canjear(codigo: string) {
    try {
      await api.canjear(idCliente, codigo);
      setMensaje('¡Canje exitoso! Disfruta tu recompensa 🎁');
      await cargarCliente(idCliente);
    } catch (e) {
      setMensaje((e as Error).message);
    }
  }

  return (
    <SafeAreaView style={estilos.pantalla}>
      <StatusBar style="light" />
      <ScrollView contentContainerStyle={estilos.scroll}>
        <View style={estilos.encabezado}>
          <Text style={estilos.marca}>✨ Fideliza</Text>
          <Text style={estilos.saludo}>{cliente ? `Hola, ${cliente.nombre.split(' ')[0]} 👋` : 'Hola 👋'}</Text>
          <SelectorCliente clientes={clientes} seleccionado={idCliente} onSeleccionar={setIdCliente} />
        </View>

        <View style={estilos.contenido}>
          {error && <Text style={estilos.error}>{error}</Text>}
          {!error && !cliente && <ActivityIndicator color={tema.primario} style={{ marginTop: 40 }} />}
          {cliente && (
            <>
              <TarjetaSaldo cliente={cliente} />
              {mensaje && <Text style={estilos.mensaje}>{mensaje}</Text>}
              <RegistrarCompra onRegistrar={registrarCompra} />
              <ListaRecompensas recompensas={recompensas} saldo={cliente.saldo} onCanjear={canjear} />
              <Historial movimientos={movimientos} />
            </>
          )}
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const estilos = StyleSheet.create({
  pantalla: { flex: 1, backgroundColor: tema.fondo },
  scroll: { paddingBottom: 48 },
  encabezado: { backgroundColor: tema.primario, paddingHorizontal: 24, paddingTop: 32, paddingBottom: 72, borderBottomLeftRadius: 32, borderBottomRightRadius: 32 },
  marca: { color: '#DDD6FE', fontWeight: '800', fontSize: 14, letterSpacing: 1 },
  saludo: { color: '#FFFFFF', fontSize: 28, fontWeight: '800', marginVertical: 12 },
  contenido: { paddingHorizontal: 20, width: '100%', maxWidth: 560, alignSelf: 'center' },
  mensaje: { marginTop: 16, padding: 12, borderRadius: 12, backgroundColor: tema.primarioClaro, color: tema.primarioOscuro, textAlign: 'center', fontWeight: '600' },
  error: { marginTop: 24, padding: 16, borderRadius: 12, backgroundColor: '#FEE2E2', color: '#991B1B', textAlign: 'center' },
});
