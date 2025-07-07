using UnityEngine;
using Enemies.Strategies;

namespace Enemies
{
    /// <summary>
    /// Factory para asignar estrategias de movimiento a los enemigos.
    /// </summary>
    public static class EnemyStrategyFactory
    {
        public enum TipoEstrategia
        {
            Normal,
            Rapido,
            Zigzag,
            Tanque,
            Aleatorio // Asigna una estrategia aleatoria
        }
        
        /// <summary>
        /// Asigna una estrategia de movimiento al enemigo basándose en el tipo especificado.
        /// </summary>
        public static void AsignarEstrategia(Enemy enemigo, TipoEstrategia tipo)
        {
            if (enemigo == null) return;
            
            // Si es aleatorio, elegir uno de los tipos disponibles
            if (tipo == TipoEstrategia.Aleatorio)
            {
                tipo = (TipoEstrategia)Random.Range(0, 4); // 0-3 para Normal, Rapido, Zigzag, Tanque
            }
            
            switch (tipo)
            {
                case TipoEstrategia.Normal:
                    enemigo.estrategiaMovimiento = new NormalMovementStrategy();
                    break;
                    
                case TipoEstrategia.Rapido:
                    enemigo.estrategiaMovimiento = new FastMovementStrategy();
                    AjustarStatsRapido(enemigo);
                    break;
                    
                case TipoEstrategia.Zigzag:
                    enemigo.estrategiaMovimiento = new ZigzagMovementStrategy();
                    AjustarStatsZigzag(enemigo);
                    break;
                    
                case TipoEstrategia.Tanque:
                    enemigo.estrategiaMovimiento = new TankMovementStrategy();
                    AjustarStatsTanque(enemigo);
                    break;
            }
            
            Debug.Log($"Enemigo {enemigo.name} asignado con estrategia: {tipo}");
        }
        
        /// <summary>
        /// Asigna estrategias a un grupo de enemigos con distribución especificada.
        /// </summary>
        public static void AsignarEstrategiasGrupo(Enemy[] enemigos, float porcentajeNormal = 0.4f, 
            float porcentajeRapido = 0.2f, float porcentajeZigzag = 0.2f, float porcentajeTanque = 0.2f)
        {
            if (enemigos == null || enemigos.Length == 0) return;
            
            // Normalizar porcentajes
            float total = porcentajeNormal + porcentajeRapido + porcentajeZigzag + porcentajeTanque;
            porcentajeNormal /= total;
            porcentajeRapido /= total;
            porcentajeZigzag /= total;
            porcentajeTanque /= total;
            
            // Mezclar array para distribución aleatoria
            for (int i = 0; i < enemigos.Length; i++)
            {
                int randomIndex = Random.Range(i, enemigos.Length);
                Enemy temp = enemigos[i];
                enemigos[i] = enemigos[randomIndex];
                enemigos[randomIndex] = temp;
            }
            
            // Asignar estrategias según porcentajes
            int index = 0;
            int cantidadNormal = Mathf.RoundToInt(enemigos.Length * porcentajeNormal);
            int cantidadRapido = Mathf.RoundToInt(enemigos.Length * porcentajeRapido);
            int cantidadZigzag = Mathf.RoundToInt(enemigos.Length * porcentajeZigzag);
            
            // Normal
            for (int i = 0; i < cantidadNormal && index < enemigos.Length; i++, index++)
            {
                AsignarEstrategia(enemigos[index], TipoEstrategia.Normal);
            }
            
            // Rápido
            for (int i = 0; i < cantidadRapido && index < enemigos.Length; i++, index++)
            {
                AsignarEstrategia(enemigos[index], TipoEstrategia.Rapido);
            }
            
            // Zigzag
            for (int i = 0; i < cantidadZigzag && index < enemigos.Length; i++, index++)
            {
                AsignarEstrategia(enemigos[index], TipoEstrategia.Zigzag);
            }
            
            // El resto son tanques
            while (index < enemigos.Length)
            {
                AsignarEstrategia(enemigos[index], TipoEstrategia.Tanque);
                index++;
            }
        }
        
        private static void AjustarStatsRapido(Enemy enemigo)
        {
            // Los enemigos rápidos tienen menos vida pero mayor recompensa
            enemigo.saludMax = Mathf.RoundToInt(enemigo.saludMax * 0.7f);
            enemigo.recompensaOro = Mathf.RoundToInt(enemigo.recompensaOro * 1.5f);
        }
        
        private static void AjustarStatsZigzag(Enemy enemigo)
        {
            // Los enemigos zigzag son balanceados pero dan más oro por ser difíciles de golpear
            enemigo.recompensaOro = Mathf.RoundToInt(enemigo.recompensaOro * 1.3f);
        }
        
        private static void AjustarStatsTanque(Enemy enemigo)
        {
            // Los tanques tienen mucha más vida y hacen más daño, pero dan mucho oro
            enemigo.saludMax = Mathf.RoundToInt(enemigo.saludMax * 2.5f);
            enemigo.dañoAlNucleo = Mathf.RoundToInt(enemigo.dañoAlNucleo * 2f);
            enemigo.recompensaOro = Mathf.RoundToInt(enemigo.recompensaOro * 2f);
        }
    }
}
