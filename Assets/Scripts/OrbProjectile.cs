using UnityEngine;

public class OrbProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Transform _target; // Variável interna para guardar o alvo

    // Este método é chamado pelo Boss ou Spawner para definir quem seguir
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        // Se não tiver alvo, o projétil vai apenas para frente ou fica parado
        if (_target == null) return;

        // 1. Calcula a direção
        Vector3 direction = (_target.position - transform.position).normalized;

        // 2. Move o projétil
        transform.position += direction * speed * Time.deltaTime;

        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

}