using UnityEngine;

namespace Enemies.Strategies
{
    /// <summary>
    /// Estrategia de movimiento tanque: lento pero puede embestir cuando está cerca del objetivo.
    /// </summary>
    public class TankMovementStrategy : IMovementStrategy
    {
        private Transform objetivoActual;
        private int indiceWaypoint = 0;
        private float velocidadActual;
        private bool preparandoEmbestida = false;
        private float tiempoInicioEmbestida;
        private Vector3 direccionEmbestida;
        
        private const float MULTIPLICADOR_VELOCIDAD_BASE = 0.5f; // Más lento de lo normal
        private const float DISTANCIA_EMBESTIDA = 5f;
        private const float DURACION_PREPARACION = 1f;
        private const float VELOCIDAD_EMBESTIDA = 3f;
        
        public void Mover(Enemy enemigo)
        {
            if (enemigo == null) return;
            
            // Inicializar velocidad
            if (velocidadActual == 0)
            {
                velocidadActual = enemigo.velocidad * MULTIPLICADOR_VELOCIDAD_BASE;
            }
            
            // Obtener waypoint actual
            if (objetivoActual == null)
            {
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                if (objetivoActual == null) return;
            }
            
            float distanciaAlObjetivo = Vector3.Distance(enemigo.transform.position, objetivoActual.position);
            
            // Verificar si debemos preparar embestida
            if (!preparandoEmbestida && distanciaAlObjetivo <= DISTANCIA_EMBESTIDA && distanciaAlObjetivo > 0.5f)
            {
                PrepararEmbestida(enemigo);
            }
            
            // Comportamiento según estado
            if (preparandoEmbestida)
            {
                float tiempoTranscurrido = Time.time - tiempoInicioEmbestida;
                
                if (tiempoTranscurrido < DURACION_PREPARACION)
                {
                    // Durante preparación: reducir velocidad y temblar ligeramente
                    velocidadActual = enemigo.velocidad * 0.1f;
                    
                    // Efecto de temblor
                    float temblor = Mathf.Sin(tiempoTranscurrido * 30f) * 0.05f;
                    enemigo.transform.position += enemigo.transform.right * temblor;
                }
                else
                {
                    // Ejecutar embestida
                    velocidadActual = enemigo.velocidad * VELOCIDAD_EMBESTIDA;
                    preparandoEmbestida = false;
                }
            }
            else if (distanciaAlObjetivo > DISTANCIA_EMBESTIDA)
            {
                // Movimiento normal lento
                velocidadActual = Mathf.Lerp(velocidadActual, enemigo.velocidad * MULTIPLICADOR_VELOCIDAD_BASE, Time.deltaTime * 2f);
            }
            
            // Mover hacia el waypoint
            enemigo.transform.position = Vector3.MoveTowards(
                enemigo.transform.position,
                objetivoActual.position,
                velocidadActual * Time.deltaTime
            );
            
            // Rotar hacia el objetivo (más lento para tanques)
            Vector3 direccion = (objetivoActual.position - enemigo.transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                enemigo.transform.rotation = Quaternion.Slerp(
                    enemigo.transform.rotation, 
                    rotacionObjetivo, 
                    Time.deltaTime * 5f
                );
            }
            
            // Verificar si llegamos al waypoint
            if (distanciaAlObjetivo < 0.1f)
            {
                indiceWaypoint++;
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                preparandoEmbestida = false;
                
                if (objetivoActual == null)
                {
                    enemigo.AlcanzarNucleo();
                }
            }
        }
        
        private void PrepararEmbestida(Enemy enemigo)
        {
            preparandoEmbestida = true;
            tiempoInicioEmbestida = Time.time;
            direccionEmbestida = (objetivoActual.position - enemigo.transform.position).normalized;
        }
    }
}
