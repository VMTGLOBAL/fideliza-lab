import { Pressable, StyleSheet, Text, View } from 'react-native';
import { sombra, tema } from '../tema';
import type { Recompensa } from '../tipos';
import { formatearPuntos } from '../utilidades';

interface Props {
  recompensas: Recompensa[];
  saldo: number;
  onCanjear: (codigo: string) => Promise<void>;
}

export function ListaRecompensas({ recompensas, saldo, onCanjear }: Props) {
  return (
    <View style={{ marginTop: 28 }}>
      <Text style={estilos.titulo}>🎁 Recompensas</Text>
      <View style={estilos.grilla}>
        {recompensas.map((r) => {
          // El botón se desactiva si no alcanza el saldo.
          const alcanza = saldo >= r.puntos;
          return (
            <View key={r.codigo} style={[estilos.tarjeta, sombra]}>
              <Text style={estilos.categoria}>{r.categoria}</Text>
              <Text style={estilos.nombre}>{r.nombre}</Text>
              <Text style={estilos.puntos}>{formatearPuntos(r.puntos)} pts</Text>
              <Pressable
                disabled={!alcanza}
                onPress={() => onCanjear(r.codigo)}
                style={[estilos.boton, !alcanza && estilos.botonDesactivado]}
              >
                <Text style={estilos.botonTexto}>{alcanza ? 'Canjear' : 'Te faltan puntos'}</Text>
              </Pressable>
            </View>
          );
        })}
      </View>
    </View>
  );
}

const estilos = StyleSheet.create({
  titulo: { color: tema.texto, fontSize: 20, fontWeight: '800', marginBottom: 12 },
  grilla: { flexDirection: 'row', flexWrap: 'wrap', gap: 12 },
  tarjeta: { backgroundColor: tema.tarjeta, borderRadius: tema.radio, padding: 16, width: '48%', minWidth: 150, flexGrow: 1 },
  categoria: { color: tema.primario, fontSize: 12, fontWeight: '700', textTransform: 'uppercase' },
  nombre: { color: tema.texto, fontSize: 16, fontWeight: '700', marginVertical: 6 },
  puntos: { color: tema.textoSuave, marginBottom: 12 },
  boton: { backgroundColor: tema.primario, borderRadius: 12, paddingVertical: 10, alignItems: 'center' },
  botonDesactivado: { backgroundColor: '#C4B5FD' },
  botonTexto: { color: '#FFFFFF', fontWeight: '700' },
});
