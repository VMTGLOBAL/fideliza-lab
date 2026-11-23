import { useState } from 'react';
import { Pressable, StyleSheet, Text, TextInput, View } from 'react-native';
import { sombra, tema } from '../tema';

interface Props {
  onRegistrar: (monto: number) => Promise<void>;
}

export function RegistrarCompra({ onRegistrar }: Props) {
  const [monto, setMonto] = useState('');
  const valor = Number(monto.replace(',', '.'));

  return (
    <View style={[estilos.tarjeta, sombra]}>
      <Text style={estilos.titulo}>🛍️ Simular una compra</Text>
      <View style={estilos.fila}>
        <TextInput
          value={monto}
          onChangeText={setMonto}
          placeholder="Monto en dólares"
          keyboardType="decimal-pad"
          style={estilos.input}
        />
        <Pressable
          disabled={!(valor > 0)}
          onPress={async () => {
            await onRegistrar(valor);
            setMonto('');
          }}
          style={[estilos.boton, !(valor > 0) && estilos.botonDesactivado]}
        >
          <Text style={estilos.botonTexto}>Registrar</Text>
        </Pressable>
      </View>
    </View>
  );
}

const estilos = StyleSheet.create({
  tarjeta: { backgroundColor: tema.tarjeta, borderRadius: tema.radio, padding: 20, marginTop: 20 },
  titulo: { color: tema.texto, fontSize: 18, fontWeight: '700', marginBottom: 12 },
  fila: { flexDirection: 'row', gap: 10 },
  input: { flex: 1, minWidth: 0, borderWidth: 1, borderColor: tema.primarioClaro, borderRadius: 12, paddingHorizontal: 14, paddingVertical: 10, fontSize: 16 },
  boton: { backgroundColor: tema.primario, borderRadius: 12, paddingHorizontal: 18, justifyContent: 'center' },
  botonDesactivado: { opacity: 0.4 },
  botonTexto: { color: '#FFFFFF', fontWeight: '700' },
});
