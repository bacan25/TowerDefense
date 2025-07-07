using UnityEngine;

namespace Enemies.Strategies
{
    /// <summary>
    /// Estrategia de movimiento normal: sigue los waypoints a velocidad constante.
    /// </summary>
    public class NormalMovementStrategy : IMovementStrategy
    {
        private Transform objetivoActual;
        private int indiceWaypoint = 0;
        
        public void Mover(Enemy enemigo)
        {
            if (enemigo == null) return;
            
            // Obtener waypoint actual si no lo tenemos
            if (objetivoActual == null)
            {
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                if (objetivoActual == null) return;
            }
            
            // Mover hacia el waypoint
            enemigo.transform.position = Vector3.MoveTowards(
                enemigo.transform.position,
                objetivoActual.position,
                enemigo.velocidad * Time.deltaTime
            );
            
            // Rotar hacia el objetivo
            Vector3 direccion = (objetivoActual.position - enemigo.transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                enemigo.transform.rotation = Quaternion.Slerp(
                    enemigo.transform.rotation, 
                    rotacionObjetivo, 
                    Time.deltaTime * 10f
                );
            }
            
            // Verificar si llegamos al waypoint
            if (Vector3.Distance(enemigo.transform.position, objetivoActual.position) < 0.1f)
            {
                indiceWaypoint++;
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                
                // Si no hay más waypoints, llegamos al núcleo
                if (objetivoActual == null)
                {
                    enemigo.AlcanzarNucleo();
                }
            }
        }
    }
}
