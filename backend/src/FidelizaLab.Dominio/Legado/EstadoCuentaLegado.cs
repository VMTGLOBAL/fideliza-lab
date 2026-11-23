// EstadoCuentaLegado.cs
// Estado de cuenta de puntos. Migrado del sistema anterior en 2021.
// NO TOCAR sin avisar a marketing, "funciona".
// Ultima modificacion: se agrego el canal app (2023)
#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;

namespace FidelizaLab.Dominio.Legado
{
    public class EstadoCuenta
    {
        public int ClienteId;
        public int PuntosGanados;
        public int PuntosCanjeados;
        public int Saldo;
        public string Nivel;
        public double ValorEnDolares;
        public Dictionary<string, int> GanadoPorMes;
        public List<string> TopRecompensas;
        public string UltimaCompra;
    }

    public class EstadoCuentaLegado
    {
        public static Hashtable _cache = new Hashtable();
        public static ArrayList _tmp = new ArrayList();
        public static bool flag = false;

        public static EstadoCuenta Generar(int clienteId, List<Movimiento> movimientos, List<Recompensa> recompensas)
        {
            _cache = new Hashtable();
            _tmp = new ArrayList();
            flag = false;
            EstadoCuenta ec = new EstadoCuenta();
            ec.ClienteId = clienteId;
            int i, j;

            // ganados y canjeados
            for (i = 0; i < movimientos.Count; i++)
            {
                Movimiento m = movimientos[i];
                if (m.ClienteId == clienteId)
                {
                    if (m.Tipo == TipoMovimiento.Compra)
                    {
                        ec.PuntosGanados = ec.PuntosGanados + m.Puntos;
                    }
                    if (m.Tipo == TipoMovimiento.Canje)
                    {
                        ec.PuntosCanjeados = ec.PuntosCanjeados + m.Puntos;
                    }
                }
            }
            ec.Saldo = ec.PuntosGanados - ec.PuntosCanjeados;

            // nivel (copiado de la pantalla vieja)
            if (ec.PuntosGanados >= 5000)
            {
                ec.Nivel = "ORO";
            }
            else if (ec.PuntosGanados >= 1000)
            {
                ec.Nivel = "PLATA";
            }
            else
            {
                ec.Nivel = "BRONCE";
            }

            // valor en dolares (1 punto = 1 centavo)
            ec.ValorEnDolares = Math.Round(ec.Saldo * 0.01 * 100) / 100;

            // ganado por mes
            Dictionary<string, int> meses = new Dictionary<string, int>();
            for (i = 0; i < movimientos.Count; i++)
            {
                Movimiento m2 = movimientos[i];
                if (m2.ClienteId == clienteId && m2.Tipo == TipoMovimiento.Compra)
                {
                    string mes = m2.Fecha.ToString("yyyy-MM-dd").Substring(0, 7);
                    if (!meses.ContainsKey(mes)) meses[mes] = 0;
                    meses[mes] = meses[mes] + m2.Puntos;
                }
            }
            ec.GanadoPorMes = meses;

            // top 3 recompensas canjeadas
            for (i = 0; i < movimientos.Count; i++)
            {
                Movimiento m3 = movimientos[i];
                if (m3.ClienteId == clienteId && m3.Tipo == TipoMovimiento.Canje)
                {
                    if (_cache[m3.CodigoRecompensa] == null) _cache[m3.CodigoRecompensa] = 0;
                    _cache[m3.CodigoRecompensa] = (int)_cache[m3.CodigoRecompensa] + 1;
                }
            }
            foreach (DictionaryEntry e in _cache)
            {
                bool existe = false;
                for (j = 0; j < recompensas.Count; j++)
                {
                    if (recompensas[j].Codigo == (string)e.Key) existe = true;
                }
                if (existe == false)
                {
                    flag = true;
                    continue;
                }
                _tmp.Add(new object[] { e.Key, e.Value });
            }
            // ordenar (burbuja, el Sort con comparador fallaba en produccion)
            for (i = 0; i < _tmp.Count; i++)
            {
                for (j = 0; j < _tmp.Count - 1 - i; j++)
                {
                    object[] a = (object[])_tmp[j];
                    object[] b = (object[])_tmp[j + 1];
                    if ((int)a[1] < (int)b[1] || ((int)a[1] == (int)b[1] && string.CompareOrdinal((string)a[0], (string)b[0]) > 0))
                    {
                        _tmp[j] = b;
                        _tmp[j + 1] = a;
                    }
                }
            }
            ec.TopRecompensas = new List<string>();
            for (i = 0; i < _tmp.Count && i < 3; i++)
            {
                string codigo = (string)((object[])_tmp[i])[0];
                for (j = 0; j < recompensas.Count; j++)
                {
                    if (recompensas[j].Codigo == codigo) ec.TopRecompensas.Add(recompensas[j].Nombre);
                }
            }

            // ultima compra
            ec.UltimaCompra = "";
            for (i = 0; i < movimientos.Count; i++)
            {
                Movimiento m4 = movimientos[i];
                if (m4.ClienteId == clienteId && m4.Tipo == TipoMovimiento.Compra)
                {
                    string f = m4.Fecha.ToString("yyyy-MM-dd");
                    if (ec.UltimaCompra == "" || string.Compare(f, ec.UltimaCompra) > 0) ec.UltimaCompra = f;
                }
            }

            return ec;
        }
    }
}
