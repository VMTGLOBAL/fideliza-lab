import { StyleSheet, Text, View } from 'react-native';
import { sombra, tema } from '../tema';
import type { Cliente } from '../tipos';
import { ICONO_NIVEL, formatearPuntos, progresoNivel, valorEnDolares } from '../utilidades';

export function TarjetaSaldo({ cliente }: { cliente: Cliente }) {
  const progreso = progresoNivel(cliente.nivel, cliente.siguienteNivel, cliente.acumulados);

  return (
    <View style={[estilos.tarjeta, sombra]}>
      <Text style={estilos.etiqueta}>Tu saldo disponible</Text>
      <Text style={estilos.saldo}>{formatearPuntos(cliente.saldo)} pts</Text>
      <Text style={estilos.equivalente}>≈ {valorEnDolares(cliente.saldo)}</Text>

      <View style={estilos.nivel}>
        <Text style={estilos.nivelTexto}>
          {ICONO_NIVEL[cliente.nivel]} Nivel {cliente.nivel}
        </Text>
        <Text style={estilos.acumulados}>{formatearPuntos(cliente.acumulados)} acumulados</Text>
      </View>

      <View style={estilos.barra}>
        <View style={[estilos.relleno, { width: `${Math.round(progreso * 100)}%` }]} />
      </View>
      <Text style={estilos.meta}>
        {cliente.siguienteNivel
          ? `Te faltan ${formatearPuntos(cliente.puntosParaSiguienteNivel ?? 0)} puntos para ${cliente.siguienteNivel} ✨`
          : '¡Estás en el nivel más alto! 🎉'}
      </Text>
    </View>
  );
}

const estilos = StyleSheet.create({
  tarjeta: { backgroundColor: tema.tarjeta, borderRadius: tema.radio, padding: 24, marginTop: -48 },
  etiqueta: { color: tema.textoSuave, fontSize: 14, textAlign: 'center' },
  saldo: { color: tema.texto, fontSize: 40, fontWeight: '800', textAlign: 'center', marginTop: 4 },
  equivalente: { color: tema.textoSuave, textAlign: 'center', marginBottom: 16 },
  nivel: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 8 },
  nivelTexto: { color: tema.primario, fontWeight: '700', fontSize: 16 },
  acumulados: { color: tema.textoSuave },
  barra: { height: 10, borderRadius: 999, backgroundColor: tema.primarioClaro, overflow: 'hidden' },
  relleno: { height: '100%', backgroundColor: tema.primario, borderRadius: 999 },
  meta: { color: tema.textoSuave, marginTop: 8, textAlign: 'center' },
});
