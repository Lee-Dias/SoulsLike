using UnityEngine;

public class OrbProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Transform _target; // Variável interna para guardar o alvo
    private Transform Curr; // Variável para guardar a posição constante, se necessário
    private bool AlwaysFollow = true; 
    // Este método é chamado pelo Boss ou Spawner para definir quem seguir
    public void SetTarget(Transform target, bool constantFollow = true)
    {
        Vector3 newTargetPosition = new Vector3(target.position.x, transform.position.y + 1.5f, target.position.z);
        target.position = newTargetPosition;
        _target = target;
        Curr = target;
        AlwaysFollow = constantFollow;
    }

    private void Update()
    {
        // Se não tiver alvo, o projétil vai apenas para frente ou fica parado
        if (_target == null) return;

        if (AlwaysFollow)
        {
            Curr = _target; // Atualiza o alvo constantemente
        }
        // 1. Calcula a direção
        Vector3 direction = (Curr.position - transform.position).normalized;

        // 2. Move o projétil
        transform.position += direction * speed * Time.deltaTime;

        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void ChangeCharacter(GameObject gameObject)
    {
        GetComponent<Attack>().SetCharacther(gameObject);
    }

}