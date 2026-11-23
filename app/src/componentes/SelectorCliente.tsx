import { Pressable, ScrollView, StyleSheet, Text } from 'react-native';
import { tema } from '../tema';
import type { ClienteResumen } from '../tipos';

interface Props {
  clientes: ClienteResumen[];
  seleccionado: number;
  onSeleccionar: (id: number) => void;
}

export function SelectorCliente({ clientes, seleccionado, onSeleccionar }: Props) {
  return (
    <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={estilos.fila}>
      {clientes.slice(0, 6).map((c) => {
        const activo = c.id === seleccionado;
        return (
          <Pressable key={c.id} onPress={() => onSeleccionar(c.id)} style={[estilos.chip, activo && estilos.chipActivo]}>
            <Text style={[estilos.texto, activo && estilos.textoActivo]}>{c.nombre.split(' ')[0]}</Text>
          </Pressable>
        );
      })}
    </ScrollView>
  );
}

const estilos = StyleSheet.create({
  fila: { gap: 8, paddingVertical: 4 },
  chip: { paddingHorizontal: 16, paddingVertical: 8, borderRadius: 999, backgroundColor: 'rgba(255,255,255,0.18)' },
  chipActivo: { backgroundColor: '#FFFFFF' },
  texto: { color: '#FFFFFF', fontWeight: '600' },
  textoActivo: { color: tema.primario },
});
