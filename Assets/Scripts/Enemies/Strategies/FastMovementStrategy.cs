using UnityEngine;

namespace Enemies.Strategies
{
    /// <summary>
    /// Estrategia de movimiento rápido: aumenta velocidad gradualmente y tiene sprints aleatorios.
    /// </summary>
    public class FastMovementStrategy : IMovementStrategy
    {
        private Transform objetivoActual;
        private int indiceWaypoint = 0;
        private float velocidadActual;
        private float tiempoProximoSprint;
        private bool enSprint = false;
        private float duracionSprint = 2f;
        private float tiempoFinSprint;
        
        private const float MULTIPLICADOR_VELOCIDAD_BASE = 1.5f;
        private const float MULTIPLICADOR_SPRINT = 2.5f;
        private const float ACELERACION = 5f;
        
        public void Mover(Enemy enemigo)
        {
            if (enemigo == null) return;
            
            // Inicializar velocidad
            if (velocidadActual == 0)
            {
                velocidadActual = enemigo.velocidad * MULTIPLICADOR_VELOCIDAD_BASE;
                tiempoProximoSprint = Time.time + Random.Range(3f, 6f);
            }
            
            // Obtener waypoint actual
            if (objetivoActual == null)
            {
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                if (objetivoActual == null) return;
            }
            
            // Gestionar sprints aleatorios
            if (!enSprint && Time.time >= tiempoProximoSprint)
            {
                IniciarSprint();
            }
            else if (enSprint && Time.time >= tiempoFinSprint)
            {
                TerminarSprint();
            }
            
            // Aplicar aceleración/desaceleración suave
            float velocidadObjetivo = enemigo.velocidad * MULTIPLICADOR_VELOCIDAD_BASE;
            if (enSprint)
            {
                velocidadObjetivo = enemigo.velocidad * MULTIPLICADOR_SPRINT;
            }
            
            velocidadActual = Mathf.Lerp(velocidadActual, velocidadObjetivo, Time.deltaTime * ACELERACION);
            
            // Mover hacia el waypoint
            enemigo.transform.position = Vector3.MoveTowards(
                enemigo.transform.position,
                objetivoActual.position,
                velocidadActual * Time.deltaTime
            );
            
            // Rotar hacia el objetivo con rotación más rápida
            Vector3 direccion = (objetivoActual.position - enemigo.transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                enemigo.transform.rotation = Quaternion.Slerp(
                    enemigo.transform.rotation, 
                    rotacionObjetivo, 
                    Time.deltaTime * 15f
                );
            }
            
            // Verificar si llegamos al waypoint
            if (Vector3.Distance(enemigo.transform.position, objetivoActual.position) < 0.1f)
            {
                indiceWaypoint++;
                objetivoActual = PathManager.Instance.GetWaypoint(indiceWaypoint);
                
                if (objetivoActual == null)
                {
                    enemigo.AlcanzarNucleo();
                }
            }
        }
        
        private void IniciarSprint()
        {
            enSprint = true;
            tiempoFinSprint = Time.time + duracionSprint;
        }
        
        private void TerminarSprint()
        {
            enSprint = false;
            tiempoProximoSprint = Time.time + Random.Range(4f, 8f);
        }
    }
}
