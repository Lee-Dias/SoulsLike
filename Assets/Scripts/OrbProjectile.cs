using UnityEngine;

public class OrbProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Transform _targetTransform; // Usado se estiver seguindo o alvo em tempo real
    private Vector3 _moveDirection;     // Direção fixa calculada se NÃO estiver seguindo
    private bool _hasTarget = false;
    private bool _followTarget = false;  

    private void Start()
    {
        // Destrói a orbe após 5 segundos para não pesar na memória do jogo
        Destroy(gameObject, 5f); 
    }

    public void SetTarget(Transform target, bool followTarget = false)
    {
        _followTarget = followTarget;
        _hasTarget = true;

        if (_followTarget)
        {
            _targetTransform = target;
        }
        else
        {
            // 1. Calcula a posição ideal onde o alvo está agora
            Vector3 targetPosition = target.position + new Vector3(0, 0.5f, 0);
            
            // 2. Calcula a direção exata da orbe até essa posição e salva permanentemente
            _moveDirection = (targetPosition - transform.position).normalized;
        }
    }

    private void Update()
    {
        if (!_hasTarget) return;

        Vector3 direction;

        if (_followTarget)
        {
            if (_targetTransform == null) return; 
            
            // Se estiver seguindo, calcula a direção para a posição atual do alvo a cada frame
            Vector3 currentDestination = _targetTransform.position + new Vector3(0, 0.5f, 0);
            direction = (currentDestination - transform.position).normalized;
        }
        else
        {
            // Se NÃO estiver seguindo, usa a direção fixa calculada no início (fazendo ela passar direto)
            direction = _moveDirection;
        }

        // 1. Move o projétil na direção definida
        transform.position += direction * speed * Time.deltaTime;

        // 2. Rotaciona o projétil para olhar para onde está indo
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