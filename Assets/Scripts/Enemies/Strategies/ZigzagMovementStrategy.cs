using UnityEngine;

namespace Enemies.Strategies
{
    /// <summary>
    /// Estrategia de movimiento en zigzag: se mueve lateralmente mientras avanza para evadir disparos.
    /// </summary>
    public class ZigzagMovementStrategy : IMovementStrategy
    {
        private Transform objetivoActual;
        private int indiceWaypoint = 0;
        private float tiempoZigzag = 0f;
        private float frecuenciaZigzag = 2f; // Cambios por segundo
        private float amplitudZigzag = 2f; // Distancia lateral
        private Vector3 direccionBase;
        private Vector3 posicionBase;
        
        public void Mover(Enemy enemigo)
        {
            if (enemigo == null) return;
            
            // Obtener waypoint actual
            if (objetivoActual == null)
            {
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                if (objetivoActual == null) return;
                ActualizarDireccionBase(enemigo);
            }
            
            tiempoZigzag += Time.deltaTime;
            
            // Calcular dirección hacia el waypoint
            Vector3 direccionObjetivo = (objetivoActual.position - enemigo.transform.position).normalized;
            
            // Calcular desplazamiento lateral (perpendicular a la dirección)
            Vector3 direccionLateral = Vector3.Cross(direccionObjetivo, Vector3.up).normalized;
            float desplazamientoLateral = Mathf.Sin(tiempoZigzag * frecuenciaZigzag * Mathf.PI) * amplitudZigzag;
            
            // Combinar movimiento hacia adelante con zigzag
            Vector3 movimiento = direccionObjetivo * enemigo.velocidad + direccionLateral * desplazamientoLateral;
            
            // Aplicar movimiento
            enemigo.transform.position += movimiento * Time.deltaTime;
            
            // Rotar suavemente hacia la dirección de movimiento
            if (movimiento != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(movimiento);
                enemigo.transform.rotation = Quaternion.Slerp(
                    enemigo.transform.rotation, 
                    rotacionObjetivo, 
                    Time.deltaTime * 8f
                );
            }
            
            // Verificar si llegamos al waypoint (con mayor tolerancia por el zigzag)
            if (Vector3.Distance(enemigo.transform.position, objetivoActual.position) < 1.5f)
            {
                indiceWaypoint++;
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                
                if (objetivoActual == null)
                {
                    enemigo.AlcanzarNucleo();
                }
                else
                {
                    ActualizarDireccionBase(enemigo);
                    tiempoZigzag = 0f; // Reiniciar patrón zigzag
                }
            }
        }
        
        private void ActualizarDireccionBase(Enemy enemigo)
        {
            if (objetivoActual != null)
            {
                direccionBase = (objetivoActual.position - enemigo.transform.position).normalized;
                posicionBase = enemigo.transform.position;
            }
        }
    }
}
