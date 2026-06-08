using UnityEngine;
using UnityEngine.VFX;
using System.Collections; // Necessário para a Coroutine

public class OrbProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private VisualEffect visualEffect; 
    
    private Transform _targetTransform; 
    private Vector3 _moveDirection;     
    private bool _hasTarget = false;
    private bool _followTarget = false;  
    private bool _isFadingOut = false;   

    // Armazena a cor original que você definiu no VFX
    private Vector4 _corOriginal;

    private void Start()
    {
        if (visualEffect != null)
        {
            // Captura a cor exata (incluindo intensidade HDR) que já está no componente
            _corOriginal = visualEffect.GetVector4("Orb Color");
        }

        // Inicia a contagem para o fadeout
        StartCoroutine(RotinaDestruicaoComFadeout());
    }

    private IEnumerator RotinaDestruicaoComFadeout()
    {
        // Espera os 5 segundos iniciais
        yield return new WaitForSeconds(3f);

        _isFadingOut = true; 

        float tempoPassado = 0f;
        float duracaoFade = 0.5f; // 1 segundo sumindo

        while (tempoPassado < duracaoFade)
        {
            tempoPassado += Time.deltaTime;
            float progresso = tempoPassado / duracaoFade;

            // Interpola da cor original até o preto total (zero intensidade/alpha)
            Vector4 corAtual = Vector4.Lerp(_corOriginal, Vector4.zero, progresso);

            if (visualEffect != null)
            {
                visualEffect.SetVector4("Orb Color", corAtual);
            }

            yield return null; 
        }

        Destroy(gameObject);
    }

    // --- Restante do seu código de movimento ---

    public void SetTarget(Transform target, bool followTarget = true)
    {
        _followTarget = followTarget;
        _hasTarget = true;
        if (_followTarget) _targetTransform = target;
        else
        {
            Vector3 targetPosition = target.position + new Vector3(0, 0.5f, 0);
            _moveDirection = (targetPosition - transform.position).normalized;
        }
    }

    private void Update()
    {
        if (!_hasTarget) return;

        Vector3 direction = _followTarget ? 
            (_targetTransform.position + new Vector3(0, 0.5f, 0) - transform.position).normalized : 
            _moveDirection;

        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public GameObject GetTarget()
    {
        return _hasTarget ? _targetTransform.gameObject : null;
    }
    public void ChangeCharacter(GameObject gameObject)
    {
        GetComponent<Attack>().SetCharacther(gameObject);
    }
}