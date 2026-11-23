import { StyleSheet, Text, View } from 'react-native';
import { sombra, tema } from '../tema';
import type { Movimiento } from '../tipos';
import { formatearPuntos } from '../utilidades';

export function Historial({ movimientos }: { movimientos: Movimiento[] }) {
  return (
    <View style={[estilos.tarjeta, sombra]}>
      <Text style={estilos.titulo}>🧾 Últimos movimientos</Text>
      {movimientos.map((m, i) => {
        const esCompra = m.tipo === 'Compra';
        return (
          <View key={`${m.fecha}-${i}`} style={estilos.fila}>
            <View style={{ flex: 1 }}>
              <Text style={estilos.descripcion}>{esCompra ? `Compra en ${m.canal}` : `Canje: ${m.recompensa}`}</Text>
              <Text style={estilos.fecha}>{m.fecha}</Text>
            </View>
            <Text style={[estilos.puntos, { color: esCompra ? tema.exito : tema.peligro }]}>
              {esCompra ? '+' : '−'}
              {formatearPuntos(m.puntos)}
            </Text>
          </View>
        );
      })}
    </View>
  );
}

const estilos = StyleSheet.create({
  tarjeta: { backgroundColor: tema.tarjeta, borderRadius: tema.radio, padding: 20, marginTop: 28 },
  titulo: { color: tema.texto, fontSize: 18, fontWeight: '700', marginBottom: 8 },
  fila: { flexDirection: 'row', alignItems: 'center', paddingVertical: 10, borderBottomWidth: 1, borderBottomColor: tema.primarioClaro },
  descripcion: { color: tema.texto, fontWeight: '600' },
  fecha: { color: tema.textoSuave, fontSize: 12 },
  puntos: { fontWeight: '800', fontSize: 16 },
});
