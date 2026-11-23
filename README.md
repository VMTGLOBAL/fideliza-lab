# Fideliza Lab

Repositorio de práctica del taller **Programación con IA para Desarrolladores** de VIAMATICA.

Es una app de fidelización de clientes: los clientes acumulan puntos con sus compras, suben de nivel y canjean recompensas. Todos los datos son ficticios.

- **Backend:** API en .NET 10 (`backend/`), con pruebas xUnit.
- **App:** React Native con Expo (`app/`), que se puede abrir en el navegador o en el teléfono con Expo Go.
- **Datos:** clientes, recompensas y movimientos de ejemplo (`data/`). La API los carga en memoria al iniciar; los cambios se pierden al reiniciarla.

## Cómo se ejecuta

Necesitas el SDK de .NET 10 y Node.js LTS. Usa dos terminales desde la raíz del repositorio.

```powershell
# Terminal 1: API en http://localhost:5080
dotnet run --project backend/src/FidelizaLab.Api

# Terminal 2: app en el navegador
cd app
npm install
npm run web
```

Pruebas del backend:

```powershell
dotnet test
```

También puedes abrir `FidelizaLab.sln` en Visual Studio.

Para abrir la app en tu teléfono con Expo Go, la API debe ser accesible desde la red: define `EXPO_PUBLIC_API_URL` con la IP de tu computadora (por ejemplo `http://192.168.1.20:5080`) antes de `npm start`.

## Endpoints

| Método | Ruta | Qué hace |
|---|---|---|
| GET | `/api/clientes` | Lista de clientes |
| GET | `/api/clientes/{id}` | Saldo, nivel y puntos para el siguiente nivel |
| GET | `/api/clientes/{id}/movimientos` | Últimos movimientos |
| GET | `/api/recompensas` | Catálogo de recompensas |
| POST | `/api/clientes/{id}/compras` | Registra una compra `{ "monto": 45.90 }` |
| POST | `/api/clientes/{id}/canjes` | Canjea una recompensa `{ "codigoRecompensa": "CAF-01" }` |
| GET | `/api/clientes/{id}/estado-cuenta` | Estado de cuenta (módulo heredado) |

## Reglas de negocio

Estas reglas son la fuente de verdad. Si el código o una prueba las contradice, manda la regla.

1. **Acumulación:** cada compra da 1 punto por cada dólar completo. Los centavos no suman ($25,90 da 25 puntos).
2. **Niveles:** el nivel depende de los puntos acumulados desde el alta. **Bronce** por debajo de 1.000; **Plata desde 1.000 inclusive**; **Oro desde 5.000 inclusive**.
3. **Multiplicador:** al acumular, Plata multiplica los puntos por 1,25 y Oro por 1,5. El resultado se trunca a entero.
4. **Canje:** un canje solo procede si el saldo alcanza. El saldo nunca puede quedar negativo: si no alcanza, el canje se rechaza con un error y el saldo no cambia.
5. **Saldo y acumulados:** los canjes restan del saldo, pero no de los acumulados. El nivel nunca baja por canjear.
6. **Valor del punto:** cada punto equivale a $0,01 en el estado de cuenta.
